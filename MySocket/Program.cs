using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace MySocket
{
    class Program
    {
        static void Main(string[] args)
        {
            Server server = new Server();
            server.Start("172.16.0.4", 10081);
            while (true)
            {
                string str = Console.ReadLine();
                switch (str)
                {
                    case "quit":
                        return;
                }
            }
        }
    }
}
