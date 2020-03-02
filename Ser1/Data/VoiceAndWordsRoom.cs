/**************************************************************************
模    块:        
项    目:        Ser1.Data
作    者:        M0KJIQ06S1TIC8K by fursion
创建时间:         2020/2/27 15:11:05
E-mail:         fursion@fursion.cn
Copyright (c)    fursion.cn

描    述：文字和语音服务房间
***************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsServer.Data
{
    class VoiceAndWordsRoom:Room
    {
        public VoiceAndWordsRoom()
        {
            Voice = true;
            Words = true;
        }

    }
}
