using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MSG_Server.Net;
using Newtonsoft.Json;
using Fursion.Protocol;
using NewsServer.Data;

namespace MethodPool
{
    class HandConnPool
    {
        public HandConnPool()
        {

        }
        public delegate void ProtocolMethod(object OB);
        public void Hearbeat(ConnToClient CTC, object[] OB)
        {

        }
        public void ConnectedNewsServer(ConnToClient CTC,object[] OB)
        {

        }

        public void JoinNewsRoom(ConnToClient CTC,object[] OB)
        {
            RoomGC.A.AddRecord();
        }
        public void Init(ConnToClient CTC, object[] OB)
        {
            Console.WriteLine("Client INit");
            string Openid = OB[1].ToString();
            CTC.Openid = Openid;
            if (!ClientMC.A.CTCD.ContainsKey(Openid))
            {
                ClientMC.A.CTCD.Add(Openid, CTC);
            }
        }
        public void ConnToServer(ConnToClient CTC, object[] OB)
        {
            string Openid = OB[1].ToString();
            CTC.Openid = Openid;
            if (!ClientMC.A.CTCD.ContainsKey(Openid))
            {
                ClientMC.A.CTCD.Add(Openid, CTC);
            }
        }
        public void DisConnToServer(ConnToClient CTC, object[] OB)
        {
            if (CTC.Openid != null)
                ClientMC.A.CTCD.Remove(CTC.Openid);
            CTC.Close();
        }
        public void SendMSGTo(ConnToClient CTC, object[] OB)
        {
            Console.WriteLine("MSGProcess");
            string MSG = OB[1].ToString();
            NewMSG newMSG = JsonConvert.DeserializeObject<NewMSG>(MSG);
            switch (newMSG.MSGState)
            {
                case State.Friend:
                    Thread FriendMSGSendTo = new Thread(new ParameterizedThreadStart(FriendMSGSend));
                    FriendMSGSendTo.Start(newMSG);
                    break;
                case State.Team: break;
                case State.Wrold:
                    Thread WroldMSGSendTo = new Thread(new ParameterizedThreadStart(WroldMSGSend));
                    WroldMSGSendTo.Start(newMSG);
                    Console.WriteLine("send to" + newMSG.Destination);
                    break;
            }
        }
        private void FriendMSGSend(object obj)
        {
            NewMSG newMSG = (NewMSG)obj;
            Console.WriteLine(newMSG.MSGText);
            ProtocolBytes MSG = new ProtocolBytes();
            MSG.AddData("MSG");
            string MSGStr = JsonConvert.SerializeObject(newMSG);
            MSG.AddData(MSGStr);
            if (ClientMC.A.CTCD.ContainsKey(newMSG.Destination))
                ClientMC.A.CTCD[newMSG.Destination].Send(MSG);
        }
        public void WroldMSGSend(object O)
        {
            NewMSG newMSG = (NewMSG)O;
            Console.WriteLine(newMSG.MSGText);
            ProtocolBytes MSG = new ProtocolBytes();
            MSG.AddData("MSG");
            string MSGStr = JsonConvert.SerializeObject(newMSG);
            MSG.AddData(MSGStr);
            Dictionary<string, ConnToClient> Online = ClientMC.A.CTCD;
            foreach (ConnToClient CTC in Online.Values)
            {
                CTC.Send(MSG);
            }
        }
    }
}
