using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebConnections.Core.Common
{
    public static class Consts
    {
        /// <summary>
        /// Data zero (padrão) de referência para conversão de valores inteiros em data/hora
        /// </summary>
        public static DateTime DateTimeBase = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

    }
}
