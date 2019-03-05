using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace MySocket
{
    class Server
    {
        //监听嵌套字
        public Socket listedfd;
        //客户端连接
        public Conn[] conns;
        //最大连接数
        public int maxConn = 50;
        //获取连接池 返回负数表示获取失败
        public int NewIndex()
        {
            if (conns == null)
            {
                return -1;
            }
            for (int i = 0; i < conns.Length; i++)
            {

                if (conns[i] == null)
                {
                    conns[i] = new Conn();
                    return i;
                }
                else if (conns[i].isUse == false)
                {
                    return i;
                }
            }
            return -1;
        }
        //开启服务器的方法
        public void Start(string host,int port)
        {
            //连接池
            conns = new Conn[maxConn];
            for(int i = 0; i < maxConn; i++)
            {
                conns[i] = new Conn();
            }
            //Socket
            listedfd = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //Bind
            IPAddress ipAdr = IPAddress.Parse(host);
            IPEndPoint ipEp = new IPEndPoint(ipAdr, port);
            //开启监听
            listedfd.Bind(ipEp);
            listedfd.Listen(maxConn);
            listedfd.BeginAccept(AcceptCb, null);
            Console.WriteLine("服务器启动成功");

        }
        //回调
        private void AcceptCb(IAsyncResult ar)
        {
            try
            {
                Socket socket = listedfd.EndAccept(ar);
                int index = NewIndex();
                if (index < 0)
                {
                    socket.Close();
                    Console.WriteLine("警告，连接已满");
                }
                else
                {
                    Conn conn = conns[index];
                    conn.Init(socket);
                    string adr = conn.GetAdress();
                    Console.WriteLine("有客户端连接" + adr + "ID号为" + index);
                    conn.socket.BeginReceive(conn.readBuff, conn.bufCount, conn.BuffRemain(), SocketFlags.None, ReceiveCb, conn);
                    listedfd.BeginAccept(AcceptCb, null); 
                }
                
            }
            catch(Exception e)
            {
                Console.WriteLine("回调失败" + e.Message);
            }

        }
        private  void ReceiveCb(IAsyncResult ar)
        {
            Conn conn = (Conn)ar.AsyncState;
            try
            {
                int count = conn.socket.EndReceive(ar);
                if (count <= 0)
                {
                    Console.WriteLine(conn.GetAdress() + "与服务器断开连接");
                    conn.Close();
                    return;
                }
                string str = System.Text.Encoding.UTF8.GetString(conn.readBuff, 0, count);
                Console.WriteLine("收到" + conn.GetAdress() + "发过来的数据");//+str
                //str = conn.GetAdress() + ":" + str;
                byte[] bytes = System.Text.Encoding.UTF8 .GetBytes(str);
                byte[] record = System.Text.Encoding.UTF8.GetBytes(str+ "---"+DateTime.Now.ToString()+ "\r\n");
                //聊天记录
                FileStream fs = new FileStream("C:\\Chatting Room\\Record.txt", FileMode.Append);
                //开始写入
                fs.Write(record, 0, record.Length);
                //清空缓冲区、关闭流
                fs.Flush();
                fs.Close();
                //广播消息
                for (int i = 0; i < conns.Length; i++)
                {
                    if (conns[i] == null)
                        continue;
                    if (!conns[i].isUse)
                        continue;
                    Console.WriteLine("服务器将消息转发给" + conns[i].GetAdress());
                    conns[i].socket.Send(bytes);
                }
                //继续接收数据
                conn.socket.BeginReceive(conn.readBuff, conn.bufCount,conn.BuffRemain(), SocketFlags.None, ReceiveCb,conn);
            }
            catch(Exception e)
            {
                Console.WriteLine("收到" + conn.GetAdress() + "断开连接" + e.Message);
                conn.Close();
            }
        }
    }

}
