using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace WebConnections.Core.Common
{
    /// <summary>
    /// Extensão para auxílio a manipulação em ObservableCollection
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Conversão para ObservableCollection
        /// </summary>
        /// <typeparam name="T">Tipo da lista</typeparam>
        /// <param name="col">Lista a ser convertida</param>
        /// <returns>Lista convertida</returns>
        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> col)
        {
            return new ObservableCollection<T>(col);
        }
    }

    /// <summary>
    /// Classe contendo métodos públicos comuns
    /// </summary>
    public static class Utils
    {
        #region Conversão para ObservableCollection

        /// <summary>
        /// Conversão de enumerados definidos em uma lista de objetos
        /// </summary>
        /// <param name="listaEntrada">Lista de entrada</param>
        /// <returns>Lista convertida</returns>
        public static ObservableCollection<object> ToObservableCollection(IEnumerable listaEntrada)
        {
            return new ObservableCollection<object>(listaEntrada.Cast<object>());
        }

        /// <summary>
        ///  Conversão de enumerados genéricos em uma lista genérica
        /// </summary>
        /// <typeparam name="T">Lista genérica</typeparam>
        /// <param name="listaEntrada">Lista de entrada</param>
        /// <returns>Lista convertida</returns>
        public static ObservableCollection<T> ToObservableCollection<T>(IEnumerable<T> listaEntrada)
        {
            return new ObservableCollection<T>(listaEntrada);
        }

        /// <summary>
        /// Conversão de enumerados definidos (não-genéricos) em uma lista genérica
        /// </summary>
        /// <typeparam name="T">Lista genérica</typeparam>
        /// <param name="listaEntrada">Lista de entrada</param>
        /// <returns>Lista convertida</returns>
        public static ObservableCollection<T> ToObservableCollection<T>(IEnumerable listaEntrada)
        {
            return new ObservableCollection<T>(listaEntrada.Cast<T>());
        }

        #endregion Conversão para ObservableCollection

        public static void SetProperty(object instance, string name, object value)
        {
            var setMethod = instance.GetType().GetProperty(name).GetSetMethod();
            setMethod.Invoke(instance, new object[] { value });
        }

        #region Manipulação sobre jSON

        /// <summary>
        /// Converter a classe base em um objeto do tipo Json
        /// </summary>
        /// <typeparam name="T">Classe base</typeparam>
        /// <param name="baseClass">Tipo da classe</param>
        /// <returns>json no formato padrão</returns>
        public static string SerializerJson<T>(T baseClass)
        {
            try
            {
                return JsonConvert.SerializeObject(baseClass);
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Converter o objeto do tipo Json na classe base
        /// </summary>
        /// <typeparam name="T">Classe base</typeparam>
        /// <param name="jsonString">objeto Json</param>
        /// <returns>Classe que foi gerada</returns>
        public static T DeserializerJson<T>(string jsonString)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(jsonString);
            }
            catch
            {
                return Activator.CreateInstance<T>();
            }
        }

        #endregion

        /// <summary>
        /// Fixar a cultura padrão da aplicação.
        /// </summary>
        /// <returns>System.CultureInfo contendo as configurações padrões</returns>
        public static CultureInfo DefaultCultureInfo()
        {
            CultureInfo ci = new CultureInfo("pt-BR");
            ci.DateTimeFormat.ShortDatePattern = "dd/MM/yyyy";
            ci.DateTimeFormat.ShortTimePattern = "hh:mm:ss tt";
            return ci;
        }

        /// <summary>
        /// Convert um valor inteiro em uma Data/Hora local
        /// </summary>
        /// <param name="value">Valor inteiro válido</param>
        /// <returns>Data no formato da cultura corrente</returns>
        public static DateTime IntToLocalTime(int value)
        {
            return Consts.DateTimeBase.AddSeconds(value).ToLocalTime();
        }

        /// <summary>
        /// Convert um valor inteiro em uma Data/Hora local
        /// </summary>
        /// <param name="value">Valor inteiro válido</param>
        /// <returns>Data no formato da cultura corrente</returns>
        public static int LocalTimeToInt(DateTime value)
        {
            TimeSpan diff = value.ToUniversalTime() - Consts.DateTimeBase;
            return (int)Math.Floor(diff.TotalSeconds);
        }

        #region Manipulação sobre caracteres, textos e números

        /// <summary>
        /// Converter para número com duas casas decimais.
        /// O CultureInfo da aplicação está definido para pt-BR
        /// </summary>
        /// <param name="value">Valor a ser convertido</param>
        /// <returns>Valor convertido</returns>
        public static string ConvertToDecimal(string value)
        {
            return ConvertToDecimal(value, '.', ',');
        }
        /// <summary>
        /// Converter para número com duas casas decimais.
        /// </summary>
        /// <param name="value">Valor a ser convertido</param>
        /// <param name="separatorIN">Separador de origem</param>
        /// <param name="separatorOUT">Separador para substituição</param>
        /// <returns>Valor convertido</returns>
        public static string ConvertToDecimal(string value, char separatorIN, char separatorOUT)
        {
            if (string.IsNullOrEmpty(value))
                value = string.Concat("0", separatorOUT, "00");
            decimal conVal = 0;
            if (decimal.TryParse(value.Replace(separatorIN, separatorOUT), out conVal))
                return conVal.ToString("n2");
            else
                return value;
        }
        public static int? ConvertToInt32(string value, int? defaultValue = null)
        {
            int conVal = 0;
            if (int.TryParse(value, out conVal))
                return conVal;
            else
                return defaultValue;
        }

        /// <summary>
        /// Remove caracteres especiais
        /// </summary>
        /// <param name="text">Texto de entrada</param>
        /// <returns>Texto sem caracteres especiais</returns>
        public static string RemoveEspecialCharacters(string text)
        {
            return RemoveEspecialCharacters(text, string.Empty);
        }

        public static string GetSlug(string text)
        {
            Regex regex = new Regex(@"[^a-z0-9]", RegexOptions.None);
            return regex.Replace(ChangeLetter(text).ToLower(), "-");
        }

        /// <summary>
        /// Remove caracteres especiais
        /// </summary>
        /// <param name="text">Texto de entrada</param>
        /// <param name="exceptionsChar">Caracteres a serem desconsiderados como especais</param>
        /// <returns>Texto sem caracteres especiais</returns>
        public static string RemoveEspecialCharacters(string text, string exceptionsChar)
        {
            Regex regex = new Regex(string.Format(@"[^a-zA-Z0-9\{0}\s]", exceptionsChar), (RegexOptions)0);
            return regex.Replace(ChangeLetter(text), string.Empty);
        }

        /// <summary>
        /// Alterar caracteres com acentos para sem acento
        /// </summary>
        /// <param name="text">Texto de entrada</param>
        /// <returns>Texto sem caracteres acentuados</returns>
        public static string ChangeLetter(string text)
        {
            string comAcentos = "ÄÅÁÂÀÃäáâàãÉÊËÈéêëèÍÎÏÌíîïìÖÓÔÒÕöóôòõÜÚÛüúûùÇç";
            string semAcentos = "AAAAAAaaaaaEEEEeeeeIIIIiiiiOOOOOoooooUUUuuuuCc";
            for (int i = 0; i < comAcentos.Length; i++)
            {
                text = text.Replace(comAcentos[i].ToString(), semAcentos[i].ToString());
            }
            return text;
        }

        /// <summary>
        /// Remover as tagas HTML de um determinado texto
        /// </summary>
        /// <param name="html">string HTML</param>
        /// <returns>String sem as tags HTML</returns>
        public static string ClearHTMLTags(string html)
        {
            string pattern = @"<(.|\n)*?>";
            html = Regex.Replace(html, pattern, string.Empty);
            return Regex.Replace(html, @"\s+", " ").Trim();
        }

        #endregion
    }
}
