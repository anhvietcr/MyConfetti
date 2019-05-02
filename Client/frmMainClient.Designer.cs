namespace Client
{
    partial class frmMainClient
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.txt_port = new System.Windows.Forms.TextBox();
            this.btn_connect = new System.Windows.Forms.Button();
            this.txt_ip = new System.Windows.Forms.TextBox();
            this.answer_A = new System.Windows.Forms.Button();
            this.groupBox_question = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.answer_D = new System.Windows.Forms.Button();
            this.answer_B = new System.Windows.Forms.Button();
            this.answer_C = new System.Windows.Forms.Button();
            this.txt_noti = new System.Windows.Forms.Label();
            this.btn_close = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.txt_score = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.groupBox_webcam = new System.Windows.Forms.GroupBox();
            this.groupBox_question.SuspendLayout();
            this.SuspendLayout();
            // 
            // txt_port
            // 
            this.txt_port.Location = new System.Drawing.Point(90, 537);
            this.txt_port.Name = "txt_port";
            this.txt_port.Size = new System.Drawing.Size(41, 20);
            this.txt_port.TabIndex = 7;
            this.txt_port.Text = "6969";
            // 
            // btn_connect
            // 
            this.btn_connect.Location = new System.Drawing.Point(137, 535);
            this.btn_connect.Name = "btn_connect";
            this.btn_connect.Size = new System.Drawing.Size(75, 23);
            this.btn_connect.TabIndex = 5;
            this.btn_connect.Text = "Connect";
            this.btn_connect.UseVisualStyleBackColor = true;
            this.btn_connect.Click += new System.EventHandler(this.btn_connect_Click);
            // 
            // txt_ip
            // 
            this.txt_ip.Location = new System.Drawing.Point(12, 537);
            this.txt_ip.Name = "txt_ip";
            this.txt_ip.Size = new System.Drawing.Size(72, 20);
            this.txt_ip.TabIndex = 4;
            this.txt_ip.Text = "127.0.0.1";
            // 
            // answer_A
            // 
            this.answer_A.Location = new System.Drawing.Point(6, 74);
            this.answer_A.Name = "answer_A";
            this.answer_A.Size = new System.Drawing.Size(34, 23);
            this.answer_A.TabIndex = 8;
            this.answer_A.Text = "A";
            this.answer_A.UseVisualStyleBackColor = true;
            this.answer_A.Click += new System.EventHandler(this.btn_answer_Clicked);
            // 
            // groupBox_question
            // 
            this.groupBox_question.Controls.Add(this.label5);
            this.groupBox_question.Controls.Add(this.label4);
            this.groupBox_question.Controls.Add(this.label3);
            this.groupBox_question.Controls.Add(this.label2);
            this.groupBox_question.Controls.Add(this.label1);
            this.groupBox_question.Controls.Add(this.answer_D);
            this.groupBox_question.Controls.Add(this.answer_B);
            this.groupBox_question.Controls.Add(this.answer_C);
            this.groupBox_question.Controls.Add(this.answer_A);
            this.groupBox_question.Location = new System.Drawing.Point(12, 348);
            this.groupBox_question.Name = "groupBox_question";
            this.groupBox_question.Size = new System.Drawing.Size(629, 172);
            this.groupBox_question.TabIndex = 9;
            this.groupBox_question.TabStop = false;
            this.groupBox_question.Text = "Câu hỏi";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(407, 130);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(65, 13);
            this.label5.TabIndex = 10;
            this.label5.Text = "Câu trả lời D";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(407, 74);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(64, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "Câu trả lời B";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(46, 130);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(64, 13);
            this.label3.TabIndex = 10;
            this.label3.Text = "Câu trả lời C";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(46, 74);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(64, 13);
            this.label2.TabIndex = 10;
            this.label2.Text = "Câu trả lời A";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(88, 13);
            this.label1.TabIndex = 9;
            this.label1.Text = "Nội dung câu hỏi";
            // 
            // answer_D
            // 
            this.answer_D.Location = new System.Drawing.Point(367, 130);
            this.answer_D.Name = "answer_D";
            this.answer_D.Size = new System.Drawing.Size(34, 23);
            this.answer_D.TabIndex = 8;
            this.answer_D.Text = "D";
            this.answer_D.UseVisualStyleBackColor = true;
            this.answer_D.Click += new System.EventHandler(this.btn_answer_Clicked);
            // 
            // answer_B
            // 
            this.answer_B.Location = new System.Drawing.Point(367, 74);
            this.answer_B.Name = "answer_B";
            this.answer_B.Size = new System.Drawing.Size(34, 23);
            this.answer_B.TabIndex = 8;
            this.answer_B.Text = "B";
            this.answer_B.UseVisualStyleBackColor = true;
            this.answer_B.Click += new System.EventHandler(this.btn_answer_Clicked);
            // 
            // answer_C
            // 
            this.answer_C.Location = new System.Drawing.Point(6, 130);
            this.answer_C.Name = "answer_C";
            this.answer_C.Size = new System.Drawing.Size(34, 23);
            this.answer_C.TabIndex = 8;
            this.answer_C.Text = "C";
            this.answer_C.UseVisualStyleBackColor = true;
            this.answer_C.Click += new System.EventHandler(this.btn_answer_Clicked);
            // 
            // txt_noti
            // 
            this.txt_noti.AutoSize = true;
            this.txt_noti.Font = new System.Drawing.Font("Courier New", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt_noti.Location = new System.Drawing.Point(12, 5);
            this.txt_noti.Name = "txt_noti";
            this.txt_noti.Size = new System.Drawing.Size(43, 22);
            this.txt_noti.TabIndex = 10;
            this.txt_noti.Text = "...";
            // 
            // btn_close
            // 
            this.btn_close.Enabled = false;
            this.btn_close.Location = new System.Drawing.Point(218, 535);
            this.btn_close.Name = "btn_close";
            this.btn_close.Size = new System.Drawing.Size(75, 23);
            this.btn_close.TabIndex = 11;
            this.btn_close.Text = "Close";
            this.btn_close.UseVisualStyleBackColor = true;
            this.btn_close.Click += new System.EventHandler(this.btn_close_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(461, 9);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(112, 16);
            this.label6.TabIndex = 12;
            this.label6.Text = "Trả lời đúng:";
            // 
            // txt_score
            // 
            this.txt_score.AutoSize = true;
            this.txt_score.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt_score.Location = new System.Drawing.Point(579, 9);
            this.txt_score.Name = "txt_score";
            this.txt_score.Size = new System.Drawing.Size(16, 16);
            this.txt_score.TabIndex = 13;
            this.txt_score.Text = "0";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(601, 9);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(40, 16);
            this.label8.TabIndex = 14;
            this.label8.Text = "/ 10";
            // 
            // groupBox_webcam
            // 
            this.groupBox_webcam.Location = new System.Drawing.Point(12, 34);
            this.groupBox_webcam.Name = "groupBox_webcam";
            this.groupBox_webcam.Size = new System.Drawing.Size(629, 308);
            this.groupBox_webcam.TabIndex = 15;
            this.groupBox_webcam.TabStop = false;
            this.groupBox_webcam.Text = "Streamer";
            // 
            // frmMainClient
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(650, 562);
            this.Controls.Add(this.groupBox_webcam);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.txt_score);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.btn_close);
            this.Controls.Add(this.txt_noti);
            this.Controls.Add(this.groupBox_question);
            this.Controls.Add(this.txt_port);
            this.Controls.Add(this.btn_connect);
            this.Controls.Add(this.txt_ip);
            this.Name = "frmMainClient";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Client MyConfetti";
            this.groupBox_question.ResumeLayout(false);
            this.groupBox_question.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txt_port;
        private System.Windows.Forms.Button btn_connect;
        private System.Windows.Forms.TextBox txt_ip;
        private System.Windows.Forms.Button answer_A;
        private System.Windows.Forms.GroupBox groupBox_question;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button answer_D;
        private System.Windows.Forms.Button answer_B;
        private System.Windows.Forms.Button answer_C;
        private System.Windows.Forms.Label txt_noti;
        private System.Windows.Forms.Button btn_close;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label txt_score;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.GroupBox groupBox_webcam;
    }
}

