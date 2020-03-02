using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using MSG_Server.Net;
using System.Threading;
using Fursion.Protocol;
using MethodPool;
using static MethodPool.HandConnPool;
using Newtonsoft.Json;

namespace MSG_Server
{
    /// <summary>
    /// 与主服务器的连接类
    /// </summary>
    class ToMainServerConn
    {
        public static ToMainServerConn A;
        public static int Vrsion = 1;
        private readonly string Server_Vrsion = "Tank_MSG_Server_" + Vrsion.ToString();
        public Dictionary<Fursion_Protocol, ProtocolMethod> Event = new Dictionary<Fursion_Protocol, ProtocolMethod>();
        public Dictionary<Fursion_Protocol, ProtocolMethod> OnceEvent = new Dictionary<Fursion_Protocol, ProtocolMethod>();
        public ToMainServerConn()
        {
            A = this;
        }
        private const int BUFFER_SIZE = 2048;
        public bool ToServerState = false;
        public Socket socket;
        private byte[] readbuff = new byte[BUFFER_SIZE];
        private int buffCount = 0;
        private Int32 msglenght = 0;
        private byte[] lenBytes = new byte[sizeof(Int32)];
        public ProtocolBase PB = new ProtocolBytes();
        public int Temp = 0;
        public void AddEvent(Fursion_Protocol FP, ProtocolMethod PM)
        {
            if (Event.ContainsKey(FP))
                return;
            Event.Add(FP, PM);
        }
        public void DelEvent(Fursion_Protocol FP)
        {
            if (Event.ContainsKey(FP))
                Event.Remove(FP);
        }
        public bool ToConnServer(string IP, int Prot)
        {
            try
            {
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPAddress IPAdd = IPAddress.Parse(IP);
                IPEndPoint IEP = new IPEndPoint(IPAdd, Prot);
                socket.Connect(IEP);
                ToServerState = false;
                socket.BeginReceive(readbuff, buffCount, BUFFER_SIZE - buffCount, SocketFlags.None, ReceiveCb, readbuff);
                ProtocolBytes bytes = new ProtocolBytes();
                bytes.AddData(Var.ServerInfo);
                bytes.AddData(Server_Vrsion);
                bytes.AddData(ClientMC.A.LoadIP);
                bytes.AddData(ClientMC.A.LoadPort);
                SendToMainServer(bytes);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }
        private void ReceiveCb(IAsyncResult ar)
        {
            try
            {
                int count = socket.EndReceive(ar);
                if (count <= 0)
                {
                    return;
                }
                buffCount += count;
                PrcessByte();
                Array.Copy(readbuff, lenBytes, sizeof(Int32));
                socket.BeginReceive(readbuff, buffCount, BUFFER_SIZE - buffCount, SocketFlags.None, ReceiveCb, readbuff);
            }
            catch (Exception e)
            {
                ToServerState = false;
                ReConnect();
                Console.WriteLine(e.Message);
            }
        }
        private void PrcessByte()
        {
            if (buffCount < sizeof(Int32))//接收到消息小于包头长度
                return;
            Array.Copy(readbuff, lenBytes, sizeof(Int32));
            msglenght = BitConverter.ToInt32(lenBytes, 0);
            if (buffCount < msglenght + sizeof(Int32))
                return;
            ProtocolBytes Pb = (ProtocolBytes)PB.Decode(readbuff, sizeof(Int32), msglenght);
            HandMSG(Pb);
            int count = buffCount - msglenght - sizeof(Int32);//计算剩余未处理消息的长度
            Array.Copy(readbuff, sizeof(Int32) + msglenght, readbuff, 0, count);
            buffCount = count;
            if (buffCount > 0)
            {
                PrcessByte();
            }
        }
        private void HandMSG(ProtocolBytes PB)
        {
            if (Event.ContainsKey(PB.Protocol))
                Event[PB.Protocol](PB);
        }
        public void Getserver()
        {
            ProtocolBytes bytes = new ProtocolBytes
            {
                Protocol = Fursion_Protocol.RetServer
            };
            bytes.AddData(ClientMC.A.Utilization());
            SendToMainServer(bytes);
        }
        public void SendToMainServer(ProtocolBytes PBS)
        {
            byte[] bytes = PBS.Encode();
            byte[] Lenght = BitConverter.GetBytes(bytes.Length);
            byte[] SendBuff = Lenght.Concat(bytes).ToArray();
            socket.BeginSend(SendBuff, 0, SendBuff.Length, SocketFlags.None, null, null);
        }
        public void SendToMainServer(Fursion.Protocol.Fursion_Protocol fursion_Protocol, object O)
        {
            ProtocolBytes protocol = new ProtocolBytes
            {
                Protocol = fursion_Protocol
            };
            protocol.AddData(JsonConvert.SerializeObject(O));
            SendToMainServer(protocol);
        }
        private void ReConnect()
        {
            Thread ReConn = new Thread(Program.ToS);
            ReConn.Start();
        }
    }
}
