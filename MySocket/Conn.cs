using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace MySocket
{
    class Conn
    {
        public const int buff_size = 1024;

        public Socket socket;

        public bool isUse = false;

        public byte[] readBuff = new byte[buff_size];

        public int bufCount = 0;

        public Conn()
        {
            readBuff = new byte[buff_size];
        }

        public void Init(Socket socket)
        {
            this.socket = socket;
            isUse = true;
            bufCount = 0;
        }

        public int BuffRemain()
        {
            return buff_size - bufCount;
        }

        public string GetAdress()
        {
            if (!isUse)
                return "无";
            return socket.RemoteEndPoint.ToString();            
        }

        public void Close()
        {
            if (!isUse)
                return;
            Console.WriteLine(GetAdress() + "与服务器断开连接");
            socket.Close();
            isUse = false;
        }
    }
}
