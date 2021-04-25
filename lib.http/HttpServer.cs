using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace lib.http
{
    public class HttpServer
    {
        public delegate void ThreadMessageEventHandler(string text);
        public delegate void ConnectedEvent(HttpListenerContext context);
        public event ThreadMessageEventHandler ThreadMsgEvent;
        public event ConnectedEvent AppConnected;
        public event ConnectedEvent ImgConnected;
        private HttpListener AppServer;
        private HttpListener ImgServer;
        private Semaphore Sem1 = new Semaphore(100, 1000);
        private Semaphore Sem2 = new Semaphore(10, 10);
        Thread App;
        Thread Img;
        public HttpServer(string appurl = "http://*:7060/request/", string imgurl = "http://*:7060/upload/")
        {
            AppServer = new HttpListener();
            AppServer.Prefixes.Add(appurl);
            ImgServer = new HttpListener();
            ImgServer.Prefixes.Add(imgurl);
        }

        public void StartApp(ConnectedEvent app)
        {
            AppServer.Start();
            App = new Thread(() => {
                while (true)
                {
                    try
                    {
                        var v = AppServer.GetContext();
                        app?.Invoke(v);
                    }
                    catch (Exception ex)
                    {
                        ThreadMsgEvent?.BeginInvoke(ex.StackTrace, null, null);
                    }
                }
            });
            App.Start();
        }
        public void CloseApp()
        {
            App?.Abort();
            AppServer.Stop();
        }

        public void StartImg(ConnectedEvent img)
        {
            ImgServer.Start();
            Img = new Thread(() => {
                while (true)
                {
                    Sem2.WaitOne();
                    try
                    {
                        var v = ImgServer.GetContext();
                        img?.Invoke(v);
                    }
                    catch (Exception ex)
                    {
                        ThreadMsgEvent?.BeginInvoke(ex.StackTrace, null, null);
                    }
                    Sem2.Release();
                }
            });
            Img.Start();
        }
        public void CloseImg()
        {
            Img?.Abort();
            ImgServer.Stop();
        }




    }
}
