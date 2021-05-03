using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace lib.http
{
    /// <summary>
    /// http服务端
    /// </summary>
    public class HttpServer
    {
        /// <summary>
        /// 线程消息委托
        /// </summary>
        /// <param name="text"></param>
        public delegate void ThreadMessageEventHandler(string text);
        /// <summary>
        /// 连接事件委托
        /// </summary>
        /// <param name="context"></param>
        public delegate void ConnectedEvent(HttpListenerContext context);
        /// <summary>
        /// 线程消息事件
        /// </summary>
        public event ThreadMessageEventHandler ThreadMsgEvent;
        /// <summary>
        /// app连接
        /// </summary>
        public event ConnectedEvent AppConnected;
        /// <summary>
        /// 图片上传连接
        /// </summary>
        public event ConnectedEvent ImgConnected;
        /// <summary>
        /// app服务器
        /// </summary>
        private HttpListener AppServer;
        /// <summary>
        /// 图片上传服务器
        /// </summary>
        private HttpListener ImgServer;
        /// <summary>
        /// 线程池
        /// </summary>
        private Semaphore Sem1 = new Semaphore(100, 1000);
        /// <summary>
        /// 线程池
        /// </summary>
        private Semaphore Sem2 = new Semaphore(10, 10);
        Thread App;
        Thread Img;
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="appurl"></param>
        /// <param name="imgurl"></param>
        public HttpServer(string appurl = "http://*:7060/request/", string imgurl = "http://*:7060/upload/")
        {
            AppServer = new HttpListener();
            AppServer.Prefixes.Add(appurl);
            ImgServer = new HttpListener();
            ImgServer.Prefixes.Add(imgurl);
        }
        /// <summary>
        /// 运行aap服务器
        /// </summary>
        /// <param name="app"></param>
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
        /// <summary>
        /// 关闭app服务器
        /// </summary>
        public void CloseApp()
        {
            App?.Abort();
            AppServer.Stop();
        }
        /// <summary>
        /// 运行图像上传服务器
        /// </summary>
        /// <param name="img"></param>
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
