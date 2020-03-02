using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace NewsServer.Data
{
    class ConnDataBase
    {
        MySqlConnection SQLConn;
        public static ConnDataBase A;
        public ConnDataBase()
        {
            A = this;
            Connect();
        }
        private void Connect()
        {
            string ConnStr= "Database=TankTest;DataSource=cdb-ahtsamo2.cd.tencentcdb.com;";
            ConnStr += "User ID=root;Password=Dj199706194430;port=10000;";
            SQLConn = new MySqlConnection(ConnStr);
            try {
                SQLConn.Open();
                Console.WriteLine("连接数据库成功");
            } catch(Exception e)
            {
                SQLConn.Dispose();
                Console.WriteLine("数据库连接失败 ："+e.Message);
                Connect();
            }           
        }
        public UserData GetUserData(string OPENID)
        {
            UserData UD = null;
            string Str = string.Format("select * from Player where ID='{0}';", OPENID);
            if (SQLConn.State == ConnectionState.Open)
            {
                MySqlCommand cmd = new MySqlCommand(Str, SQLConn);
                byte[] buff = new byte[1];
                MySqlDataReader reader = cmd.ExecuteReader();
                if (!reader.HasRows)
                {
                    reader.Close();
                    reader.Dispose();
                    return UD;
                }
                reader.Read();
                long len = reader.GetBytes(3, 0, null, 0, 0);
                buff = new byte[len];
                reader.GetBytes(3,0, buff, 0, (int)len);
                reader.Close();
                MemoryStream stream = new MemoryStream(buff);
                BinaryFormatter formatter = new BinaryFormatter();
                UD = (UserData)formatter.Deserialize(stream);
                return UD;
            }
            else
            {
                Connect();
                GetUserData(OPENID);
                return UD;
            }   
        }
    }
}
