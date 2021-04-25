using System;
using WebSocket4Net;
using SuperSocket.ClientEngine;

namespace lib.websocket
{
    public class WSClient : IDisposable
    {
        public delegate void DataReceivedEventHander(byte[] data);
        public delegate void MsgEventHander(string msg);

        WebSocket ws;
        /// <summary>
        /// 消息到达
        /// </summary>
        public MsgEventHander MessageReceived;
        /// <summary>
        /// 数据到达
        /// </summary>
        public DataReceivedEventHander DataReceived;
        /// <summary>
        /// UI信息事件,错误消息
        /// </summary>
        public MsgEventHander ToUI;
        /// <summary>
        /// 远程连接是否打开
        /// </summary>
        public bool IsOpen { get; private set; }
        /// <summary>
        /// 创建到远程连接并创建加密通道
        /// </summary>
        /// <param name="url"></param>
        public WSClient(string url)
        {
            
            ws = new WebSocket(url);
            ws.Opened += OnOpened;
            ws.MessageReceived += OnMessage;
            ws.DataReceived += OnData;
            ws.Closed += OnClose;
            ws.Error += OnError;

        }

        public void Dispose()
        {
            if (ws != null)
            {
                ws?.Close();
                ws?.Dispose();
                ws = null;
            }
        }

        public void Open()
        {
            if (ws.State == WebSocketState.Open) ws.Close();
            ws.Open();
        }

        private void OnOpened(object sender, EventArgs e)
        {
            IsOpen = true;
        }
        public void Close()
        {
            Dispose();
        }

        private void OnError(object sender, ErrorEventArgs e)
        {
            int i = 0;
            //Open();
        }

        private void OnClose(object sender, EventArgs e)
        {
            IsOpen = false;
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="data">数据</param>
        public void SendMessage(string data)
        {
            ws.Send(data);//发送
        }


        private void OnData(object sender, DataReceivedEventArgs e)
        {
            DataReceived?.Invoke(e.Data);
        }

        private void OnMessage(object sender, MessageReceivedEventArgs e)
        {
            MessageReceived?.Invoke(e.Message);
        }

    }
}
