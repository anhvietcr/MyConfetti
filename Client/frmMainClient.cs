using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections;
using Newtonsoft.Json.Linq;
using NAudio.Wave;
using NAudio.Wave.Compression;
using Codecs.Codecs;
using Codecs;


namespace Client
{
    public partial class frmMainClient : Form
    {
        #region GLOBAL

        private TcpClient client                        = null;
        private StreamWriter writer                     = null;
        private StreamReader reader                     = null;
        private string id                               = string.Empty;
        private static int score                        = 0;
        private static string answer                    = null;
        private static bool isDisconnect                = false;

        // Audio record Properties
        private UdpClient udpListener;
        private IWavePlayer waveOut;
        private BufferedWaveProvider waveProvider;
        private INetworkChatCodec codec;
        private volatile bool connected;
        #endregion

        public frmMainClient()
        {
            InitializeComponent();
        }

        /**
         * 
         * Buttons Event
         * 
         * */
        private void btn_connect_Click(object sender, EventArgs e)
        {
            string ip = null;
            int port = 0;

            // valid IP address and PORT
            if (string.IsNullOrEmpty(txt_ip.Text)) return;
            if (string.IsNullOrEmpty(txt_port.Text)) return;

            // valid IP format
            ip = txt_ip.Text;

            // valid PORT format
            if (int.TryParse(txt_port.Text, out int _port))
                port = _port;
            else
                return;

            // toggle button
            btn_connect.Enabled = false;
            btn_close.Enabled = true;

            // Let's go
            ConnectServer(ip, port);
        }

        private void btn_answer_Clicked(object sender, EventArgs e)
        {
            if (client == null) return;

            // get answer A, B, C from user
            answer = (sender as Button).Tag.ToString();

            answer_A.Enabled = false;
            answer_B.Enabled = false;
            answer_C.Enabled = false;

            writer.WriteLine("answer");
            writer.WriteLine(id);
            writer.WriteLine(answer);
        }

        private void btn_close_Click(object sender, EventArgs e)
        {
            // toggle button
            btn_connect.Enabled = true;
            btn_close.Enabled = false;

            isDisconnect = true;
        }

        class ListenerThreadState
        {
            public IPEndPoint EndPoint { get; set; }
            public INetworkChatCodec Codec { get; set; }
        }

        int i = 0;
        byte[] buffer = new byte[1024];
        byte[] decoded = null;
        private void ListenerThread(object state)
        {
            ListenerThreadState listenerThreadState = (ListenerThreadState)state;
            IPEndPoint endPoint = listenerThreadState.EndPoint;
            try
            {
                while (connected)
                {
                    buffer = udpListener.Receive(ref endPoint);
                    decoded = listenerThreadState.Codec.Decode(buffer, 0, buffer.Length);
                    waveProvider.AddSamples(decoded, 0, decoded.Length);
                    Console.WriteLine("{1} audio receive size {0}", decoded.Length, i++);
                }
            }
            catch (SocketException ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }


        /**
         * 
         * Client's
         * 
         **/
        public void ConnectServer(string ip, int port)
        {
            //codec = new Gsm610ChatCodec(); // echo
            codec = new G722ChatCodec(); // echo 
            //codec = new AcmALawChatCodec(); // bad
            //codec = new ALawChatCodec(); // bad - G711
            //codec = new AcmMuLawChatCodec(); // bad
            //codec = new MicrosoftAdpcmChatCodec(); // mute
            //codec = new MuLawChatCodec(); // bad
            //codec = new TrueSpeechChatCodec(); // error
            //codec = new UncompressedPcmChatCodec(); // echo                                        


            try
            {
                // Audio Record
                // connect via UDP
                //IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(ip), port);
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, port);
                udpListener = new UdpClient();
                udpListener.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                udpListener.Client.Bind(endPoint);
                Console.WriteLine("Connect UDP for Audio at {0}:{1}", endPoint.Address, endPoint.Port);


                // play audio
                waveOut = new WaveOut();
                waveProvider = new BufferedWaveProvider(codec.RecordFormat);
                waveOut.Init(waveProvider);
                waveOut.Play();

                connected = true;
                ListenerThreadState threadState = new ListenerThreadState() { Codec = codec, EndPoint = endPoint };
                ThreadPool.QueueUserWorkItem(this.ListenerThread, threadState);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }


            Thread mainThread = new Thread(() =>
            {
                string msg = string.Empty;
                string state = string.Empty;
                string received = string.Empty;

                try
                {
                    // connect
                    client = new TcpClient();
                    client.Connect(IPAddress.Parse(ip), port);
                    txt_status.Text = string.Format("Connected to {0}:{1}", ip, port);

                    // read, write to server using stream, over use bytes[]
                    Stream streamer = client.GetStream();
                    reader = new StreamReader(streamer);
                    writer = new StreamWriter(streamer);
                    writer.AutoFlush = true;


                    // get user ID, state game
                    writer.WriteLine("init");
                    id = reader.ReadLine();
                    state = reader.ReadLine();
                    Console.WriteLine("My received id: {0}, game play? {1}", id, state);


                    // get current question 
                    if (state.Contains("True"))
                    {
                        Console.WriteLine("Join. Game was play");

                        writer.WriteLine("question");
                        string numberQuestion = reader.ReadLine();
                        groupBox_question.Text = string.Format("Câu hỏi: số {0}", int.Parse(numberQuestion));

                        received = reader.ReadLine();
                        parseQuestion(received);

                        answer_A.Enabled = true;
                        answer_B.Enabled = true;
                        answer_C.Enabled = true;
                    }

                   while (true)
                   {
                        // check disConnect button was clicked
                        if (isDisconnect) break;

                        // listening msg from Server
                        msg = reader.ReadLine();

                        switch (msg)
                        {
                            case "play":
                                MessageBox.Show("Game play . . .");
                                break;

                            case "new question":
                                // id
                                string numberQuestion = reader.ReadLine();
                                groupBox_question.Text = string.Format("Câu hỏi: số {0}", Convert.ToInt32(numberQuestion) + 1);

                                //question 
                                received = reader.ReadLine();
                                parseQuestion(received);

                                answer_A.Enabled = true;
                                answer_B.Enabled = true;
                                answer_C.Enabled = true;
                                break;

                            case "correct":
                                received = reader.ReadLine();
                                txt_noti.Text = "Kết quả: " + received;
                                score++;
                                txt_score.Text = score.ToString();
                                break;

                            case "incorrect":
                                received = reader.ReadLine();
                                txt_noti.Text = "Kết quả: " + received;
                                txt_score.Text = score.ToString();
                                break;

                            default: break;
                        }
                    }
                    streamer.Close();
                    client.Close();
                }
                catch (IOException ex)
                {
                    Console.WriteLine(ex.Message);
                    throw;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    throw;
                }
            });
            mainThread.Start();
        }

        private void parseQuestion(string jsonQuestion)
        {
            try
            {
                Console.WriteLine(jsonQuestion);
                dynamic data = JObject.Parse(jsonQuestion);

                // update to GUI
                txt_question.Text = data.Question;
                answer_A.Text = data.A;
                answer_B.Text = data.B;
                answer_C.Text = data.C;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                throw;
            }
        }
    }
}
