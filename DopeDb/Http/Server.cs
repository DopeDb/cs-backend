using System;
using System.Net;
using System.Threading;

namespace DopeDb.Http
{
    class Server
    {

        protected int port;

        protected readonly HttpListener listener = new HttpListener();

        protected Func<HttpListenerRequest, HttpListenerResponse, HttpListenerResponse> requestHandler;

        public Server(int port)
        {
            if (!HttpListener.IsSupported)
                throw new System.NotSupportedException("no httplistener");
            this.port = port;
            listener.Prefixes.Add($"http://localhost:{this.port}/");
            listener.Start();
        }

        public void SetRequestHandler(Func<HttpListenerRequest, HttpListenerResponse, HttpListenerResponse> callback)
        {
            this.requestHandler = callback;
        }

        public void Run()
        {
            if (this.requestHandler == null)
            {
                throw new System.ArgumentException("No request-handler specified for this server");
            }
            ThreadPool.QueueUserWorkItem(o =>
            {
                Console.WriteLine("Webserver listening on port " + this.port);
                // try {
                while (listener.IsListening)
                {
                    ThreadPool.QueueUserWorkItem(c =>
                    {
                        var ctx = c as HttpListenerContext;
                        // try {
                        var response = this.requestHandler(ctx.Request, ctx.Response);
                        // } catch { }
                        ctx.Response.OutputStream.Close();
                    }, listener.GetContext());
                }
                // }
                // catch { }
            });
        }

        public void Stop()
        {
            this.listener.Stop();
            this.listener.Close();
        }
    }
}