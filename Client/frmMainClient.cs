﻿using System;
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

namespace Client
{
    public partial class frmMainClient : Form
    {
        #region GLOBAL
        static string answer = null;
        static bool isPlay = false;
        static bool isAnswer = false;
        static bool isDisconnect = false;
        static int score = 0;

        #endregion


        public frmMainClient()
        {
            InitializeComponent();
        }

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

        public void ConnectServer(string ip, int port)
        {
            Thread mainThread = new Thread(() =>
            {
                string msg  = string.Empty;
                string id   = null;

                try
                {
                    // connect
                    TcpClient client = new TcpClient();
                    IPAddress ipAddress = IPAddress.Parse(ip);
                    client.Connect(ipAddress, port);
                    txt_status.Text = string.Format("Connected to {0}:{1}", ip, port);
                    Console.WriteLine("Connect to {0}:{1} successfully", ip, port);

                    // read, write to server using stream, over use bytes[]
                    Stream streamer = client.GetStream();
                    StreamReader reader = new StreamReader(streamer);
                    StreamWriter writer = new StreamWriter(streamer);
                    writer.AutoFlush = true;

                    id = reader.ReadLine();
                    Console.WriteLine("My received id: {0}", id);

                    while (true)
                    {
                        // check disConnect button was clicked
                        if (isDisconnect) break;

                        // waiting for SERVER start game
                        msg = reader.ReadLine();
                        if (msg == "play")
                        {
                            isPlay = true;
                        }
                        if (!isPlay) continue;

                        if (msg == "question")
                        {
                            string received = string.Empty;
                            received = reader.ReadLine();
                            Console.WriteLine(received);
                            dynamic data = JObject.Parse(received);

                            // update to GUI
                            txt_question.Text = data.Question;
                            answer_A.Text = data.A;
                            answer_B.Text = data.B;
                            answer_C.Text = data.C;
                        }


                        // resume to user choose answer
                        if (!isAnswer) continue;

                        // wow !!!, okay
                        // send answer to server
                        writer.WriteLine("answer");
                        writer.WriteLine(id + "!" + answer);

                        // receive msg from server
                        msg = reader.ReadLine();
                        string[] info = msg.Split('!');

                        if (info.Length <= 1) continue;
                        if (info[0].Equals("1"))
                        {
                            score++;
                        }
                        txt_noti.Text = "Kết quả: " + info[1];
                        txt_score.Text = score.ToString();

                        // reset answer
                        isAnswer = false;
                        if (!isAnswer)
                        {
                            answer_A.Enabled = true;
                            answer_B.Enabled = true;
                            answer_C.Enabled = true;
                        }
                    }
                    streamer.Close();
                    client.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    throw;
                }
            });
            mainThread.Start();
        }

        private void btn_answer_Clicked(object sender, EventArgs e)
        {
            // get answer A, B, C from user
            answer = (sender as Button).Tag.ToString();
            Console.WriteLine("clicked {0}", answer);

            isAnswer = true;
            if (isAnswer)
            {
                answer_A.Enabled = false;
                answer_B.Enabled = false;
                answer_C.Enabled = false;
            }
        }

        private void btn_close_Click(object sender, EventArgs e)
        {
            // toggle button
            btn_connect.Enabled = true;
            btn_close.Enabled = false;

            isDisconnect = true;
        }
    }
}