using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
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
using NAudio.Wave;
using NAudio.Wave.Compression;
using Codecs;
using Codecs.Codecs;


/*
 * 
    How to send / receive livestream webcam from server to clients ? 
    1) Capturing raw video from camera and audio from microphone.
    2) Compressing the video and audio.
    3) Transmitting and receiving video and audio.
    4) Decompressing the video and audio.
    5) Displaying the video and playing the audio.
*
*/


namespace Server
{
    public partial class frm_server : Form
    {
        #region GLOBAL

        const int MAX_CONNECT       = 100;
        static int numberConnecting = 0;
        static TcpListener server   = null;
        Thread mainThread           = null;
        private volatile bool isDisconnect    = false;
        private volatile List<Socket> listSocket = new List<Socket>();
        private bool isPlay { get; set; }
        private bool isExit { get; set; } = false;
        static bool isNewQuestion { get; set; } = false;
        static int tick = 0;
        private volatile int numberQuestion = 0;
        string[] questions = null;
        string jsonQuestion { get; set; } = string.Empty;
        private System.Windows.Forms.Timer tmr;


        // Audio record Properties
        //public IEnumerable<INetworkChatCodec> Codecs { get; set; }
        [ImportMany(typeof(INetworkChatCodec))]
        private List<INetworkChatCodec> Codecs;
        private WaveIn waveIn;
        private UdpClient udpSender;
        private INetworkChatCodec codec;
        private volatile bool connected;

        #endregion


        public frm_server()
        {
            InitializeComponent();
            GetAudioInputDevicesCombo();

            Codecs = new List<INetworkChatCodec>();
            Codecs.Add(new AcmMuLawChatCodec());
            Codecs.Add(new G722ChatCodec());
            Codecs.Add(new Gsm610ChatCodec());
            Codecs.Add(new MicrosoftAdpcmChatCodec());
            Codecs.Add(new MuLawChatCodec());
            Codecs.Add(new TrueSpeechChatCodec());
            Codecs.Add(new UncompressedPcmChatCodec());

            //codec = new Codecs.Gsm610ChatCodec();
            GetAudioCodecsCombo(Codecs);
        }


        /**
         * 
         * Buttons Event
         * 
         **/
        #region Buttons event
        private void btn_open_Click(object sender, EventArgs e)
        {
            if (!connected)
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
                comboBoxCodecs.Enabled = false;
                comboBoxInputDevices.Enabled = false;
                btn_play.Enabled = true;
                btn_open.Text = "Close";

                // Let's go
                StartServer(ip, port);
            } else
            {
                comboBoxCodecs.Enabled = true;
                comboBoxInputDevices.Enabled = true;
                btn_play.Enabled = false;
                btn_open.Text = "Open";
                CloseListener();
            }
        }

        private void CloseListener()
        {
            if (connected)
            {
                connected = false;
                isDisconnect = true;

                waveIn.DataAvailable -= waveIn_DataAvailable;
                waveIn.StopRecording();
                server.Stop();
                waveIn.Dispose();

                //waveOut.Stop();
                //udpSender.Close();
                //udpListener.Close();
                //waveOut.Dispose();

                this.codec.Dispose(); // a bit naughty but we have designed the codecs to support multiple calls to Dispose, recreating their resources if Encode/Decode called again
            }
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
        #endregion


        #region Audio
        class CodecComboItem
        {
            public string Text { get; set; }
            public INetworkChatCodec Codec { get; set; }
            public override string ToString()
            {
                return Text;
            }
        }
        class ListenerThreadState
        {
            public IPEndPoint EndPoint { get; set; }
            public INetworkChatCodec Codec { get; set; }
        }

        // List all codecs was prepare
        private void GetAudioCodecsCombo(List<INetworkChatCodec> codecs)
        {
            var sorted = from codec in codecs
                         where codec.IsAvailable
                         orderby codec.BitsPerSecond ascending
                         select codec;

            foreach (var codec in sorted)
            {
                string bitRate = codec.BitsPerSecond == -1 ? "VBR" : String.Format("{0:0.#}kbps", codec.BitsPerSecond / 1000.0);
                string text = String.Format("{0} ({1})", codec.Name, bitRate);
                this.comboBoxCodecs.Items.Add(new CodecComboItem() { Text = text, Codec = codec });
            }
            this.comboBoxCodecs.SelectedIndex = 0;
        }

        // List all microphone devices
        private void GetAudioInputDevicesCombo()
        {
            for (int n = 0; n < WaveIn.DeviceCount; n++)
            {
                var capabilities = WaveIn.GetCapabilities(n);
                this.comboBoxInputDevices.Items.Add(capabilities.ProductName);
            }
            if (comboBoxInputDevices.Items.Count > 0)
            {
                comboBoxInputDevices.SelectedIndex = 0;
            }
        }

        int i = 0;
        void waveIn_DataAvailable(object sender, WaveInEventArgs e)
        {
            byte[] encoded = codec.Encode(e.Buffer, 0, e.BytesRecorded);
            udpSender.Send(encoded, encoded.Length);
            Console.WriteLine("{1} send audio size {0}", encoded.Length, i++);

            //foreach (Socket client in listSocket)
            //{
            //    using (StreamWriter writer = new StreamWriter(new NetworkStream(client)))
            //    {
            //        client.Send(encoded, encoded.Length, SocketFlags.None);
            //        //writer.WriteLine(encoded);
            //        //writer.Flush();
            //    }
            //}

            //foreach (Socket client in listSocket)
            //{
            //    client.Send(encoded, encoded.Length, SocketFlags.None);
            //}
        }
        #endregion


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

            // Recoding Audio
            waveIn = new WaveIn();
            int inputDeviceNumber = comboBoxInputDevices.SelectedIndex;
            this.codec = ((CodecComboItem)comboBoxCodecs.SelectedItem).Codec;
            waveIn.BufferMilliseconds = 50;
            waveIn.DeviceNumber = inputDeviceNumber;
            waveIn.WaveFormat = codec.RecordFormat;
            waveIn.DataAvailable += waveIn_DataAvailable;
            waveIn.StartRecording();


            udpSender = new UdpClient();
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(txt_ip.Text), int.Parse(txt_port.Text));
            udpSender.Connect(endPoint);


            Thread mainThread = new Thread(() =>
            {
                server = new TcpListener(ipAddress, port);
                server.Start();
                connected = true;

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

            int questionID = 1;
            string answer = string.Empty;
            string msg = string.Empty;
            string idUser = string.Empty;

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

        private void sendQuestion(string jsonQuestion, int numberQuestion)
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
        private void timer_Elapsed(object sender, ElapsedEventArgs e)
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
