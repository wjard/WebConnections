using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using WebConnections.Core.Common;

namespace WebConnections.Core
{
    public abstract class BaseConnection
    {
        [Description("Evento disparado antes de obter os dados da chamada http")]
        public event BeforeResponseEventHandler BeforeResponse;

        [Description("Evento disparado depois de obter os dados da chamada http")]
        public event AfterResponseEventHandler AfterResponse;

        [Description("Evento disparado para tratamento de exceção em threads")]
        public event ExceptionEventHandler ExceptionCalled;

        public ReturnConnection ReturnResponseServerData;

        public abstract string URLBase
        {
            get;
        }

        public virtual bool AllowAutoRedirect
        {
            get { return false; }
        }

        /*
        public virtual SecurityProtocolType TipoProtocoloSeguranca
        {
            get { return SecurityProtocolType.Tls; }
        }
        */

        public virtual Encoding Encoding
        {
            get
            {
                //return System.Text.Encoding.ASCII; 
                return Encoding.GetEncoding("ISO-8859-1");
            }
        }

        public virtual string Referer
        {
            get { return string.Empty; }
        }

        public virtual string RequestMethods
        {
            get { return "Post"; }
        }

        public virtual string ContentType
        {
            get { return string.Empty; }
        }

        /// <summary>
        /// <see cref="http://developer.nokia.com/community/wiki/User-Agent_headers_for_Nokia_devices"/>
        /// </summary>
        public virtual string UserAgent
        {
            get { return string.Empty; }
            //get { return "Mozilla/5.0 (compatible; MSIE 10.0; Windows Phone 8.0; Trident/6.0; IEMobile/10.0; ARM; Touch)"; }
        }

        public virtual string Accept
        {
            get { return "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8"; }
        }

        public virtual long ContentLength
        {
            get { return 0; }
        }

        public virtual string Params
        {
            get { return string.Empty; }
        }

        public virtual CookieContainer CookieContainer
        {
            get;
            set;
        }

        public abstract WebHeaderCollection Header
        {
            get;
        }

        public virtual int Timeout
        {
            get { return 100000; }
        }

        public virtual bool KeepAlive
        {
            get { return false; }
        }

        public virtual string Expect
        {
            get { return null; }
        }

        public virtual string Host
        {
            get { return null; }
        }
                
        public virtual WebHeaderCollection ResponseHeader
        {
            get;
            internal set;
        }

        public virtual CookieContainer ResponseCookieContainer
        {
            get;
            internal set;
        }

        public string ResponseUri
        {
            get;
            internal set;
        }

        public string ResponseServerData
        {
            get;
            internal set;
        }

        public virtual T ReturnRequest<T>(string dataReturn)
        {
            throw new NotImplementedException();
        }

        protected virtual void EventExceptionCalled(object sender, Exception e)
        {
            if (ExceptionCalled != null)
                ExceptionCalled(sender, e);
        }

        protected virtual void EventBeforeResponse(object sender, EventResponse e)
        {
            if (BeforeResponse != null)
                BeforeResponse(this, e);
        }

        protected virtual void EventAfterResponse(object sender, EventResponse e)
        {
            if (AfterResponse != null)
                AfterResponse(this, e);
        }
    }
}
