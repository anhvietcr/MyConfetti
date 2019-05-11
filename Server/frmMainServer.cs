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
using Newtonsoft.Json.Linq;



/*
 * 
    How to send / receive livestream webcam from server to clients ? 
    1) Capturing raw video from camera and audio from microphone.
    2) Compressing the video and audio.
    3) Transmitting and receiving video and audio.
    4) Decompressing the video and audio.
    5) Displaying the video and playing the audio.
*
* *
* For TCP: Connect (one-one)
* For UDP: Broadcast (one-many) & Non-Broadcast (one-one)
*/


namespace Server
{
    public partial class frm_server : Form
    {
        #region GLOBAL
        private static int tick                             = 0;
        private Thread mainThread                           = null;
        private string[] _questions                         = null;
        private const int MAX_CONNECT                       = 100;
        private static TcpListener server                   = null;
        private bool _isPlay { get; set; }                  = false;
        private bool _isExit { get; set; }                  = false;
        private volatile bool _isDisconnect                 = false;
        private volatile int _numberQuestion                = 0;
        private static int _numberConnecting                = 0;
        private volatile List<Socket> _listSocket           = new List<Socket>();
        private string _jsonQuestion { get; set; }          = string.Empty;
        private string _correctAnswer { get; set; }         = string.Empty;
        private static bool _isNewQuestion { get; set; }    = false;

        // Audio record Properties
        private WaveIn waveIn;
        private UdpClient udpSender;
        private INetworkChatCodec codec;
        private volatile bool connected;
        private List<INetworkChatCodec> Codecs;
        #endregion


        public frm_server()
        {
            InitializeComponent();

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
            GetAudioInputDevicesCombo();
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
                _isDisconnect = true;

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
            if (_numberConnecting < 1)
            {
                MessageBox.Show("Chưa có người tham gia !");
                return;
            }

            if (string.IsNullOrEmpty(txtBoxFileName.Text) || _questions == null)
            {
                MessageBox.Show("Chưa chọn file chứa câu hỏi !");
                return;
            }

            _isPlay = true;
            btn_play.Enabled = false;
            btnNext.Enabled = true;

            // message to all client for start game
            foreach (Socket client in _listSocket)
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

            if (_questions.Length <= 0)
            {
                MessageBox.Show("Chưa chọn file chứa câu hỏi !");
                return;
            }

            if (_numberQuestion >= _questions.Length)
            {
                MessageBox.Show("Hết câu hỏi !");
                _numberQuestion = 0;
                btn_play.Enabled = true;
                btnNext.Enabled = false;
                txtBoxFileName.Text = string.Empty;
                return;
            }

            // Get question
            _jsonQuestion = _questions[_numberQuestion];

            // Set correct answer
            dynamic data = JObject.Parse(@_jsonQuestion);
            _correctAnswer = data.answer;
            
            // Send to Clients
            sendQuestion(_jsonQuestion, _numberQuestion);
            _numberQuestion++;
        }

        private void btnChoose_Click(object sender, EventArgs e)
        {
            if (openFileDialogQuestion.ShowDialog() == DialogResult.OK)
            {
                txtBoxFileName.Text = openFileDialogQuestion.FileName;

                // Get datas question[]
                Data read = new Data();
                _questions = read.readFile(txtBoxFileName.Text);
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
            //Console.WriteLine("{1} send audio size {0}", encoded.Length, i++);
        }
        #endregion


        /**
         * 
         * Start SERVER 
         * 
         **/
        public void StartServer(string ip, int port)
        {
            // Recoding Audio
            waveIn = new WaveIn();
            int inputDeviceNumber = comboBoxInputDevices.SelectedIndex;
            this.codec = ((CodecComboItem)comboBoxCodecs.SelectedItem).Codec;
            waveIn.BufferMilliseconds = 50;
            waveIn.DeviceNumber = inputDeviceNumber;
            waveIn.WaveFormat = codec.RecordFormat;
            waveIn.DataAvailable += waveIn_DataAvailable;
            waveIn.StartRecording();
            
            // Open UDP connect for Audio sending
            udpSender = new UdpClient();
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Broadcast, port);
            udpSender.Connect(endPoint);
            Console.WriteLine("Opened UDP for Audio at broadcast {0}:{1}", IPAddress.Broadcast, port);

            
            // Open TCP server for question, create new thread for non-block UI 
            Thread mainThread = new Thread(() =>
            {
                IPAddress ipAddress = IPAddress.Parse(ip);
                server = new TcpListener(ipAddress, port);
                server.Start();
                Console.WriteLine("Opened TCP server for Question at {0}:{1}", ip, port);

                // Listen from client
                // open a new Thread if a Client connect
                while (_numberConnecting < MAX_CONNECT)
                {
                    Socket acceptSocket = server.AcceptSocket();
                    _listSocket.Add(acceptSocket);
                    _numberConnecting++;
                    txt_numberConnect.Text = _numberConnecting.ToString(); // update UI

                    if (_isDisconnect)
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
                _isDisconnect = false;
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
                Console.WriteLine("<<ID: {0}>> New connect from {1}", _numberConnecting, client.RemoteEndPoint);


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
                            writer.WriteLine(_numberConnecting);
                            writer.WriteLine(_isPlay);
                            break;

                        case "question":
                            writer.WriteLine(_numberQuestion);
                            writer.WriteLine(_jsonQuestion);
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

                    if (_isDisconnect)
                    {
                        mainThread.Abort();
                        server.Server.Close();
                        Console.WriteLine("Server was closed from client send");
                        _isDisconnect = false;
                        break;
                    }

                    // client send exit status
                    if (_isExit) break;
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
            if (answer == this._correctAnswer)
                return true;
            return false;
        }

        private void sendQuestion(string jsonQuestion, int numberQuestion)
        {
            // valid running in game loop
            if (!_isPlay) return;

            // message to all client for start game
            foreach (Socket client in _listSocket)
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
            if (!_isPlay) return;

            if (_numberQuestion >= 10)
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
            //    _isNewQuestion = true;
            //    tick = 0;
            //    ++_numberQuestion;

            //    _jsonQuestion = getQuestion(_numberQuestion);
            //    sendQuestion(_jsonQuestion);
            //}
        }
    }
}
