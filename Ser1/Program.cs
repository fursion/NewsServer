using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using MSG_Server.Net;
using System.Threading;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Fursion.Protocol;
using NewsServer.Data;

namespace MSG_Server
{
    class Program
    {      
        public static Thread Toserver= new Thread(ToS);
        public static ToMainServerConn  TMSC = new ToMainServerConn();
        static void Main(string[] args)
        {
            RoomGC RGC = new RoomGC();
            RGC.CrateRoom(NewsRoomType.VoiceAndWords,3);
            Console.Title = "MSG_SERVER_1";
            string hostName = Dns.GetHostName();   //获取本机名
            IPHostEntry localhost = Dns.GetHostByName(hostName);//方法已过期，可以获取IPv4的地址
            IPAddress localaddr = localhost.AddressList[0];
            string IP = localaddr.ToString();
            ClientMC CMC = new ClientMC();//启动本地服务
            CMC.Start(IP, 2222);
            ConnDataBase CDB = new ConnDataBase();
            //bool TOSER=  TMSC.ToConnServer("192.168.137.1", 2012);//连接主服务器
            bool TOSER = TMSC.ToConnServer("106.52.184.86", 2012);//连接主服务器
            //bool TOSER = TMSC.ToConnServer("192.168.123.2", 2012);//连接主服务器
            if (!TOSER)
            {
                Toserver.Start();
            }
            SystemInfo info = new SystemInfo();
            Console.WriteLine(Sys.GetListTime());
            //for(int i = 0; i < 50; i++)
            //{
            //    ToMainServerConn to = new ToMainServerConn();
            //    ToMainServerConn.Vrsion = i;
            //    to.ToConnServer("192.168.137.1", 2012);
            //}
            while (true)
            {
                string str = Console.ReadLine();
                switch (str)
                {
                    case "server":
                        CMC.printinfo();
                        break;
                }
            }
        }
        public static void ToS()
        {
            bool TOSER = ToMainServerConn.A.ToServerState;
            while (!TOSER)
            {
                //TOSER = TMSC.ToConnServer("192.168.137.1", 2012);
                //TOSER = TMSC.ToConnServer("192.168.123.2", 2012);//连接主服务器
                TOSER = TMSC.ToConnServer("106.52.184.86", 2012);//连接主服务器
            }
        }
        public static void SendUT()
        {
            while (true)
            {
                float UT = ClientMC.A.Utilization();
                Math.Round(UT, 10);
                // ToMainServerConn.A.SendToMainServer(UT.ToString());
                Console.WriteLine(UT + "%  " + ClientMC.A.ClientOnlineNumber);
                Thread.Sleep(10);
            }
        }
    }
}
