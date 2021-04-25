using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Fleck;

namespace lib.websocket
{
    public class WebServer
    {
        public delegate void SocketStateHandler(IWebSocketConnection iw);
        public delegate void ReceiveEventHandler(string msg, IWebSocketConnection iw);
        public delegate void OnErrorEventHandler(Exception ex, IWebSocketConnection iw);
        public WebServer()
        {
            
        }

        public event OnErrorEventHandler OnError;
        public event SocketStateHandler OnClose;
        public event SocketStateHandler OnOpen;
        /// <summary>
        /// 全部连接
        /// </summary>
        List<IWebSocketConnection> allSockets;
        /// <summary>
        /// 服务器
        /// </summary>
        WebSocketServer server;

        public bool IsWorking { get; private set; } = false;
        public void Working(ReceiveEventHandler receive = null)
        {
            FleckLog.Level = LogLevel.Debug;
            allSockets = new List<IWebSocketConnection>();
            server = new WebSocketServer("ws://0.0.0.0:7066");
            server.RestartAfterListenError = true;
            server.Start(socket =>
            {
                socket.OnOpen = () =>
                {
                    allSockets.Add(socket);
                    OnOpen?.BeginInvoke(socket, null, null);
                };
                socket.OnClose = () =>
                {
                    allSockets.Remove(socket);
                    OnClose?.BeginInvoke(socket, null, null);//通知断开
                };
                socket.OnMessage = message =>
                {
                    receive?.BeginInvoke(message, socket, null, null);
                };
                socket.OnError = ex =>
                {
                    OnError?.BeginInvoke(ex, socket, null, null);
                };
            });

        }

        /// <summary>
        /// 发送推送消息
        /// </summary>
        /// <param name="message"></param>
        public void SendPush(string msg)
        {
            allSockets.ForEach(s => s.Send(msg));
        }

        public void Stop()
        {
            allSockets.ForEach(s => s.Close());
            server.Dispose();
        }

        public Dictionary<string, string> GetConns()
        {
            var vs = new Dictionary<string, string>();
            var ls = new List<IWebSocketConnection>(allSockets);
            foreach (var item in ls)
            {
                vs.Add(item.ConnectionInfo.Id.ToString("N"), item.ConnectionInfo.ClientIpAddress);
            }
            return vs;
        }


    }
}
