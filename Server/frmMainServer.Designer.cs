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
            this.btn_close = new System.Windows.Forms.Button();
            this.txt_port = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btn_play = new System.Windows.Forms.Button();
            this.txt_numberConnect = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // txt_ip
            // 
            this.txt_ip.Location = new System.Drawing.Point(12, 372);
            this.txt_ip.Name = "txt_ip";
            this.txt_ip.Size = new System.Drawing.Size(72, 20);
            this.txt_ip.TabIndex = 0;
            this.txt_ip.Text = "127.0.0.1";
            // 
            // btn_open
            // 
            this.btn_open.Location = new System.Drawing.Point(137, 370);
            this.btn_open.Name = "btn_open";
            this.btn_open.Size = new System.Drawing.Size(75, 23);
            this.btn_open.TabIndex = 1;
            this.btn_open.Text = "Open";
            this.btn_open.UseVisualStyleBackColor = true;
            this.btn_open.Click += new System.EventHandler(this.btn_open_Click);
            // 
            // btn_close
            // 
            this.btn_close.Enabled = false;
            this.btn_close.Location = new System.Drawing.Point(218, 370);
            this.btn_close.Name = "btn_close";
            this.btn_close.Size = new System.Drawing.Size(75, 23);
            this.btn_close.TabIndex = 2;
            this.btn_close.Text = "Close";
            this.btn_close.UseVisualStyleBackColor = true;
            this.btn_close.Click += new System.EventHandler(this.btn_close_Click);
            // 
            // txt_port
            // 
            this.txt_port.Location = new System.Drawing.Point(90, 372);
            this.txt_port.Name = "txt_port";
            this.txt_port.Size = new System.Drawing.Size(41, 20);
            this.txt_port.TabIndex = 3;
            this.txt_port.Text = "6969";
            // 
            // groupBox1
            // 
            this.groupBox1.Location = new System.Drawing.Point(12, 41);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(593, 323);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Streamer";
            // 
            // btn_play
            // 
            this.btn_play.Location = new System.Drawing.Point(511, 370);
            this.btn_play.Name = "btn_play";
            this.btn_play.Size = new System.Drawing.Size(94, 23);
            this.btn_play.TabIndex = 1;
            this.btn_play.Text = "Play Game >>";
            this.btn_play.UseVisualStyleBackColor = true;
            this.btn_play.Click += new System.EventHandler(this.btn_play_Click);
            // 
            // txt_numberConnect
            // 
            this.txt_numberConnect.AutoSize = true;
            this.txt_numberConnect.Font = new System.Drawing.Font("Courier New", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt_numberConnect.Location = new System.Drawing.Point(580, 9);
            this.txt_numberConnect.Name = "txt_numberConnect";
            this.txt_numberConnect.Size = new System.Drawing.Size(17, 17);
            this.txt_numberConnect.TabIndex = 5;
            this.txt_numberConnect.Text = "0";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Courier New", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(490, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(89, 17);
            this.label1.TabIndex = 6;
            this.label1.Text = "Tham gia:";
            // 
            // frm_server
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(617, 403);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txt_numberConnect);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.txt_port);
            this.Controls.Add(this.btn_close);
            this.Controls.Add(this.btn_play);
            this.Controls.Add(this.btn_open);
            this.Controls.Add(this.txt_ip);
            this.Name = "frm_server";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Server MyConfetti";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txt_ip;
        private System.Windows.Forms.Button btn_open;
        private System.Windows.Forms.Button btn_close;
        private System.Windows.Forms.TextBox txt_port;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btn_play;
        private System.Windows.Forms.Label txt_numberConnect;
        private System.Windows.Forms.Label label1;
    }
}

