using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using WebConnections.Core.Common;

namespace WebConnections.Core
{
    public abstract class BaseWebConnection : BaseConnection
    {
        private byte[] _postData;

        public virtual void OpenConnection()
        {
            var request = Prepare();

            if (RequestMethods.Equals("Post", StringComparison.InvariantCultureIgnoreCase))
                request.BeginGetRequestStream(GetRequestStreamCallback, request);
            else
                request.BeginGetResponse(RetornoResponse, request);
        }

        public virtual string GetData()
        {
            var request = Prepare();

            if (RequestMethods.Equals("Post", StringComparison.InvariantCultureIgnoreCase))
            {
                using (var strm = request.GetRequestStream())
                {
                    strm.Write(_postData, 0, _postData.Length);
                    strm.Flush();
                }
            }

            var beforeEvent = new EventResponse(request, null);
            EventBeforeResponse(this, beforeEvent);

            if (!beforeEvent.Cancel)
            {
                if (request != null)
                {
                    var response = request.GetResponse();
                    if (response != null && response is HttpWebResponse)
                        PrepareResponse((HttpWebResponse)response, request);
                }
                return ResponseServerData;
            }
            return string.Empty;
        }

        public HttpWebRequest Prepare()
        {
            //var creator = WebRequestCreator.ClientHttp;
            //WebRequest.RegisterPrefix("http://", creator);
            //WebRequest.RegisterPrefix("https://", creator);

            if (ResponseCookieContainer == null)
                ResponseCookieContainer = new CookieContainer();

            if (ResponseHeader == null)
                ResponseHeader = new WebHeaderCollection();

            Uri.EscapeUriString(URLBase);

            var request = (HttpWebRequest)WebRequest.Create(URLBase);

            request.Headers[HttpRequestHeader.KeepAlive] = KeepAlive.ToString();
            if (!string.IsNullOrEmpty(Expect))
                request.Headers[HttpRequestHeader.Expect] = Expect;
            request.Method = RequestMethods;
            request.Accept = Accept;
            if (!string.IsNullOrEmpty(Referer))
                request.Headers[HttpRequestHeader.Referer] = Referer;
            request.ContentType = ContentType;
            request.UserAgent = UserAgent;
            request.CookieContainer = CookieContainer;
            request.Host = Host;            
            request.AllowAutoRedirect = AllowAutoRedirect;

            if (Header != null)
                if (request.Headers == null)
                    request.Headers = Header;
                else
                {
                    var strHeaders = Header.AllKeys;
                    foreach (var h in strHeaders)
                        request.Headers[h] = Header[h];
                }

            if (RequestMethods.Equals("Post", StringComparison.InvariantCultureIgnoreCase))
            {
                var parametros = Params;
                if (!string.IsNullOrEmpty(parametros))
                {
                    _postData = Encoding.UTF8.GetBytes(parametros);
                    request.ContentLength = _postData.Length;
                }
            }
            else
                _postData = Encoding.UTF8.GetBytes(Params);

            return request;
        }

        private void GetRequestStreamCallback(IAsyncResult result)
        {
            var req = (HttpWebRequest)result.AsyncState;

            if (req.Method.Equals("Post", StringComparison.InvariantCultureIgnoreCase))
            {
                using (var strm = req.EndGetRequestStream(result))
                {
                    strm.Write(_postData, 0, _postData.Length);
                    strm.Flush();
                }
            }
            req.BeginGetResponse(RetornoResponse, req);
        }

        private void RetornoResponse(IAsyncResult result)
        {
            try
            {
                var req = result.AsyncState as HttpWebRequest;

                var beforeEvent = new EventResponse(req, result);
                EventBeforeResponse(this, beforeEvent);

                if (beforeEvent.Cancel)
                    return;

                if (req != null)
                    using (var response = (HttpWebResponse)req.EndGetResponse(result))
                    {
                        PrepareResponse(response, req);
                    }

                if (ReturnResponseServerData != null)
                    ReturnResponseServerData(this, ResponseServerData);
            }
            catch (WebException webEx)
            {
                ResponseServerData = string.Concat("URL informada não foi encontrada ou está offline", Environment.NewLine, webEx.Message);
                EventExceptionCalled(this, webEx);
            }
            catch (Exception ex)
            {
                ResponseServerData = string.Concat("Erro ao obter informações", Environment.NewLine, ex.Message);
                EventExceptionCalled(this, ex);
            }
        }

        private void PrepareResponse(HttpWebResponse response, HttpWebRequest req)
        {
            ResponseServerData = string.Empty;

            if (response != null)
            {
                ResponseUri = response.ResponseUri.AbsoluteUri;

                ResponseCookieContainer = new CookieContainer();
                if (response.Cookies != null)
                {
                    foreach (Cookie cookie in response.Cookies)
                    {
                        var domain = cookie.Domain;
                        if (!domain.StartsWith("http"))
                            domain = string.Concat("http", domain);
                        ResponseCookieContainer.Add(new Uri(domain), cookie);
                    }
                }

                ResponseHeader = new WebHeaderCollection();
                var strHeaders = response.Headers.AllKeys;
                foreach (var h in strHeaders)
                    ResponseHeader[h] = response.Headers[h];

                if (ResponseHeader[HttpRequestHeader.ContentEncoding] != null &&
                    ResponseHeader[HttpRequestHeader.ContentEncoding].ToString().Equals("gzip", StringComparison.InvariantCultureIgnoreCase))
                {
                    try
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

                        ResponseServerData = sb.ToString();
                    }
                    catch
                    {
                        using (var stIn = new StreamReader(response.GetResponseStream(), Encoding))
                        {
                            if (stIn.Peek() > 0)
                                ResponseServerData = stIn.ReadToEnd();
                        }
                    }
                }
                else
                    using (var stIn = new StreamReader(response.GetResponseStream(), Encoding))
                    {
                        if (stIn.Peek() > 0)
                            ResponseServerData = stIn.ReadToEnd();
                    }
            }

            EventAfterResponse(this, new EventResponse(req, response, ResponseServerData, ResponseHeader, ResponseCookieContainer));
        }

        public override string URLBase
        {
            get { throw new NotImplementedException(); }
        }

        public override WebHeaderCollection Header
        {
            get { throw new NotImplementedException(); }
        }
    }
}
