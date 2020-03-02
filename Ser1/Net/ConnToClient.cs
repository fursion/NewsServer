using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using Fursion.Protocol;
using Newtonsoft.Json;

namespace MSG_Server.Net
{
    /// <summary>
    /// 与客户端部的连接类
    /// </summary>
    public class ConnToClient
    {
        public string Openid;
        public Socket socket;
        public bool IsUse = false;
        public int BufferCount = 0;
        public int BUFFER_SIZE = 2048;
        public byte[] ReadBuffer;
        public byte[] Lenbyte = new byte[sizeof(Int32)];
        public Int32 MsgLen = 0;
        public long LastHearBeatTime = long.MinValue;
        public void Init(Socket SK)
        {
            socket = SK;
            IsUse = true;
            ReadBuffer = new byte[BUFFER_SIZE];
            LastHearBeatTime = Sys.GetListTime();
        }
        public int RetCount()
        {
            return BUFFER_SIZE - BufferCount;
        }
        public void Close()
        {
            ClientMC.A.ClientOnlineNumber--;
            if (socket == null)
                return;
            if (!IsUse)
                return;
            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
            IsUse = false;
            if (ClientMC.A.CTCD.ContainsKey(Openid))
            {
                ClientMC.A.CTCD.Remove(Openid);
            }
        }
        public string GetAdressIP()
        {
            if (!IsUse)
                return "连接不可用";
            return socket.RemoteEndPoint.ToString();
        }
        public void Send(ProtocolBase protoco)
        {
            byte[] bytes = protoco.Encode();
            byte[] lenght = BitConverter.GetBytes(bytes.Length);
            byte[] sendbuff = lenght.Concat(bytes).ToArray();
            try
            {
                socket.BeginSend(sendbuff, 0, sendbuff.Length, SocketFlags.None, null, null);
            }
            catch (Exception e)
            {
                Console.WriteLine("[发送消息] 错误:" + e.Message);
            }
        }
        public void Send(Fursion.Protocol.Fursion_Protocol fursion_Protocol, object O)
        {
            ProtocolBytes protocol = new ProtocolBytes
            {
                Protocol = fursion_Protocol
            };
            protocol.AddData(JsonConvert.SerializeObject(O));
            Send(protocol);
        }
    }
}
