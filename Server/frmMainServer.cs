﻿using System;
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
using Question;
using NAudio.Wave;
using NAudio.Wave.Compression;
using Codecs;
using Codecs.Codecs;
using Newtonsoft.Json.Linq;
using AForge.Video.DirectShow;


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

        private static List<int> _list_id = new List<int>();//Lưu số câu trả lời đúng cho mỗi client
        private static int _numberCorrectAnswer = 0;
        private static int cost;
        //volatile : Biến volatile thì thread nào cũng xử dụng dc
       
        // Audio record Properties
        private WaveIn waveIn;
        private UdpClient udpAudioSender;
        private INetworkChatCodec codec;
        private volatile bool connected;
        private List<INetworkChatCodec> Codecs;

        // Webcam record Properties
        private FilterInfoCollection webcam;
        private VideoCaptureDevice cam;
        private TcpListener tcpWebcamServer;
        private volatile bool webcamConnected;
        private volatile List<Socket> _listSocketWebcam = new List<Socket>();
        #endregion


        public frm_server()
        {
            InitializeComponent();

            // Audio
            Codecs = new List<INetworkChatCodec>();
            Codecs.Add(new G722ChatCodec());
            //Codecs.Add(new AcmMuLawChatCodec());
            //Codecs.Add(new Gsm610ChatCodec());
            //Codecs.Add(new MicrosoftAdpcmChatCodec());
            //Codecs.Add(new MuLawChatCodec());
            //Codecs.Add(new TrueSpeechChatCodec());
            //Codecs.Add(new UncompressedPcmChatCodec());

            //codec = new Codecs.Gsm610ChatCodec();
            GetAudioCodecsCombo(Codecs);
            GetAudioInputDevicesCombo();
            GetWebcamDevicesCombo();
           

            // list webcam size available
            //for (int i = 0; i < cam.VideoCapabilities.Length; i++)
            //{
            //    string resolution_size = cam.VideoCapabilities[i].FrameSize.ToString();
            //    Console.WriteLine(resolution_size);
            //}
        }

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
                comboBoxWebcams.Enabled = false;
                btn_play.Enabled = true;
                btn_open.Text = "Close";

                // Let's go
                StartServer(ip, port);
            } else
            {
                comboBoxCodecs.Enabled = true;
                comboBoxInputDevices.Enabled = true;
                comboBoxWebcams.Enabled = true;
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

                // audio
                this.codec.Dispose();
                udpAudioSender.Close();

                // question
                foreach (Socket client in _listSocket)
                {
                    client.Close();
                }

                // webcam
                //tcpWebcamServer.Server.Close();
                foreach (Socket client in _listSocketWebcam)
                {
                    client.Close();
                }

                server.Server.Close();
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
                if (client.Connected)
                {
                    using (StreamWriter writer = new StreamWriter(new NetworkStream(client)))
                    {
                        writer.WriteLine("play");
                    }
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

                float totalCost;
                int dem = 0;
                if (!float.TryParse(txt_cost.Text, out totalCost)) {
                    totalCost = 3000;
                }
                string winner = "";
                string winner_Cost = "";
                


                     
                for (int i = 0; i < _list_id.Count; i++)
                {
                    if (_list_id[i] == 10)
                    {
                        dem++;
                        winner += "  " + (i + 1).ToString();
                    }
                }
                if (dem > 0)
                {
                    string s  = "Người chiến thắng có id là :";
                    winner = s + winner;
                    float cost = totalCost / dem;
                   winner_Cost  = String.Format("$ ({0})", cost);
                    MessageBox.Show(winner+"\nsố tiền thưởng là : "+ winner_Cost, "Thông báo!");
                    Console.WriteLine(winner);
                    // message to all client for start game
                }
                else
                {
                    winner = "Rất tiếc chưa có ai chiến thắng!";
                }
                foreach (Socket client in _listSocket)
                {
                    if (client.Connected)
                    {
                        using (StreamWriter writer = new StreamWriter(new NetworkStream(client)))
                        {
                            writer.WriteLine("show winner");
                            writer.WriteLine(winner);
                            writer.WriteLine(winner_Cost);
                        }
                    }
                }
               
               
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
            if (comboBoxCodecs.Items.Count > 0)
            {
                comboBoxCodecs.SelectedIndex = 0;
            }
            else
            {
                comboBoxCodecs.Items.Add("No Codecs detect");
            }
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
            else
            {
                comboBoxInputDevices.Items.Add("No Audio Input detect");
            }
        }

        void waveIn_DataAvailable(object sender, WaveInEventArgs e)
        {
            byte[] encoded = codec.Encode(e.Buffer, 0, e.BytesRecorded);
            udpAudioSender.Send(encoded, encoded.Length);
           // Console.WriteLine("Send audio size {0}", encoded.Length);
        }
        #endregion

        #region Webcam
        void GetWebcamDevicesCombo()
        {
            // Webcam
            webcam = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            foreach (FilterInfo VideoCaptureDevice in webcam)
            {
                comboBoxWebcams.Items.Add(VideoCaptureDevice.Name);
            }
            if (comboBoxWebcams.Items.Count > 0)
            {
                comboBoxWebcams.SelectedIndex = 0;
            } else
            {
                //comboBoxWebcams.Items.Add("No Webcam detect");
            }
        }

        void showMyCam(object sender, AForge.Video.NewFrameEventArgs eventArgs)
        {
            try
            {
                // Detect cross threading
                this.Invoke((MethodInvoker)delegate
                {
                    Bitmap bit = (Bitmap)eventArgs.Frame.Clone();
                    pictureBoxStreamer.Image = bit;

                    SendImageToClients(bit);
                });
            } 
            catch (Exception ex)
            {
                Console.WriteLine("Webcam was close !!!");
                return;
            }
        }

        public void SendImageToClients(Image img = null)
        {
            Bitmap bImage = new Bitmap(img);
            byte[] bStream = ImageToByte(bImage);

            if (webcamConnected && bStream.Length > 0)
            {
                try
                {
                    // message to all client
                    foreach (Socket client in _listSocketWebcam)
                    {
                        if (client.Connected)
                        {
                            int sizeSent = SendChunkData(client, bStream);
                            
                            //client.Send(bStream, bStream.Length, SocketFlags.None);
                            //Console.WriteLine("send webcam size {0}", bStream.Length);
                            
                            //Console.WriteLine("send webcam size {0}", sizeSent);
                        }
                    }
                }
                catch (SocketException ex)
                {
                    Console.WriteLine("Client was disconnected");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
        private static int SendChunkData(Socket s, byte[] data)
        {
            int total = 0;
            int size = data.Length;
            int dataleft = size;
            int sent;

            byte[] datasize = new byte[4];
            datasize = BitConverter.GetBytes(size);
            sent = s.Send(datasize);

            while (total < size)
            {
                sent = s.Send(data, total, dataleft, SocketFlags.None);
                total += sent;
                dataleft -= sent;
            }
            return total;
        }
        private byte[] ImageToByte(Image img)
        {
            MemoryStream mMemoryStream = new MemoryStream();
            img.Save(mMemoryStream, System.Drawing.Imaging.ImageFormat.Gif);
            return mMemoryStream.ToArray();
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
            waveIn = new WaveIn();//Tạo object để làm việc với âm thanh
            int inputDeviceNumber = comboBoxInputDevices.SelectedIndex;//Lấy số index của devices được chọn
            this.codec = ((CodecComboItem)comboBoxCodecs.SelectedItem).Codec;//Chứa chuẩn âm thanh ( đưa âm thanh về byte)
            waveIn.BufferMilliseconds = 50;
            waveIn.DeviceNumber = inputDeviceNumber;
            waveIn.WaveFormat = codec.RecordFormat;
            waveIn.DataAvailable += waveIn_DataAvailable;
            waveIn.StartRecording();
            
            // Start Webcam 
            cam = new VideoCaptureDevice(webcam[comboBoxWebcams.SelectedIndex].MonikerString);
            cam.VideoResolution = cam.VideoCapabilities[0]; // 640 x 480
            cam.NewFrame += new AForge.Video.NewFrameEventHandler(showMyCam);
            cam.Start();

            // Open UDP connect for Audio sending
            udpAudioSender = new UdpClient();
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Broadcast, port);
            udpAudioSender.Connect(endPoint);
            Console.WriteLine("Opened UDP for Audio at broadcast {0}:{1}", IPAddress.Broadcast, port);

            // Open TCP server for question, create new thread for non-block UI 
            Thread mainThread = new Thread(() =>
            {
                try
                {
                    IPAddress ipAddress = IPAddress.Parse(ip);//Vì tcp chỉ nhận IPAddress nên phải parse ip về IPaddress
                    server = new TcpListener(ipAddress, port);
                    server.Start();
                    connected = true;
                    _isDisconnect = false;
                    Console.WriteLine("Opened TCP server for Question at {0}:{1}", ip, port);

                    // Open Tcp for Webcam streaming
                    if (comboBoxWebcams.Items.Count >= 1)
                    {
                        tcpWebcamServer = new TcpListener(IPAddress.Parse(ip), port + 1);
                        tcpWebcamServer.Start();
                        webcamConnected = true;
                        Console.WriteLine("Opened TCP for Webcam at {0}:{1}", ip, port + 1);
                    }

                    // Listen from client
                    // open a new Thread if a Client connect
                    while (_numberConnecting < MAX_CONNECT)//Số connecting hiện tại <100
                    {
                        Socket acceptSocket = server.AcceptSocket();//Tạo ra 1 socket để làm việc với client
                        Socket webcamSocket = tcpWebcamServer.AcceptSocket();

                        // Add all socket connected to control
                        _listSocketWebcam.Add(webcamSocket);
                        _listSocket.Add(acceptSocket);

                        _numberConnecting++;

                        this.Invoke(new Action(() =>
                        {
                            txt_numberConnect.Text = _numberConnecting.ToString(); // update UI
                        }));

                        if (_isDisconnect)
                        {
                            break;
                        }

                        // ready for communications
                        //Khi 1 client xác nhận connected thì tạo ra 1 luồng cho nó
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
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Server was close !!");
                    return;
                }

            });
            mainThread.Start();
        }


        private void ConnectClient(Socket client)
        {
            // okay, a client was connect
            // receive data from client for valid
             string answer = string.Empty;
             string msg = string.Empty;
             string idUser = string.Empty;
             NetworkStream streamer = null;
             StreamReader reader = null;
             StreamWriter writer = null;

            try
            {
                // read, write from client using stream, over use bytes[]
                 streamer = new NetworkStream(client);
                 reader = new StreamReader(streamer);//Kiểu streamreader : để server nhận đọc dữ liệu của client
                 writer = new StreamWriter(streamer);//Ghi dl

                // should flush buffer stream auto 
                writer.AutoFlush = true;
                Console.WriteLine("<<ID: {0}>> New connect from {1}", _numberConnecting, client.RemoteEndPoint);

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
                            _list_id.Add(0);
                            break;

                        case "question":
                            writer.WriteLine(_numberQuestion);
                            writer.WriteLine(_jsonQuestion);
                            break;

                        case "answer":
                            Console.WriteLine("==============>");
                            idUser = reader.ReadLine();
                            answer = reader.ReadLine();

                            Console.WriteLine("{0} answer {1}", idUser, answer);

                            // check awnser
                            if (CheckAwnserForQuestion(answer))
                            {
                                writer.WriteLine("correct");
                                writer.WriteLine(this._correctAnswer);
                                Console.WriteLine("Correct answer {0}: ", this._correctAnswer);
                                int i = int.Parse(idUser);
                                _list_id[i - 1]++;
                                _numberCorrectAnswer += 1;
                            }
                            else
                            {
                                writer.WriteLine("incorrect");
                                 writer.WriteLine(this._correctAnswer);
                                Console.WriteLine("Correct answer {0}: ", this._correctAnswer);
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
            catch(SocketException ex)
            {
                if (client.Connected)
                {
                    Console.WriteLine("Disconnected from {0}", client.RemoteEndPoint);
                }
            }
            catch (Exception ex)
            {
                if (client.Connected)
                {
                    Console.WriteLine("Disconnected from {0}", client.RemoteEndPoint);
                }
            }
            // Client was disconnect, should close.
            //Console.WriteLine("Disconnected from {0}", client.RemoteEndPoint);
            if (client.Connected)
            {
                Console.WriteLine("Disconnected from {0}", client.RemoteEndPoint);

                foreach (Socket webcamSocket in _listSocketWebcam)
                {
                    if (webcamSocket.RemoteEndPoint == client.RemoteEndPoint)
                    {
                        webcamSocket.Close();
                    }
                }
                client.Close();
            }
        }

        private bool CheckAwnserForQuestion(string answer)
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
                if (client.Connected)
                {
                    using (StreamWriter writer = new StreamWriter(new NetworkStream(client)))
                    {
                        writer.WriteLine("new question");
                        writer.WriteLine(numberQuestion);
                        writer.WriteLine(@jsonQuestion);
                    }
                }
            }
        }

        private void btnShowAnswer_Click(object sender, EventArgs e)
        {
            showAnswer();
        }
        private void showAnswer()
        {
            // valid running in game loop
            if (!_isPlay) return;

            // message to all client for start game
            foreach (Socket client in _listSocket)
            {
                if (client.Connected)
                {
                    using (StreamWriter writer = new StreamWriter(new NetworkStream(client)))
                    {
                        writer.WriteLine("show answer");
                    }
                }
            }
            lbCorrectAnswer.Text = _numberCorrectAnswer.ToString();
            _numberCorrectAnswer = 0;
        }
    }
}
