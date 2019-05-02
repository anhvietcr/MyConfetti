using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;


namespace Server
{
    public partial class frm_server : Form
    {
        #region GLOBAL

        const int MAX_CONNECT       = 100;
        static int numberConnecting = 0;
        static TcpListener server   = null;
        Thread mainThread           = null;
        static bool isDisconnect    = false;

        #endregion


        public frm_server()
        {
            InitializeComponent();
        }

        private void btn_open_Click(object sender, EventArgs e)
        {
            string ip = null;
            int port = 0;

            // valid IP address and PORT
            if (string.IsNullOrEmpty(txt_ip.Text)) return;
            if (string.IsNullOrEmpty(txt_port.Text)) return;

            // valid IP format
            ip = txt_ip.Text;

            // valid type of PORT
            if (int.TryParse(txt_port.Text, out int _port))
                port = _port;
            else
                return;

            // toggle button 
            btn_open.Enabled = false;
            btn_close.Enabled = true;

            // Let's go
            StartServer(ip, port);
        }

        private void btn_close_Click(object sender, EventArgs e)
        {
            // toggle button 
            btn_open.Enabled = true;
            btn_close.Enabled = false;

            isDisconnect = true;
        } 

        public void StartServer(string ip, int port)
        {
            // start
            Console.WriteLine("Open server at {0}:{1} . . .", ip, port);
            IPAddress ipAddress = IPAddress.Parse(ip);

            Thread mainThread = new Thread(() =>
            {
                server = new TcpListener(ipAddress, port);
                server.Start();


                // listen from client
                // open a new Thread if a Client connecting
                while (numberConnecting < MAX_CONNECT)
                {
                    Socket acceptSocket = server.AcceptSocket();
                    numberConnecting++;

                    if (isDisconnect)
                    {
                        break;
                    }

                    // ready for communications
                    if (acceptSocket.Connected)
                    {
                        Thread thread = new Thread((client) =>
                        {
                            ConnectClient((Socket)client);
                        });
                        thread.Start(acceptSocket);
                    }
                }
                server.Server.Close();
                isDisconnect = false;
                Console.WriteLine("Server was closed from main");

            });
            mainThread.Start();
        }

        private void ConnectClient(Socket client)
        {
            // okay, a client was connect
            // receive data from client for valid
            Console.WriteLine("New connect from {0}", client.RemoteEndPoint);
            int questionID = 1;

            try
            {
                // read, write from client using stream, over use bytes[]
                NetworkStream streamer = new NetworkStream(client);
                StreamReader reader = new StreamReader(streamer);
                StreamWriter writer = new StreamWriter(streamer);

                // should flush buffer stream auto 
                writer.AutoFlush = true;

                // send to client's ID
                writer.WriteLine(numberConnecting);
                Console.WriteLine("Client id {0}", numberConnecting);
                string answer;

                while (true)
                {
                    answer = reader.ReadLine();

                    if (isDisconnect)
                    {
                        mainThread.Abort();
                        server.Server.Close();
                        Console.WriteLine("Server was closed from client send");
                        isDisconnect = false;
                        break;
                    }

                    // client send exit status
                    if (answer == "exit") break;

                    if (string.IsNullOrEmpty(answer)) continue;
                    
                    string[] info = answer.Split('!');
                    if (info.Length <= 1) continue;
                    Console.WriteLine("User {0} choose answer {1}", info[0], info[1]);

                    // check awnser
                    if (CheckAwnserForQuestion(info[1], questionID))
                    {
                        writer.Write("1!Bạn trả lời đúng\n");
                    } else
                    {
                        writer.Write("0!Bạn trả lời sai\n");
                    }
                }
                streamer.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            // Client was disconnect, should close.
            Console.WriteLine("Disconnected from {0}", client.RemoteEndPoint);
            client.Close();
        }

        private bool CheckAwnserForQuestion(string answer, int questionID)
        {
            if (answer == "A") return true;

            return false;
        }
    }
}
