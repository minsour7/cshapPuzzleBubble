using Server.Game;
using ServerCore;
using System;
using System.Net;
using Server.Data;


namespace Server
{
    class Program
    {
        static Listener _listener = new Listener();

        static void Main(string[] args)
        {
            ConfigManager.LoadConfig();

            RoomManager.Instance.Add().Init();

            // DNS (Domain Name System)
            string host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[0];

            //IPAddress ipAddr = IPAddress.Parse("192.168.123.110");

            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7779);

            _listener.Init(endPoint, () => { return SessionManager.Instance.Generate(); });
            Console.WriteLine("Listening...");

            while (true)
            {

            }
        }
    }
}
