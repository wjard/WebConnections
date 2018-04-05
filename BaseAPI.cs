using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security;
using System.Text;
using WebConnections.Core.Common;
using WebConnections.Core.Entity;

namespace WebConnections.Core
{
    public class GZipWebClient : WebClient
    {
        [SecuritySafeCritical]
        public GZipWebClient()
        {
        }

        public string Result { get; private set; }

        protected override WebRequest GetWebRequest(Uri address)
        {
            var req = base.GetWebRequest(address);
            req.Headers[HttpRequestHeader.AcceptEncoding] = "gzip";
            return req;
        }

        protected override WebResponse GetWebResponse(WebRequest request, IAsyncResult result)
        {
            WebResponse response;
            try
            {
                response = base.GetWebResponse(request, result);
                if (response.Headers["Content-Encoding"] != null
                    && response.Headers["Content-Encoding"].ToString().Equals("gzip", StringComparison.InvariantCultureIgnoreCase))
                {
                    int count;
                    StringBuilder sb = new StringBuilder();
                    byte[] buf = new byte[8192];
                    GZipStream resStream = new GZipStream(response.GetResponseStream(), CompressionMode.Decompress);
                    do
                    {
                        count = resStream.Read(buf, 0, buf.Length);
                        if (count != 0)
                            sb.Append(Encoding.UTF8.GetString(buf, 0, count));
                    } while (count > 0);
                    Result = sb.ToString();
                }
                return response;
            }
            catch
            {
                return null;
            }
        }
    }

    public abstract class BaseAPI : BaseConnection
    {
        public event DownloadStringCompletedEvent DownloadStringCompleted;

        public DownloadProgressChanged DownloadProgressChanged;
        public GetEntityHandler GetEntity;
        public GetEntityAPIHandler GetEntityAPI;

        public bool IsRunning { get; private set; }

        public abstract string urlAPI
        {
            get;
        }

        public abstract Type BaseTypeEntity
        {
            get;
        }

        private string _jsonReturn;
        public string JsonReturn
        {
            get { return _jsonReturn; }
        }

        public BaseEntity DataContract { get; private set; }

        public bool BaseTypeEntityValid { get { return (DataContract.GetType() == BaseTypeEntity); } }

        public override string Params
        {
            get
            {
                return string.Empty;
            }
        }

        public override string URLBase
        {
            get { return this.urlAPI; }
        }

        public virtual string GetData()
        {
            IsRunning = true;
            try
            {
                WebClient busca = new WebClient();
                
                IsRunning = true;
                string url = urlAPI;
                busca.DownloadProgressChanged += busca_DownloadProgressChanged;

                try
                {
                    _jsonReturn = busca.DownloadString(new Uri(url));

                    if (string.IsNullOrEmpty(_jsonReturn))
                        return _jsonReturn;

                    var ed = new MyDownloadStringCompletedEventArgs(null
                        , false
                        , null
                        , _jsonReturn);

                    var events = new EventDownloadStringCompleted(this, ed);

                    if (DownloadStringCompleted != null)
                        DownloadStringCompleted(this, events);

                    if (!string.IsNullOrEmpty(events.ResultChanged))
                    {
                        _jsonReturn = events.ResultChanged;
                        if (events.cancel)
                            return _jsonReturn;
                    }
                    else
                        _jsonReturn = ed.Result;

                    MethodInfo method = base.GetType().GetMethod("Retorno").MakeGenericMethod(new Type[] { BaseTypeEntity });
                    DataContract = (BaseEntity)method.Invoke(this, new object[] { _jsonReturn });
                }
                catch (WebException ex)
                {
                    MessageError me = new MessageError();
                    me.errors.error.message = string.Concat("Servidor indisponível. ", ex.Message);
                    _jsonReturn = Utils.SerializerJson<MessageError>(me);
                    DataContract = me;
                }
                catch (Exception ex)
                {
                    MessageError me = new MessageError();
                    //me.errors.error.message = string.Concat("Erro no download das informações: ", ex.Message);
                    me.errors.error.message = string.Concat("Servidor indisponível. ", ex.Message);
                    _jsonReturn = Utils.SerializerJson<MessageError>(me);
                    DataContract = me;
                }

                if (GetEntity != null)
                    GetEntity(DataContract, _jsonReturn);

                if (GetEntityAPI != null)
                    GetEntityAPI(this, DataContract, _jsonReturn);

                return _jsonReturn;
            }
            finally
            {
                IsRunning = false;
            }
        }

        public virtual void DownloadJson()
        {
            GZipWebClient busca = new GZipWebClient();
            
            string url = urlAPI;
            busca.DownloadProgressChanged += busca_DownloadProgressChanged;
            busca.DownloadStringCompleted += new DownloadStringCompletedEventHandler(busca_DownloadStringCompleted);
            IsRunning = true;
            busca.DownloadStringAsync(new Uri(url));
        }

        void busca_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            if (DownloadProgressChanged != null)
                DownloadProgressChanged(sender, e);
        }

        void busca_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            IsRunning = false;

            try
            {
                if (sender == null)
                    throw new Exception("Retorno (sender) não definido");

                var wb = (GZipWebClient)sender;
                var ed = new MyDownloadStringCompletedEventArgs(e.Error
                    , e.Cancelled
                    , e.UserState
                    , string.IsNullOrEmpty(e.Result) ? wb.Result : e.Result);

                var events = new EventDownloadStringCompleted(sender, ed);

                if (DownloadStringCompleted != null)
                    DownloadStringCompleted(this, events);

                if (!string.IsNullOrEmpty(events.ResultChanged))
                {
                    _jsonReturn = events.ResultChanged;
                    if (events.cancel)
                        return;
                }
                else
                    _jsonReturn = ed.Result;

                MethodInfo method = base.GetType().GetMethod("Retorno").MakeGenericMethod(new Type[] { BaseTypeEntity });
                DataContract = (BaseEntity)method.Invoke(this, new object[] { _jsonReturn });
            }
            catch (WebException ex)
            {
                MessageError me = new MessageError();
                me.errors.error.message = string.Concat("Servidor indisponível. ", ex.Message);
                _jsonReturn = Utils.SerializerJson<MessageError>(me);
                DataContract = me;
            }
            catch (Exception ex)
            {
                MessageError me = new MessageError();
                //me.errors.error.message = string.Concat("Erro no download das informações: ", ex.Message);
                me.errors.error.message = string.Concat("Servidor indisponível. ", ex.Message);
                _jsonReturn = Utils.SerializerJson<MessageError>(me);
                DataContract = me;
            }

            if (GetEntity != null)
                GetEntity(DataContract, _jsonReturn);

            if (GetEntityAPI != null)
                GetEntityAPI(this, DataContract, _jsonReturn);
        }

        public virtual void ObterEntidade()
        {
            MethodInfo method = base.GetType().GetMethod("Download").MakeGenericMethod(new Type[] { BaseTypeEntity });
            method.Invoke(this, null);
        }

        public void Download<T>()
        {
            this.DownloadJson();
        }

        public T ReturnData<T>(string data)
        {
            return ReturnRequest<T>(data);
        }

        public override T ReturnRequest<T>(string data)
        {
            try
            {
                object objeto = Utils.DeserializerJson<T>(data);

                if (objeto != null && objeto is T)
                    return (T)objeto;
            }
            catch (Exception ex)
            {
                ResponseServerData = string.Concat(ex.Message);
                EventExceptionCalled(this, ex);
            }
            return (T)Activator.CreateInstance(BaseTypeEntity);
        }

        protected virtual void EventoDownloadCompleted(object sender, EventDownloadStringCompleted e)
        {
            if (DownloadStringCompleted != null)
                DownloadStringCompleted(this, e);
        }

    }
}
