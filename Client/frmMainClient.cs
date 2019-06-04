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
        private StreamWriter writer = null;
        private StreamReader reader = null;
        private string id                               = string.Empty;
        private  int score                        = 0;
        private  string answer                    = null;
        private  bool isDisconnect                = false;
       private  bool flag = false;
        // Audio record Properties
        private UdpClient udpAudioListener;
        private IWavePlayer waveOut;
        private BufferedWaveProvider waveProvider;
        private INetworkChatCodec codec;
        private volatile bool connected;

        // Webcam
        private UdpClient udpWebcamListener;
        private TcpClient TcpWebcamClient;
        #endregion

        public frmMainClient()
        {
            InitializeComponent();

            txt_question.Location = new Point(-31, -334);
            answer_A.Location = new Point(-165, -384);
            answer_B.Location = new Point(-165, -420);
            answer_C.Location = new Point(-165, -456);
        }

        #region Buttons Event
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
            flag = true;
            if (client == null) return;
          //  timer1.Enabled = true;
            // get answer A, B, C from user
            answer = (sender as Button).Tag.ToString();
            Console.WriteLine("Client answer {0}", answer);
            answer_A.Enabled = false;
            answer_B.Enabled = false;
            answer_C.Enabled = false;

            answer_A.BackColor = Color.Gray;
            answer_B.BackColor = Color.Gray;
            answer_C.BackColor = Color.Gray;
            (sender as Button).BackColor = Color.Blue;

            writer.WriteLine("answer");
            writer.WriteLine(id);
            writer.WriteLine(answer);

            Console.WriteLine("answer");
        }

        private void btn_close_Click(object sender, EventArgs e)
        {
            // toggle button
            btn_connect.Enabled = true;
            btn_close.Enabled = false;

            isDisconnect = true;
        }
        #endregion

        #region Audio
        class ListenerThreadState
        {
            public IPEndPoint EndPoint { get; set; }
            public INetworkChatCodec Codec { get; set; }
        }

        byte[] buffer = new byte[1024];
        byte[] decoded = null;
        private void ListenerAudioThread(object state)
        {
            ListenerThreadState listenerThreadState = (ListenerThreadState)state;
            IPEndPoint endPoint = listenerThreadState.EndPoint;
            try
            {
                while (connected)
                {
                    buffer = udpAudioListener.Receive(ref endPoint);
                    decoded = listenerThreadState.Codec.Decode(buffer, 0, buffer.Length);
                    waveProvider.AddSamples(decoded, 0, decoded.Length);
                   // Console.WriteLine("audio receive size {0}", decoded.Length);
                }
            }
            catch (SocketException ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
        #endregion

        #region Webcam
        private void ListenerWebcamThread(object sender)
        {
            try
            {
                while (connected)
                {
                    NetworkStream ns = TcpWebcamClient.GetStream();

                    byte[] data = Receive(ns);
                    byte[] outputBuffer = new byte[data.Length];
                   // Console.WriteLine("webcam receive size {0}", outputBuffer.Length);

                    // add to picture streamer
                    pictureBoxStreamer.Image = ByteToImage(data);
                }
            }
            catch (SocketException ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
        private byte[] Receive(NetworkStream netstr)
        {
            try
            {
                // Buffer to store the response bytes.
                byte[] recv = new Byte[256 * 1000];

                // Read the first batch of the TcpServer response bytes.
                int bytes = netstr.Read(recv, 0, recv.Length);
                byte[] a = new byte[bytes];

                for (int i = 0; i < bytes; i++)
                {
                    a[i] = recv[i];
                }
                return a;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

                return null;
            }

        }
        public Image ByteToImage(byte[] byteArrayIn)
        {

            System.Drawing.ImageConverter converter = new System.Drawing.ImageConverter();
            Image img = (Image)converter.ConvertFrom(byteArrayIn);

            return img;
        }
        #endregion

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
                // Audio Receive
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, port);
                udpAudioListener = new UdpClient();
                udpAudioListener.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                udpAudioListener.Client.Bind(endPoint);
                Console.WriteLine("Connect UDP for Audio at {0}:{1}", endPoint.Address, endPoint.Port);

                // Webcam Receive
                TcpWebcamClient = new TcpClient();
                TcpWebcamClient.Connect(ip, port + 1);
                Console.WriteLine("Connect UDP for Webcam at {0}:{1}", ip, port + 1);

                connected = true;

                // listening & play audio
                waveOut = new WaveOut();
                waveProvider = new BufferedWaveProvider(codec.RecordFormat);
                waveOut.Init(waveProvider);
                waveOut.Play();
                ListenerThreadState threadState = new ListenerThreadState() { Codec = codec, EndPoint = endPoint };
                ThreadPool.QueueUserWorkItem(this.ListenerAudioThread, threadState);
                
                // listening & play webcam 
                ThreadPool.QueueUserWorkItem(this.ListenerWebcamThread);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }


            Thread mainThread = new Thread(() =>
            {
               
                string state = string.Empty;
                 string msg = string.Empty;//Biến lưu chủ đề trả từ server về
                 string received = string.Empty;//Biến lưu kết quả đúng sai
                 //StreamWriter writer = null;
                 //StreamReader reader = null;
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
                    txt_Id.Text = id;
                    string text = String.Format("Bạn là User_{0}", id);
                    MessageBox.Show(text, "Thông Báo!");
                    // get current question 
                    if (state.Contains("True"))
                    {
                        Console.WriteLine("Join. Game was play");

                        writer.WriteLine("question");
                        string numberQuestion = reader.ReadLine();
                        groupBox_webcam.Text = string.Format("Streaner - Câu hỏi: số {0}", int.Parse(numberQuestion));

                        received = reader.ReadLine();
                        parseQuestion(received);
                        
                        // markup UI
                        answer_A.Enabled = true;
                        answer_B.Enabled = true;
                        answer_C.Enabled = true;

                        answer_A.BackColor = Color.Honeydew;
                        answer_B.BackColor = Color.Honeydew;
                        answer_C.BackColor = Color.Honeydew;

                        txt_question.Location = new Point(31, 334);
                        answer_A.Location = new Point(165, 384);
                        answer_B.Location = new Point(165, 420);
                        answer_C.Location = new Point(165, 456);
                    }

                   while (true)
                   {
                        // check disConnect button was clicked
                        if (isDisconnect) break;

                        // listening msg from Server
                        msg = reader.ReadLine();

                        switch (msg)
                        {
                            case "img":
                                Console.WriteLine("get img");
                                break;

                            case "play":
                                MessageBox.Show("Game play . . .");
                                break;

                            case "new question":
                               
                                // id
                                string numberQuestion = reader.ReadLine();
                                groupBox_webcam.Text = string.Format("Streamer - Câu hỏi: số {0}", Convert.ToInt32(numberQuestion) + 1);

                                //question 
                                received = reader.ReadLine();
                                parseQuestion(received);

                                
                                // Markup UI
                                answer_A.Enabled = true;
                                answer_B.Enabled = true;
                                answer_C.Enabled = true;

                                answer_A.BackColor = Color.Honeydew;
                                answer_B.BackColor = Color.Honeydew;
                                answer_C.BackColor = Color.Honeydew;

                                txt_question.Location = new Point(31, 334);
                                answer_A.Location = new Point(165, 384);
                                answer_B.Location = new Point(165, 420);
                                answer_C.Location = new Point(165, 456);
                                break;
                            
                            case "correct":
                                received = reader.ReadLine();
                                Console.WriteLine(received);
                              
                                score++;
                               
                                break;

                            case "incorrect":
                                received = reader.ReadLine();
                                Console.WriteLine(received);
                                break;
                            case "show answer":
                                
                                if (received == "A")
                                {
                                    answer_A.BackColor = Color.Green;
                                    if (answer == "A")
                                    {
                                        txt_score.Text = score.ToString();
                                    }
                                    
                                }
                                else if (received == "B")
                                {
                                    answer_B.BackColor = Color.Green;
                                    if (answer == "B")
                                    {
                                        txt_score.Text = score.ToString();
                                       
                                    }
                                  
                                }
                                else if (received == "C")
                                {
                                    answer_C.BackColor = Color.Green;
                                    if (answer == "C")
                                    {
                                        txt_score.Text = score.ToString();
                                       
                                    }
                                  
                                }
                                else
                                {
                                    Console.WriteLine("Dont A, B, C");
                                }

                                answer = "";
                                break;
                            case "show winner":
                                received = reader.ReadLine();
                                string cost = reader.ReadLine();
                                Console.WriteLine(received);
                                string s = received + "\n" + cost;
                                MessageBox.Show(s);
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
               //Tach luong

                this.Invoke(new Action(() =>
                {
                    timer1.Enabled = true;
                }));

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
        int seconds = 10;
        private void time1_Tick(object sender, EventArgs e) 
        {
            if (seconds <= 0) {
                timer1.Enabled = false;
                seconds = 10;
                lbCountDown.Text = "10";
                answer_A.Enabled = false;
                answer_B.Enabled = false;
                answer_C.Enabled = false;

                if (!flag)
                {
                    writer.WriteLine("answer");
                    writer.WriteLine(id);
                    writer.WriteLine("D");

                    Console.WriteLine("answer o timer");
                }
                flag = false;
            }
            Console.WriteLine("timer");
            lbCountDown.Text = seconds.ToString();
            seconds--;
        }
        private void timerEnable()
        {
            timer1.Enabled = true;
          
        }
    }
}
