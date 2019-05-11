namespace Server
{
    partial class frm_server
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
            this.txt_ip = new System.Windows.Forms.TextBox();
            this.btn_open = new System.Windows.Forms.Button();
            this.txt_port = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btn_play = new System.Windows.Forms.Button();
            this.txt_numberConnect = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btnNext = new System.Windows.Forms.Button();
            this.btnChoose = new System.Windows.Forms.Button();
            this.txtBoxFileName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.openFileDialogQuestion = new System.Windows.Forms.OpenFileDialog();
            this.comboBoxCodecs = new System.Windows.Forms.ComboBox();
            this.comboBoxInputDevices = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.comboBoxWebcams = new System.Windows.Forms.ComboBox();
            this.pictureBoxStreamer = new System.Windows.Forms.PictureBox();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxStreamer)).BeginInit();
            this.SuspendLayout();
            // 
            // txt_ip
            // 
            this.txt_ip.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt_ip.Location = new System.Drawing.Point(16, 594);
            this.txt_ip.Name = "txt_ip";
            this.txt_ip.Size = new System.Drawing.Size(107, 24);
            this.txt_ip.TabIndex = 0;
            this.txt_ip.Text = "127.0.0.1";
            // 
            // btn_open
            // 
            this.btn_open.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_open.Location = new System.Drawing.Point(184, 592);
            this.btn_open.Name = "btn_open";
            this.btn_open.Size = new System.Drawing.Size(80, 28);
            this.btn_open.TabIndex = 1;
            this.btn_open.Text = "Open";
            this.btn_open.UseVisualStyleBackColor = true;
            this.btn_open.Click += new System.EventHandler(this.btn_open_Click);
            // 
            // txt_port
            // 
            this.txt_port.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt_port.Location = new System.Drawing.Point(129, 594);
            this.txt_port.Name = "txt_port";
            this.txt_port.Size = new System.Drawing.Size(49, 24);
            this.txt_port.TabIndex = 3;
            this.txt_port.Text = "6969";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.pictureBoxStreamer);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(12, 58);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(652, 511);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Streamer";
            // 
            // btn_play
            // 
            this.btn_play.Enabled = false;
            this.btn_play.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_play.Location = new System.Drawing.Point(358, 594);
            this.btn_play.Name = "btn_play";
            this.btn_play.Size = new System.Drawing.Size(154, 63);
            this.btn_play.TabIndex = 1;
            this.btn_play.Text = "Play Game >>";
            this.btn_play.UseVisualStyleBackColor = true;
            this.btn_play.Click += new System.EventHandler(this.btn_play_Click);
            // 
            // txt_numberConnect
            // 
            this.txt_numberConnect.AutoSize = true;
            this.txt_numberConnect.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt_numberConnect.Location = new System.Drawing.Point(652, 10);
            this.txt_numberConnect.Name = "txt_numberConnect";
            this.txt_numberConnect.Size = new System.Drawing.Size(16, 18);
            this.txt_numberConnect.TabIndex = 5;
            this.txt_numberConnect.Text = "0";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(562, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(73, 18);
            this.label1.TabIndex = 6;
            this.label1.Text = "Tham gia:";
            // 
            // btnNext
            // 
            this.btnNext.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnNext.Enabled = false;
            this.btnNext.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnNext.Location = new System.Drawing.Point(524, 594);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(140, 63);
            this.btnNext.TabIndex = 10;
            this.btnNext.Text = "Câu hỏi mới";
            this.btnNext.UseVisualStyleBackColor = true;
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // btnChoose
            // 
            this.btnChoose.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnChoose.Location = new System.Drawing.Point(321, 633);
            this.btnChoose.Name = "btnChoose";
            this.btnChoose.Size = new System.Drawing.Size(31, 24);
            this.btnChoose.TabIndex = 9;
            this.btnChoose.Text = "...";
            this.btnChoose.UseVisualStyleBackColor = true;
            this.btnChoose.Click += new System.EventHandler(this.btnChoose_Click);
            // 
            // txtBoxFileName
            // 
            this.txtBoxFileName.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtBoxFileName.Location = new System.Drawing.Point(92, 633);
            this.txtBoxFileName.Name = "txtBoxFileName";
            this.txtBoxFileName.Size = new System.Drawing.Size(260, 24);
            this.txtBoxFileName.TabIndex = 8;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(16, 636);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(63, 18);
            this.label2.TabIndex = 7;
            this.label2.Text = "Câu hỏi:";
            // 
            // openFileDialogQuestion
            // 
            this.openFileDialogQuestion.FileName = "openFileDialog1";
            // 
            // comboBoxCodecs
            // 
            this.comboBoxCodecs.FormattingEnabled = true;
            this.comboBoxCodecs.Location = new System.Drawing.Point(203, 30);
            this.comboBoxCodecs.Name = "comboBoxCodecs";
            this.comboBoxCodecs.Size = new System.Drawing.Size(154, 21);
            this.comboBoxCodecs.TabIndex = 11;
            // 
            // comboBoxInputDevices
            // 
            this.comboBoxInputDevices.FormattingEnabled = true;
            this.comboBoxInputDevices.Location = new System.Drawing.Point(15, 30);
            this.comboBoxInputDevices.Name = "comboBoxInputDevices";
            this.comboBoxInputDevices.Size = new System.Drawing.Size(159, 21);
            this.comboBoxInputDevices.TabIndex = 11;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(12, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(49, 18);
            this.label3.TabIndex = 6;
            this.label3.Text = "Audio:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(200, 9);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(64, 18);
            this.label4.TabIndex = 6;
            this.label4.Text = "Codecs:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(375, 10);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(72, 18);
            this.label5.TabIndex = 6;
            this.label5.Text = "Webcam:";
            // 
            // comboBoxWebcams
            // 
            this.comboBoxWebcams.FormattingEnabled = true;
            this.comboBoxWebcams.Location = new System.Drawing.Point(378, 31);
            this.comboBoxWebcams.Name = "comboBoxWebcams";
            this.comboBoxWebcams.Size = new System.Drawing.Size(152, 21);
            this.comboBoxWebcams.TabIndex = 11;
            // 
            // pictureBoxStreamer
            // 
            this.pictureBoxStreamer.Location = new System.Drawing.Point(6, 23);
            this.pictureBoxStreamer.Name = "pictureBoxStreamer";
            this.pictureBoxStreamer.Size = new System.Drawing.Size(640, 480);
            this.pictureBoxStreamer.TabIndex = 0;
            this.pictureBoxStreamer.TabStop = false;
            // 
            // frm_server
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(677, 664);
            this.Controls.Add(this.comboBoxInputDevices);
            this.Controls.Add(this.comboBoxWebcams);
            this.Controls.Add(this.comboBoxCodecs);
            this.Controls.Add(this.btnNext);
            this.Controls.Add(this.btnChoose);
            this.Controls.Add(this.txtBoxFileName);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txt_numberConnect);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.txt_port);
            this.Controls.Add(this.btn_play);
            this.Controls.Add(this.btn_open);
            this.Controls.Add(this.txt_ip);
            this.MaximumSize = new System.Drawing.Size(693, 703);
            this.MinimumSize = new System.Drawing.Size(693, 703);
            this.Name = "frm_server";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Server MyConfetti";
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxStreamer)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txt_ip;
        private System.Windows.Forms.Button btn_open;
        private System.Windows.Forms.TextBox txt_port;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btn_play;
        private System.Windows.Forms.Label txt_numberConnect;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnNext;
        private System.Windows.Forms.Button btnChoose;
        private System.Windows.Forms.TextBox txtBoxFileName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.OpenFileDialog openFileDialogQuestion;
        private System.Windows.Forms.ComboBox comboBoxCodecs;
        private System.Windows.Forms.ComboBox comboBoxInputDevices;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox comboBoxWebcams;
        private System.Windows.Forms.PictureBox pictureBoxStreamer;
    }
}

