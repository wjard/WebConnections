using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;

namespace WebConnections.Core.Common
{
    public class Event : EventArgs
    {
    }

    /// <summary>
    /// Eventos que ocorrem antes e depois de um response
    /// </summary>
    public class EventResponse : Event
    {
        /// <summary>
        /// Request obtido
        /// </summary>
        public HttpWebRequest ResultRequest;

        /// <summary>
        /// Request obtido
        /// </summary>
        public IAsyncResult AsyncResult;

        /// <summary>
        /// Response obtido
        /// </summary>
        public HttpWebResponse Response;

        /// <summary>
        /// Registro corrente
        /// </summary>
        public string ResponseServerData;

        /// <summary>
        /// Header de retorno do response
        /// </summary>
        public WebHeaderCollection ResponseHeader;

        /// <summary>
        /// Cookies de retorno do response
        /// </summary>
        public CookieContainer ResponseCookieContainer;

        /// <summary>
        /// Interromper execução da chamada
        /// </summary>
        public bool Cancel { get; set; }

        /// <summary>
        /// Construtor da classe
        /// </summary>
        public EventResponse(HttpWebRequest resultRequest, IAsyncResult result)
        {
            ResultRequest = resultRequest;
            AsyncResult = result;
        }

        /// <summary>
        /// Construtor da classe
        /// </summary>
        public EventResponse(HttpWebRequest resultRequest)
        {
            ResultRequest = resultRequest;
        }

        /// <summary>
        /// Construtor da classe
        /// </summary>
        public EventResponse(HttpWebRequest resultRequest, HttpWebResponse response, string responseServerData, WebHeaderCollection responseHeader, CookieContainer responseCookieContainer)
        {
            ResultRequest = resultRequest;
            Response = response;
            ResponseServerData = responseServerData;
            ResponseHeader = responseHeader;
            ResponseCookieContainer = responseCookieContainer;
        }
    }

    // Summary:
    //     Provides data for the System.Net.WebClient.DownloadStringCompleted event.
    public class MyDownloadStringCompletedEventArgs : AsyncCompletedEventArgs
    {
        public MyDownloadStringCompletedEventArgs(Exception error, bool cancelled, object userState, string result)
            : base(error, cancelled, userState)
        {
            Result = result;
        }

        public string Result { get; set; }
    }

    /// <summary>
    /// Eventos que ocorrem depois de obter o retorno de uma chamada webclient
    /// </summary>
    public class EventDownloadStringCompleted : Event
    {
        public bool cancel = false;
        public object sender;
        public MyDownloadStringCompletedEventArgs e;
        public string ResultChanged;

        /// <summary>
        /// Construtor da classe
        /// </summary>
        public EventDownloadStringCompleted(object sender, MyDownloadStringCompletedEventArgs e)
        {
            this.sender = sender;
            this.e = e;
        }
    }
}
