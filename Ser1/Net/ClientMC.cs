using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using MethodPool;
using Fursion.Protocol;
using System.Reflection;

namespace MSG_Server.Net
{
    class ClientMC
    {
        public string LoadIP;
        public int LoadPort;
        public static ClientMC A;
        public Socket socket;//监听连接的socket；
        public ConnToClient[] SCTCS;
        public Dictionary<string, ConnToClient> CTCD;
        public int ConnClientMaxNumber = 50000;
        private ProtocolBase PB;
        public int ClientOnlineNumber = 0;
        private long HearbeatTime = 300;
        public ProtocolBytes ToMainServe = new ProtocolBytes();
        private HandConnPool HCP;
        public ClientMC()
        {
            A = this;
        }
        public void Start(string IP, int Prot)
        {
            HCP = new HandConnPool();
            LoadIP = IP;
            LoadPort = Prot;
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress IPAdd = IPAddress.Parse(IP);
            IPEndPoint IPEP = new IPEndPoint(IPAdd, Prot);
            SCTCS = new ConnToClient[ConnClientMaxNumber];
            CTCD = new Dictionary<string, ConnToClient>();
            PB = new ProtocolBytes();
            socket.Bind(IPEP);
            socket.Listen(ConnClientMaxNumber);
            socket.BeginAccept(AcceptCb, null);
            Console.WriteLine("ClientMC启动 ：" + IPEP.ToString());
        }
        private int NewIndex()
        {
            for (int i = 0; i < ConnClientMaxNumber; i++)
            {
                if (SCTCS[i] == null)
                    return i;
                if (SCTCS[i].socket == null)
                    return i;
                if (SCTCS[i].IsUse == false)
                    return i;
            }
            return -1;
        }
        private void AcceptCb(IAsyncResult ar)
        {
            Socket RetCSocket = socket.EndAccept(ar);
            int Index = NewIndex();
            SCTCS[Index] = new ConnToClient();
            ConnToClient CTC = SCTCS[Index];
            ClientOnlineNumber++;
            CTC.Init(RetCSocket);
            CTC.socket.BeginReceive(CTC.ReadBuffer, CTC.BufferCount, CTC.RetCount(), SocketFlags.None, ReceiveCb, CTC);
            socket.BeginAccept(AcceptCb, null);
        }

        private void ReceiveCb(IAsyncResult ar)
        {
            ConnToClient CTC = (ConnToClient)ar.AsyncState;
            try
            {
                if (CTC.IsUse == false)
                    return;
                int count = CTC.socket.EndReceive(ar);
                if (count <= 0)
                {
                    CTC.Close();
                    return;
                }
                CTC.BufferCount += count;
                PrcessByte(CTC);
                CTC.socket.BeginReceive(CTC.ReadBuffer, CTC.BufferCount, CTC.RetCount(), SocketFlags.None, ReceiveCb, CTC);
            }
            catch (Exception e)
            {
                CTC.Close();
                Console.WriteLine(e.Message);
            }
        }
        private void PrcessByte(ConnToClient CTC)
        {
            if (CTC.BufferCount < sizeof(Int32))//接收到消息小于包头长度
                return;
            Array.Copy(CTC.ReadBuffer, CTC.Lenbyte, sizeof(Int32));
            CTC.MsgLen = BitConverter.ToInt32(CTC.Lenbyte, 0);
            if (CTC.BufferCount < CTC.MsgLen + sizeof(Int32))
                return;
            ProtocolBytes Pb = (ProtocolBytes)PB.Decode(CTC.ReadBuffer, sizeof(Int32), CTC.MsgLen);
            object[] vs = new object[] { CTC, Pb };
            object O = vs;
            Thread HandMSGT = new Thread(new ParameterizedThreadStart(HandMSG));
            HandMSGT.Start(O);
            int count = CTC.BufferCount - CTC.MsgLen - sizeof(Int32);
            Array.Copy(CTC.ReadBuffer, sizeof(Int32) + CTC.MsgLen, CTC.ReadBuffer, 0, count);
            CTC.BufferCount = count;
            if (CTC.BufferCount > 0)
                PrcessByte(CTC);
        }
        /// <summary>
        /// 事件分发函数
        /// </summary>
        /// <param name="O"></param>
        private void HandMSG(object O)
        {
            object[] vs = (object[])O;
            ConnToClient CTC = (ConnToClient)vs[0]; ProtocolBytes PB = (ProtocolBytes)vs[1];
            try
            {
                object[] OB = PB.GetDecode();
                string ProtoName = PB.Protocol.ToString();
                string MethodName = ProtoName;       
                try
                {
                    MethodInfo MI = HCP.GetType().GetMethod(MethodName);
                    object[] OS = new object[] { CTC, OB };
                    MI.Invoke(HCP, OS);
                }
                catch (Exception e)
                {
                    Console.WriteLine("132"+e.Message+MethodName);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message + " HandMSG");
            }

        }
        public float Utilization()
        {
            float UT = (float)ClientOnlineNumber / ConnClientMaxNumber;//返回利用率
            return UT;
        }
        public void printinfo()
        {
            Console.WriteLine("==============SCS==============");
            foreach (string openid in CTCD.Keys)
            {
                Console.WriteLine(openid+"  "+CTCD[openid].GetAdressIP());
            }
            //for (int i = 0; i < SCTCS.Length; i++)
            //{
            //    if (SCTCS[i] != null)
            //        Console.WriteLine(SCTCS[i].GetAdressIP());
            //}
            Console.WriteLine("===============================");
        }
        public void Send(ConnToClient CTC, ProtocolBytes bytes)
        {
            CTC.Send(bytes);
        }
        public void Hearbeat()
        {
            long TimeNow = Sys.GetListTime();
            for (int i = 0; i < SCTCS.Length; i++)
            {
                if (SCTCS[i] == null)
                    continue;
                ConnToClient CTC = SCTCS[i];
                if (!CTC.IsUse)
                    continue;
                if (CTC.LastHearBeatTime < TimeNow - HearbeatTime)
                {
                    lock (CTC)
                        CTC.Close();
                }
            }
        }
        public ConnToClient CheckCTC(ConnToClient CTC)
        {
            for(int i = 0; i < SCTCS.Length; i++)
            {
                if (SCTCS == null)
                    continue;
                if (SCTCS[i] == null)
                    continue;
                if (!SCTCS[i].IsUse)
                    continue;
                if (SCTCS[i] == CTC)
                    return SCTCS[i];
            }
            return null;
        }
    }
    class Sys
    {
        public static long GetListTime()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds);
        }
    }

}

