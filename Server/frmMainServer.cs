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
using System.Timers;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using Question;



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
        static List<Socket> listSocket = new List<Socket>();
        static bool isPlay { get; set; }
        static bool isExit { get; set; } = false;
        static bool isNewQuestion { get; set; } = false;
        static int tick = 0;
        static int numberQuestion = 0;
        string[] questions = null;
        string jsonQuestion { get; set; } = string.Empty;


        #endregion


        private System.Windows.Forms.Timer tmr;

        public frm_server()
        {
            InitializeComponent();
        }
 

        /**
         * 
         * Buttons Event
         * 
         **/
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
            btn_play.Enabled = true;

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

        private void btn_play_Click(object sender, EventArgs e)
        {
            if (numberConnecting < 1)
            {
                MessageBox.Show("Chưa có người tham gia !");
                return;
            }

            if (string.IsNullOrEmpty(txtBoxFileName.Text) || questions == null)
            {
                MessageBox.Show("Chưa chọn file chứa câu hỏi !");
                return;
            }

            isPlay = true;
            btn_play.Enabled = false;
            btnNext.Enabled = true;

            // message to all client for start game
            foreach (Socket client in listSocket)
            {
                using (StreamWriter writer = new StreamWriter(new NetworkStream(client)))
                {
                    writer.WriteLine("play");
                }
            }
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (!File.Exists(txtBoxFileName.Text))
            {
                MessageBox.Show("File không tồn tại, vui lòng thử lại!");
                return;
            }

            if (questions.Length <= 0)
            {
                MessageBox.Show("Chưa chọn file chứa câu hỏi !");
                return;
            }

            if (numberQuestion >= questions.Length)
            {
                MessageBox.Show("Hết câu hỏi !");
                numberQuestion = 0;
                btn_play.Enabled = true;
                btnNext.Enabled = false;
                txtBoxFileName.Text = "";
                return;
            }

            jsonQuestion = questions[numberQuestion];
            sendQuestion(jsonQuestion, numberQuestion);

            numberQuestion++;
        }

        private void btnChoose_Click(object sender, EventArgs e)
        {
            if (openFileDialogQuestion.ShowDialog() == DialogResult.OK)
            {
                txtBoxFileName.Text = openFileDialogQuestion.FileName;

                // Get datas question[]
                Data read = new Data();
                questions = read.readFile(txtBoxFileName.Text);
            }
        }


        /**
         * 
         * Start SERVER 
         * 
         **/
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
                    listSocket.Add(acceptSocket);
                    numberConnecting++;
                    txt_numberConnect.Text = numberConnecting.ToString();

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

            int questionID  = 1;
            string answer   = string.Empty;
            string msg      = string.Empty;
            string idUser   = string.Empty;

            try
            {
                // read, write from client using stream, over use bytes[]
                NetworkStream streamer = new NetworkStream(client);
                StreamReader reader = new StreamReader(streamer);
                StreamWriter writer = new StreamWriter(streamer);

                // should flush buffer stream auto 
                writer.AutoFlush = true;

                Console.WriteLine("<<ID: {0}>> New connect from {1}", numberConnecting, client.RemoteEndPoint);


                //var timer = new System.Timers.Timer(1000);
                //timer.Elapsed += timer_Elapsed;
                //timer.Start();


                while (true)
                {
                    // listen from client
                    msg = reader.ReadLine();

                    switch (msg)
                    {
                        case "init":
                            // send to client's ID, state game
                            writer.WriteLine(numberConnecting);
                            writer.WriteLine(isPlay);
                            break;

                        case "question":
                            writer.WriteLine(numberQuestion);
                            writer.WriteLine(@jsonQuestion);
                            break;

                        case "answer":
                            Console.WriteLine("get answer");
                            idUser = reader.ReadLine();
                            answer = reader.ReadLine();

                            Console.WriteLine("{0} answer {1}", idUser, answer);

                            // check awnser
                            if (CheckAwnserForQuestion(answer, questionID))
                            {
                                writer.WriteLine("correct");
                                writer.WriteLine("Bạn trả lời đúng\n");
                            }
                            else
                            {
                                writer.WriteLine("incorrect");
                                writer.WriteLine("Bạn trả lời sai\n");
                            }
                            break;
                        default:
                            continue;
                    }

                    if (isDisconnect)
                    {
                        mainThread.Abort();
                        server.Server.Close();
                        Console.WriteLine("Server was closed from client send");
                        isDisconnect = false;
                        break;
                    }

                    // client send exit status
                    if (isExit) break;
                }
                streamer.Close();
            }
            catch (NullReferenceException ex)
            {
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
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

        static void sendQuestion(string jsonQuestion, int numberQuestion)
        {
            // valid running in game loop
            if (!isPlay) return;

            // message to all client for start game
            foreach (Socket client in listSocket)
            {
                using (StreamWriter writer = new StreamWriter(new NetworkStream(client)))
                {
                    writer.WriteLine("new question");
                    writer.WriteLine(numberQuestion);
                    writer.WriteLine(@jsonQuestion);
                }
            }
        }

        static string getQuestion(int i)
        {
            // get new question by ID
            return @"{'Question':'Cau hoi ????','A':'Dap an A','B':'Dap an B','C':'Dap an C','answer':'A',}";
        }

        // every 1 second timer tick
        static void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (!isPlay) return;

            if (numberQuestion >= 10)
            {
                return;
                // done a collect include 10 question
            }

            Thread.Sleep(3000);
            tick += 1;
            //Console.Clear();
            Console.WriteLine("{0}s", tick);

            //if (tick == 10)
            //{
            //    Console.WriteLine("New question avaliable");
            //    isNewQuestion = true;
            //    tick = 0;
            //    ++numberQuestion;

            //    jsonQuestion = getQuestion(numberQuestion);
            //    sendQuestion(jsonQuestion);
            //}
        }
    }
}
