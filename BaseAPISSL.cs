using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using WebConnections.Core.Common;

namespace WebConnections.Core
{
    public abstract class BaseAPISSL : BaseWebConnection
    {
        public override string URLBase
        {
            get { throw new NotImplementedException("Informe a URL Base"); }
        }

        public BaseAPISSL(WebHeaderCollection responseHeader, CookieContainer responseCookieContainer)
        {
            ConfigSession(responseHeader, responseCookieContainer);
        }

        public override WebHeaderCollection Header
        {
            get
            {
                if (_header == null)
                {
                    _header = new WebHeaderCollection();
                    _header[HttpRequestHeader.AcceptEncoding] = "gzip";
                }
                return _header;
            }
        } WebHeaderCollection _header;

        public abstract Type BaseTypeEntity
        {
            get;
        }

        private void ConfigSession(WebHeaderCollection responseHeader, CookieContainer responseCookieContainer)
        {
            string[] strHeaders = responseHeader.AllKeys;
            foreach (string h in strHeaders)
                if (h == "Set-Cookie" || h == "Cookie")
                {
                    Header["Cookie"] = responseHeader[h];
                    Header[h] = responseHeader[h];
                }

            this.CookieContainer = responseCookieContainer;
        }

        public override string RequestMethods
        {
            get
            {
                return "Get";
            }
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
    }
}
