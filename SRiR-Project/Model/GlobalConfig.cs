using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Interop;

namespace SRiR_Project.Model
{
    class GlobalConfig
    {
        private static GlobalConfig _instance;
        public static GlobalConfig Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GlobalConfig();
                }
                return _instance;

            }
        }

        private GlobalConfig()
        {
            Username = "";
            OtherUserName = "";
            AppType = Type.Not_Declared;
            ServerOnline = false;
            mode = 0;
            Shiptext = "****\n***\n***\n**\n**\n**\n*\n*\n*\n*";
        }
        public string Username;
        public enum Type { Server, Client, Not_Declared }

        public Type AppType;

        private TcpListener ServerListener;
        private TcpClient Client;
        private NetworkStream ns;
        private int Port;
        private string Ip;
        public string OtherUserName;
        public bool isHost;
        public int mode; //0 - 3 strzały, 1 - 1+n strzałów

        public bool ServerOnline;
        private Thread _ServerThread;
        public void InitServer(int port)
        {
            ServerListener = TcpListener.Create(port);
            ServerListener.Start();
            _ServerThread = new Thread(new ThreadStart(IncomingConnections));
            ServerOnline = true;
            _ServerThread.Start();
        }
        public void StopServer()
        {
            ServerListener.Stop();
            _ServerThread.Abort();
        }
        private void IncomingConnections()
        {
            while (ServerOnline)
            {
                if (ServerListener.Pending())
                {
                    Client = ServerListener.AcceptTcpClient();
                    ns = Client.GetStream();
                    string msg = GetMessage();
                    if (msg.Substring(0, 2) == "hi")
                    {
                        OtherUserName = msg.Substring(2);
                        SendMessage("hi" + Username);
                        ServerOnline = false;
                    }
                }
                else
                {
                    Thread.Sleep(1000);
                }
            }
            ServerListener.Stop();
        }

        public void DenyLobby()
        {
            OtherUserName = "";
            SendMessage("end");
            ns.Close();
            Client.Close();
            if (!_ServerThread.IsAlive)
            {
                ServerListener.Start();
                _ServerThread = new Thread(new ThreadStart(IncomingConnections));
                _ServerThread.Start();
            }
        }

        public void AcceptLobby()
        {
            SendMessage("oki" + mode);

        }

        /****************
         *
         * Klient
         * 
         ****************/

        public bool JoinSerwer(string ip, string port)
        {
            Client = new TcpClient(ip, Convert.ToInt32(port));
            ns = Client.GetStream();
            SendMessage("hi" + Username);
            string ans = GetMessage();
            if (ans.Substring(0, 2) == "hi")
            {
                OtherUserName = ans.Substring((2));
                return true;
            }
            return false;
        }

        public int CheckDecision()
        {
            if (ns.DataAvailable)
            {
                string ans = GetMessage();
                if (ans.Substring(0, 3) == "oki")
                {
                    mode = Convert.ToInt32(ans.Substring(3));
                    return 1;
                }
                else
                {
                    OtherUserName = "";
                    ns.Close();
                    Client.Close();
                    return 0;
                }
            }
            return -1;

        }

        public string GetMessage()
        {
            Byte[] bytes = new Byte[512];
            int k = ns.Read(bytes, 0, bytes.Length);
            String data = Encoding.ASCII.GetString(bytes, 0, k);

            return data;
        }

        public bool PendingMessages()
        {
            return ns.DataAvailable;
        }

        public bool SendMessage(string txt)
        {
            Byte[] data = System.Text.Encoding.ASCII.GetBytes(txt);
            ns.Write(data, 0, data.Length);

            return true;
        }

        /****************
         *
         * Gra
         * 
         ****************/

        public string Infotext;

        public void InfotextAdd(string txt)
        {
            Infotext = ">> " + txt + "\n" + Infotext;
        }

        public string Shiptext;
    }
}
