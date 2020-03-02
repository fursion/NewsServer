/**************************************************************************
模    块:        
项    目:        Ser1.Data
作    者:        M0KJIQ06S1TIC8K by fursion
创建时间:         2020/2/27 15:01:44
E-mail:         fursion@fursion.cn
Copyright (c)    fursion.cn

描    述：房间基类
***************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using MSG_Server.Net;

namespace NewsServer.Data
{
    public class Room
    {
        private string roomid;
        /// <summary>
        /// 信息服务器的房间号，
        /// </summary>
        public string RoomID { get { return roomid; } set { roomid = value; } }
        private string key;
        public string Key { get { return key; } set { key = value; } }
        private bool voice;
        /// <summary>
        /// 语音权限
        /// </summary>
        public bool Voice { get { return voice; } set { voice = value; } }
        private bool words;
        /// <summary>
        /// 文字权限
        /// </summary>
        public bool Words { get { return words; } set { words = value; } }
        /// <summary>
        /// 房间内的人数
        /// </summary>
        private int number_of_merbers;
        public int freeNumber;
        public int Number_Of_Merbers { get { return number_of_merbers; } set { number_of_merbers = value; CTCS = new ConnToClient[number_of_merbers]; freeNumber = number_of_merbers; } }
        public ConnToClient[] CTCS;
        public Room()
        {
            StartServer();
        }

        public void StartServer()
        {

        }
        public int GetIndex()
        {
            foreach (ConnToClient CTC in CTCS)
            {
                if (CTC != null)
                    freeNumber--;
            }
            int Index = 0;
            foreach (ConnToClient iPEnd in CTCS)
            {
                if (iPEnd != null)
                    Index++;
                if (iPEnd == null)
                    return Index;
            }
            return -1;
        }
        public void SendNews()
        {

        }
        public void Close()
        {

        }
    }
}
