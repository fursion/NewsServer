using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public enum State
{
    Friend,
    Wrold,
    Team
}
class Var
{
    public static readonly string ServerInfo = "ServerInfo";

}
class MethodName
{

}
[Serializable]
public class UserData
{
    public string NickNeam;
    public string NickWebPath;
    public string Openid;
}
/// <summary>
/// 消息包JSON模板
/// </summary>
[Serializable]
public class NewMSG
{
    public string Source;
    public string Destination;
    public string MSGText;
    public State MSGState;
}
/// <summary>
/// 返回给客户端处理结果的JSON模板
/// </summary>
[Serializable]
public class NewsRetinformation
{
    public int Ret;
    public string RoomID;
}
/// <summary>
/// 返回给主服务器信息房间信息的JSON模板
/// </summary>
[Serializable]
public class RetNewsRoomIDAndKey
{
    public string OrderNumber;
    public string RoomID;
    public string Key;
}

