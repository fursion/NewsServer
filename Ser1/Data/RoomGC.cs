/**************************************************************************
模    块:        
项    目:        Ser1.Data
作    者:        M0KJIQ06S1TIC8K by fursion
创建时间:         2020/2/27 16:06:12
E-mail:         fursion@fursion.cn
Copyright (c)    fursion.cn

描    述：房间信息服务管理模块
***************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using MSG_Server;
using MSG_Server.Net;
using System.Web.Security;

namespace NewsServer.Data
{
    public enum NewsRoomType
    {
        Words,
        VoiceAndWords
    }
    public class RoomGC
    {
        public static RoomGC A;
        public RoomGC()
        {
            A = this;
        }
        /// <summary>
        /// 信息服务房间登记表
        /// </summary>  
        public Dictionary<string, Room> RoomList = new Dictionary<string, Room>();
        /// <summary>
        /// 创建信息服务房间
        /// </summary>
        /// <param name="RoomType">房间类型</param>
        /// <param name="MaxNumber">人数上限</param>
        /// <param name="OrderNumber">申请单号</param>
        public void CrateRoom(NewsRoomType RoomType, int MaxNumber, string OrderNumber)
        {
            switch (RoomType)
            {
                ///创建语音房间并加入管理列表
                case NewsRoomType.VoiceAndWords:
                    VoiceAndWordsRoom VAWR = new VoiceAndWordsRoom
                    {
                        RoomID = MadeRoomID(),
                        Key = MadeKey(),
                        Number_Of_Merbers = MaxNumber
                    };
                    RoomList.Add(VAWR.RoomID, VAWR);
                    ReturnRoomIDAndKey(OrderNumber, VAWR.RoomID, VAWR.Key);
                    break;
                ///创建文字列表并加入管理列表
                case NewsRoomType.Words:
                    WordsRoom WR = new WordsRoom
                    {
                        RoomID = MadeRoomID(),
                        Key = MadeKey(),
                        Number_Of_Merbers = MaxNumber
                    };
                    RoomList.Add(WR.RoomID, WR);
                    ReturnRoomIDAndKey(OrderNumber, WR.RoomID, WR.Key);
                    break;
            }
        }
        /// <summary>
        /// 将信息服务房间的ID和秘钥返回给主服务器
        /// </summary>
        /// <param name="OrderNumber">申请创建房间的单号</param>
        /// <param name="RoomID">创建好的房间ID</param>
        /// <param name="Key">创建好的房间秘钥</param>
        private void ReturnRoomIDAndKey(string OrderNumber, string RoomID, string Key)
        {
            RetNewsRoomIDAndKey RNRIAK = new RetNewsRoomIDAndKey()
            {
                OrderNumber = OrderNumber,
                RoomID = RoomID,
                Key = Key
            };
            ToMainServerConn.A.SendToMainServer(Fursion.Protocol.Fursion_Protocol.ReturnRoomIDAndKey, RNRIAK);
        }
        /// <summary>
        /// 加入信息房间
        /// </summary>
        /// <param name="roomid">RoomID</param>
        /// <param name="key">目标房间的秘钥</param>
        /// <param name="CTC">连接对象</param>
        public void AddRecord(string roomid, string key, ConnToClient CTC)
        {

            if (RoomList.ContainsKey(roomid))//监测房间是否存在
            {
                Room room = RoomList[roomid];
                if (key != room.Key)//核对秘钥
                    return;
                if (!room.CTCS.Contains(CTC))//检查请求地址是否已经被记录
                {
                    if (room.GetIndex() >= 0)//检查资源余量并取得索引
                    {
                        room.CTCS[room.GetIndex()] = CTC;//添加记录
                        //返回添加结果字符串
                        NewsRetinformation newsRetinformation = new NewsRetinformation
                        {
                            Ret = 0,
                            RoomID = room.RoomID
                        };
                        //调用发送函数
                        room.GetIndex();
                    }
                    else//返回添加失败信息
                    {
                        NewsRetinformation newsRetinformation = new NewsRetinformation
                        {
                            Ret = -1,
                            //调用发送函数
                            RoomID = room.RoomID
                        };

                    }
                }

            }
        }
        /// <summary>
        /// 按创建时间生成RoomID
        /// </summary>
        /// <returns></returns>
        public string MadeRoomID()
        {
            Console.WriteLine(DateTime.UtcNow.ToString());
            TimeSpan TS = DateTime.UtcNow - new DateTime(1949, 10, 1, 0, 0, 0);
            return Convert.ToInt64(TS.TotalSeconds).ToString();
        }
        /// <summary>
        /// 生成房间秘钥
        /// </summary>
        /// <returns></returns>
        public string MadeKey()
        {
            string key = Membership.GeneratePassword(10, 0);
            return key;
        }
    }
}
