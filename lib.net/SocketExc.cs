using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace lib.net
{
    public class SocketExc
    {
        #region 私有属性
        /// <summary>
        /// 最大缓存
        /// </summary>
        private int RecvMax = 8912;
        /// <summary>
        /// 远程IP地址
        /// </summary>
        private IPAddress Remoteip;
        #endregion

        #region 属性
        /// <summary>
        /// 远程IP
        /// </summary>
        public string IP { get { return Remoteip.ToString(); } set { Remoteip = IPAddress.Parse(value); } }
        /// <summary>
        /// 远程端口
        /// </summary>
        public int RemotePort { get; set; }
        /// <summary>
        /// 本地主机名称
        /// </summary>
        public string LocalHostName { get; private set; }
        /// <summary>
        /// 监听端口
        /// </summary>
        public int LocalPort { get; set; } = 8008;
        /// <summary>
        /// 通信Socket
        /// </summary>
        Socket Server;
        #endregion

       



        #region 构造、析构
        public SocketExc(ProtocolType protocol = ProtocolType.Tcp)
        {
            LocalHostName = Dns.GetHostName();
            Server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, protocol);
        }

        public SocketExc(string remoteIp, int remotePort, ProtocolType protocol = ProtocolType.Tcp)
        {
            Remoteip = IPAddress.Parse(remoteIp);
            RemotePort = remotePort;
            LocalHostName = Dns.GetHostName();
            Server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, protocol);
        }

        /// <summary>
        /// 创建一个绑定到本地IP的Socket
        /// </summary>
        /// <param name="port"></param>
        /// <param name="maxClient"></param>
        /// <param name="recvMax"></param>
        /// <param name="protocol"></param>
        public SocketExc(int port, int maxClient, int recvMax, ProtocolType protocol = ProtocolType.Tcp)
        {
            LocalHostName = Dns.GetHostName();
            LocalPort = port;
            MaxClient = maxClient;
            RecvMax = recvMax;
            Server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, protocol);
        }


        ~SocketExc()
        {
            Server = null;
        }
        #endregion


        static Socket ConnectSocket(IPAddress mip, int mport, ProtocolType protocol = ProtocolType.Tcp)
        {
            try
            {
                IPEndPoint endPoint = new IPEndPoint(mip, mport);
                Socket sk = new Socket(AddressFamily.InterNetwork, SocketType.Stream, protocol);
                sk.Connect(endPoint);
                if (sk.Connected) return sk;
                else throw new Exception("连接失败");
            }
            catch (Exception)
            {
                throw;
            }

        }

        static Socket ConnectSocket(string url, int mport, ProtocolType protocol = ProtocolType.Tcp)
        {
            try
            {
                foreach (IPAddress i in Dns.GetHostEntry(url).AddressList)
                {
                    IPEndPoint endPoint = new IPEndPoint(i, mport);
                    Socket sk = new Socket(AddressFamily.InterNetwork, SocketType.Stream, protocol);
                    sk.Connect(endPoint);
                    if (sk.Connected) return sk;
                }
                throw new Exception("连接失败");
            }
            catch (Exception)
            {
                throw;
            }

        }

        public void Connect()
        {
            IPEndPoint endPoint = new IPEndPoint(Remoteip, RemotePort);
            Server.Connect(endPoint);
            if (!Server.Connected) throw new Exception("连接失败");
        }
        public string Send(string request)
        {
            try
            {
                byte[] bs = Encoding.UTF8.GetBytes(request);
                byte[] Received = new byte[RecvMax];
                Server.Send(bs, bs.Length, SocketFlags.None);

                List<byte> bts = new List<byte>();
                while (true)
                {
                    int i = Server.Receive(Received, Received.Length, SocketFlags.None);
                    if (i > 0)
                    {
                        if (i == Received.Length) bts.AddRange(Received);
                        else
                        {
                            byte[] b = new byte[i];
                            Buffer.BlockCopy(Received, 0, b, 0, i);
                            bts.AddRange(b);
                        }
                    }
                    else break;
                }
                string str = Encoding.UTF8.GetString(bts.ToArray(), 0, bts.Count);
                return str;
            }
            catch (Exception)
            {
                throw;
            }
        }

        static public string SendReceive(string request, IPAddress Remoteip, int RemotePort, int RecvMax = 8912)
        {
            try
            {
                byte[] bs = Encoding.UTF8.GetBytes(request);
                byte[] Received = new byte[RecvMax];
                Socket s = ConnectSocket(Remoteip, RemotePort);
                s.Send(bs, bs.Length, SocketFlags.None);

                List<byte> bts = new List<byte>();
                while (true)
                {
                    int i = s.Receive(Received, Received.Length, SocketFlags.None);
                    if (i > 0)
                    {
                        if (i == Received.Length) bts.AddRange(Received);
                        else
                        {
                            byte[] b = new byte[i];
                            Buffer.BlockCopy(Received, 0, b, 0, i);
                            bts.AddRange(b);
                        }
                    }
                    else break;
                }
                string str = Encoding.UTF8.GetString(bts.ToArray(), 0, bts.Count);
                s.Close();
                return str;
            }
            catch (Exception)
            {
                throw;
            }
        }

        static public byte[] SendReceive(byte[] request, IPAddress Remoteip, int RemotePort, int RecvMax = 8912)
        {
            try
            {
                byte[] Received = new byte[RecvMax];
                Socket s = ConnectSocket(Remoteip, RemotePort);
                s.Send(request, request.Length, SocketFlags.None);

                List<byte> bts = new List<byte>();
                while (true)
                {
                    int i = s.Receive(Received, Received.Length, SocketFlags.None);
                    if (i > 0)
                    {
                        if (i == Received.Length) bts.AddRange(Received);
                        else
                        {
                            byte[] b = new byte[i];
                            Buffer.BlockCopy(Received, 0, b, 0, i);
                            bts.AddRange(b);
                        }
                    }
                    else break;
                }
                s.Close();
                return bts.ToArray();
            }
            catch (Exception)
            {
                throw;
            }
        }

        //------------服务端部分--------------

        public enum WorkStatus { None, ServerInitialing, ServerRuning, ClientRuning }
        public enum CommunicationProtocol { Tcp, Udp }

        /// <summary>
        /// 开始监听数据的委托
        /// </summary>
        public delegate void StartListenHandler();
        /// <summary>
        /// 开始监听数据的事件
        /// </summary>
        public event StartListenHandler StartListen;
        /// <summary>
        /// 接收到信息时的事件委托
        /// </summary>
        /// <param name="info"></param>
        public delegate void ReceiveMsgHandler(string uid, byte[] bs);
        /// <summary>
        /// 接收到信息时的事件
        /// </summary>
        public event ReceiveMsgHandler OnMsgReceived;
        /// <summary>
        /// 发送信息完成后的委托
        /// </summary>
        /// <param name="successorfalse"></param>
        public delegate void SendCompletedHandler(string uid, string msg);
        /// <summary>
        /// 发送信息完成后的事件
        /// </summary>
        public event SendCompletedHandler OnSended;

        public delegate void ClientConnectedHandler(string ip, int port);
        /// <summary>
        /// 客户端连接
        /// </summary>
        public ClientConnectedHandler ClientConnected;
        public delegate void ExceptionHandler(Exception ex);
        /// <summary>
        /// 异常报告
        /// </summary>
        public event ExceptionHandler SocketException;

        /// <summary>
        /// 服务同步锁
        /// </summary>
        private static Mutex mutex = new Mutex();
        /// <summary>
        /// 当前连接数
        /// </summary>
        public int ConnectionCount { get { return connCount; } }
        private int connCount;
        /// <summary>
        /// 最大并发量
        /// </summary>
        public int MaxClient { get; private set; } = 1000;
        /// <summary>
        /// 服务器状态
        /// </summary>
        public WorkStatus ServerState { get; private set; }

        /// <summary>
        /// 连接池
        /// </summary>
        private SocketPool Clients;
        /// <summary>
        /// 缓存池
        /// </summary>
        BufferManager SocketBuffer;
        /// <summary>
        /// 并发控制信号量
        /// </summary>
        private Semaphore SemaphoreAcceptedClients;
        /// <summary>
        /// 通信协议
        /// </summary>
        private CommunicationProtocol CommProtocol;
        

        public bool IsRuning { get; private set; }
        /// <summary>
        /// 获取在线用户
        /// </summary>
        /// <returns></returns>
        public string[] OnLineUID() { return Clients.getOnLineList(); }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="maxConn"></param>
        /// <param name="receiveBufferSize"></param>
        /// <param name="getIp"></param>
        public void setListener(int maxConn, int receiveBufferSize, ClientConnectedHandler getIp)
        {
            ServerState = WorkStatus.ServerInitialing;
            connCount = 0;
            MaxClient = maxConn;
            Clients = new SocketPool();
            SemaphoreAcceptedClients = new Semaphore(ConnectionCount, MaxClient);
            SocketBuffer = new BufferManager(receiveBufferSize * maxConn, receiveBufferSize);
            CommProtocol = new CommunicationProtocol();
            ClientConnected = getIp;
        }
        public void InitServer()
        {
            for (int i = 0; i < MaxClient; i++)
            {
                SocketAsyncEventArgs arg = new SocketAsyncEventArgs();
                arg.Completed += new EventHandler<SocketAsyncEventArgs>(IO_Completed);
                arg.UserToken = new SocketElement();
                SocketBuffer.SetBuffer(arg);
                Clients.Push(arg);
            }
        }

        public void RunServer()
        {
            IsRuning = true;
            try
            {
                Server.Bind(new IPEndPoint(IPAddress.Any, LocalPort));
                Server.Listen(MaxClient);
                LingerOption lo = new LingerOption(true, 1);
                Server.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.AcceptConnection, lo);

                StartAccept(null);

                //ListenThread = new Thread(new ThreadStart(ListenClient));
                //ListenThread.Start();

                ServerState = WorkStatus.ServerRuning;
                mutex.WaitOne();
            }
            catch (Exception)
            {
                if (Server != null && Server.Connected) Server.Close();
                IsRuning = false;
                throw;
            }
        }
        public void CloseServer()
        {
            if (Server != null) Server.Close();
            Server = null;
            mutex.ReleaseMutex();
            ServerState = WorkStatus.None;
        }

        

        /// <summary>
        /// 开始监听线程的入口函数
        /// </summary>
        public void Listen()
        {
            while (true)
            {
                string[] keys = Clients.getOnLineList();
                foreach (string uid in keys)
                {
                    SocketAsyncEventArgs arg = Clients.getSocketAsyncEventArgs(uid);
                    if (arg != null && arg.LastOperation != SocketAsyncOperation.Receive)
                    {
                        bool willRaiseEvent = (arg.UserToken as SocketElement).Socket.ReceiveAsync(arg);
                        if (!willRaiseEvent) ProcessReceive(arg);
                    }
                }
            }
        }
        /// <summary>
        /// 发送信息
        /// </summary>
        /// <param name="uid">要发送的用户的uid</param>
        /// <param name="msg">消息体</param>
        public void Send(string uid, byte[] bs)
        {
            if (uid == string.Empty || uid == "" || bs == null || bs.Length < 1) return;
            using (SocketAsyncEventArgs arg = new SocketAsyncEventArgs())
            {
                arg.AcceptSocket = Clients.getSocketAsyncEventArgs(uid).AcceptSocket;
                if (arg == null)
                    OnSended(uid, "用户不在线");
                else
                {
                    if (arg.SocketError == SocketError.Success)
                    {
                        int i = 0;
                        try
                        {
                            //写入缓存
                            arg.SetBuffer(bs, 0, bs.Length);
                            ProcessSend(arg);
                        }
                        catch (Exception ex)
                        {
                            if (i <= 5)
                            {
                                i++;
                                //如果发送出现异常就延迟0.01秒再发
                                Thread.Sleep(10);
                                Send(uid, bs);
                            }
                            else
                            {
                                OnSended(uid, ex.ToString());
                            }
                        }
                    }
                    else
                    {
                        OnSended(uid, "200");
                        CloseClientSocket(arg);
                    }
                }
            }
              
        }

        /// <summary>
        /// 接受请求
        /// </summary>
        /// <param name="arg"></param>
        private void StartAccept(SocketAsyncEventArgs arg)
        {
            if (arg == null)
            {
                arg = new SocketAsyncEventArgs();
                arg.Completed += new EventHandler<SocketAsyncEventArgs>(OnAcceptCompleted);
            }
            else arg.AcceptSocket = null;
            SemaphoreAcceptedClients.WaitOne();
            bool willRaiseEvent = Server.AcceptAsync(arg);
            if (!willRaiseEvent) ProcessAccept(arg);
        }
        private void OnAcceptCompleted(object sender, SocketAsyncEventArgs e)
        {
            ProcessAccept(e);
        }
       public class UserTokenEventArgs
        {
            SocketElement User;
            public UserTokenEventArgs(SocketElement u)
            {
                User = u;
            }
        }
        public delegate void AcceptHandler(object sender, SocketElement e);
        public event AcceptHandler SetUserToken;

        private void ProcessAccept(SocketAsyncEventArgs e)
        {
            if (e.LastOperation != SocketAsyncOperation.Accept)  return;
            if (e.BytesTransferred <= 0) return;
            //不允许同Ip多个连接
            if (Clients.CheckIpOnLine((e.AcceptSocket.RemoteEndPoint as IPEndPoint).Address.ToString())) return;
            SocketElement se = new SocketElement("", e.AcceptSocket);
            SetUserToken?.Invoke(null, se);//
            if (Clients.ContainsUID(se.UID))
            {
                SocketException?.Invoke(new Exception("用户id已存在！"));
                return;
            }
            SocketAsyncEventArgs arg = Clients.Pop(se);
            ((SocketElement)arg.UserToken).Socket = e.AcceptSocket;
            Interlocked.Increment(ref connCount);

            bool willRaiseEvent = e.AcceptSocket.ReceiveAsync(arg);
            if (!willRaiseEvent) ProcessReceive(arg);

            StartAccept(e);
        }

        void IO_Completed(object sender, SocketAsyncEventArgs e)
        {
            switch (e.LastOperation)
            {
                case SocketAsyncOperation.Receive:
                    ProcessReceive(e);
                    break;
                case SocketAsyncOperation.Send:
                    ProcessSend(e);
                    break;
                default:
                    throw new ArgumentException("未知的处理请求");
            }

        }


        /// <summary>
        /// 处理接收
        /// </summary>
        /// <param name="e"></param>
        private void ProcessReceive(SocketAsyncEventArgs e)
        {
            if (e.BytesTransferred > 0 && e.SocketError == SocketError.Success)
            {
                //传递消息
                OnMsgReceived(((SocketElement)e.UserToken).UID, BufferManager.newBytes(e.Buffer, e.Offset, e.BytesTransferred));
                
                //发送一个异步接受请求，并获取请求是否为成功
                bool willRaiseEvent = (e.UserToken as Socket).ReceiveAsync(e);
                if (!willRaiseEvent) ProcessReceive(e);
            }
            else
                CloseClientSocket(e);
        }
        /// <summary>
        /// 处理发送
        /// </summary>
        /// <param name="e"></param>
        private void ProcessSend(SocketAsyncEventArgs e)
        {
            if (e.SocketError == SocketError.Success)
            {
                bool willRaiseEvent = (e.UserToken as Socket).ReceiveAsync(e);
                if (!willRaiseEvent)
                {
                    ProcessReceive(e);
                }

                OnSended(((SocketElement)e.UserToken).UID, "100");
            }
            else CloseClientSocket(e);
        }


        private void CloseClientSocket(SocketAsyncEventArgs e)
        {
            SocketElement se = e.UserToken as SocketElement;
            try
            {
                se.Socket.Shutdown(SocketShutdown.Send);
            }
            catch (Exception) { }
            Interlocked.Decrement(ref connCount);
            SemaphoreAcceptedClients.Release();
            Clients.Push(e);
        }


    

        /// <summary>
        /// 连接单元
        /// </summary>
        public class SocketElement : IDisposable
        {
            public bool state = false;
            public string UID { get; set; } = "";
            public Socket Socket;

            public SocketElement()
            {
            }
            public SocketElement(string uid, Socket socket)
            {
                UID = uid;
            }

            public void Dispose() {
            }

            public void setSocket(string uid, Socket socket)
            {
                UID = uid;
            }
        }

        /// <summary>
        /// 连接池
        /// </summary>
        public class SocketPool : IDisposable
        {
            /// <summary>
            /// 在线字典
            /// </summary>
            private Dictionary<string, SocketAsyncEventArgs> busyPool;
            /// <summary>
            /// 空闲栈
            /// </summary>
            private Stack<SocketAsyncEventArgs> freePool;

            public SocketPool()
            {
                busyPool = new Dictionary<string, SocketAsyncEventArgs>();
                freePool = new Stack<SocketAsyncEventArgs>();
            }

            public void Dispose()
            {
                busyPool = null;
                freePool = null;
            }

            public SocketAsyncEventArgs Pop(SocketElement UserToken)
            {
                SocketAsyncEventArgs arg;
                lock (freePool)
                {
                    arg = freePool.Pop();
                }
                arg.UserToken = UserToken;
                lock (busyPool)
                {
                    busyPool.Add(UserToken.UID, arg);
                }
                return arg;
            }

            public void Push(SocketAsyncEventArgs arg)
            {
                SocketElement se = (SocketElement)arg.UserToken;
                lock (busyPool)
                {
                    if (busyPool.ContainsKey(se.UID)) busyPool.Remove(se.UID);
                }
                lock (freePool)
                {
                    freePool.Push(arg);
                }
            }
            public bool ContainsUID(string uid)
            {
                lock (busyPool)
                {
                    if (busyPool.ContainsKey(uid)) return true;
                }
                return false;
            }

            public bool CheckIpOnLine(string ip)
            {
                lock (busyPool)
                {
                    foreach(SocketAsyncEventArgs i in busyPool.Values)
                    {
                        if (((IPEndPoint)i.RemoteEndPoint).Address.ToString() == ip) return true;
                    }
                }
                return false;
            }


            public SocketAsyncEventArgs getSocketAsyncEventArgs(string uid)
            {
                lock (busyPool)
                {
                    if (busyPool.ContainsKey(uid)) return busyPool[uid];
                }
                return null;
            }


            public string[] getOnLineList()
            {
                lock (busyPool)
                {
                    return busyPool.Keys.ToArray();
                }
            }

        }

        /// <summary>
        /// 缓存池
        /// </summary>
        public sealed class BufferManager : IDisposable
        {
            /// <summary>
            /// 缓存池
            /// </summary>
            private byte[] buffer;
            /// <summary>
            /// 单元大小
            /// </summary>
            private int blockSize;
            /// <summary>
            /// 总大小
            /// </summary>
            private int bufferSize;
            /// <summary>
            /// 当前可用索引
            /// </summary>
            private int Index;
            /// <summary>
            /// 闲置栈
            /// </summary>
            private Stack<int> freeIndexPool;

            /// <summary>
            /// 
            /// </summary>
            /// <param name="BufferSize">总大小</param>
            /// <param name="BlockSize">单元大小</param>
            public BufferManager(int BufferSize, int BlockSize)
            {
                if (BufferSize < 1 || BlockSize < 1) throw new Exception();
                bufferSize = BufferSize;
                blockSize = BlockSize;
                Index = 0;
                buffer = new byte[bufferSize];
                freeIndexPool = new Stack<int>();
            }

            /// <summary>
            /// 分配缓存
            /// </summary>
            /// <param name="args"></param>
            /// <returns></returns>
            public bool SetBuffer(SocketAsyncEventArgs args)
            {
                if (freeIndexPool.Count > 0)
                {
                    args.SetBuffer(buffer, freeIndexPool.Pop(), blockSize);
                }
                else
                {
                    if ((bufferSize - Index) < blockSize)
                    {
                        return false;
                    }
                    args.SetBuffer(buffer, Index, blockSize);
                    Index += blockSize;
                }
                return true;
            }

            /// <summary>
            /// 释放缓存
            /// </summary>
            /// <param name="e"></param>
            public void FreeBuffer(SocketAsyncEventArgs e)
            {
                freeIndexPool.Push(e.Offset);
                for (int i = e.Offset; i < e.Offset + blockSize; i++)
                {
                    if (buffer[i] == 0) break;
                    buffer[i] = 0;
                }
                e.SetBuffer(null, 0, 0);
            }

            /// <summary>
            /// 复制缓存
            /// </summary>
            /// <param name="buff"></param>
            /// <param name="offset"></param>
            /// <param name="lenght"></param>
            /// <returns></returns>
            public static byte[] newBytes(byte[] buff,int offset,int lenght)
            {
                byte[] bs = new byte[lenght];
                Buffer.BlockCopy(buff, offset, bs, 0, lenght);
                return bs;
            }


            public void Dispose()
            {
                freeIndexPool = null;
                buffer = null;
            }


        }




    }
}
