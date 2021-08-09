namespace SaveTextFile
{
    partial class Form1
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
            this.components = new System.ComponentModel.Container();
            this.btnConnect = new System.Windows.Forms.Button();
            this.CbdetectCom = new System.Windows.Forms.ComboBox();
            this.Com = new System.IO.Ports.SerialPort(this.components);
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.Send = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.myzed = new ZedGraph.ZedGraphControl();
            this.btnStopDraw = new System.Windows.Forms.Button();
            this.labelStatus = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txb_save_status = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(33, 68);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(75, 23);
            this.btnConnect.TabIndex = 0;
            this.btnConnect.Text = "Connect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // CbdetectCom
            // 
            this.CbdetectCom.FormattingEnabled = true;
            this.CbdetectCom.Location = new System.Drawing.Point(33, 20);
            this.CbdetectCom.Name = "CbdetectCom";
            this.CbdetectCom.Size = new System.Drawing.Size(75, 21);
            this.CbdetectCom.TabIndex = 1;
            // 
            // Com
            // 
            this.Com.BaudRate = 19200;
            this.Com.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(this.Com_DataReceived);
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // Send
            // 
            this.Send.Location = new System.Drawing.Point(33, 163);
            this.Send.Name = "Send";
            this.Send.Size = new System.Drawing.Size(75, 23);
            this.Send.TabIndex = 2;
            this.Send.Text = "SaveData";
            this.Send.UseVisualStyleBackColor = true;
            this.Send.Click += new System.EventHandler(this.Send_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(33, 116);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 3;
            this.button2.Text = "Diconnect";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // myzed
            // 
            this.myzed.Location = new System.Drawing.Point(169, 37);
            this.myzed.Name = "myzed";
            this.myzed.ScrollGrace = 0D;
            this.myzed.ScrollMaxX = 0D;
            this.myzed.ScrollMaxY = 0D;
            this.myzed.ScrollMaxY2 = 0D;
            this.myzed.ScrollMinX = 0D;
            this.myzed.ScrollMinY = 0D;
            this.myzed.ScrollMinY2 = 0D;
            this.myzed.Size = new System.Drawing.Size(901, 445);
            this.myzed.TabIndex = 4;
            // 
            // btnStopDraw
            // 
            this.btnStopDraw.Location = new System.Drawing.Point(33, 203);
            this.btnStopDraw.Name = "btnStopDraw";
            this.btnStopDraw.Size = new System.Drawing.Size(75, 23);
            this.btnStopDraw.TabIndex = 5;
            this.btnStopDraw.Text = "StopDraw";
            this.btnStopDraw.UseVisualStyleBackColor = true;
            this.btnStopDraw.Click += new System.EventHandler(this.btnStopDraw_Click);
            // 
            // labelStatus
            // 
            this.labelStatus.AutoSize = true;
            this.labelStatus.Location = new System.Drawing.Point(48, 252);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(0, 13);
            this.labelStatus.TabIndex = 6;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txb_save_status);
            this.groupBox1.Controls.Add(this.labelStatus);
            this.groupBox1.Controls.Add(this.btnStopDraw);
            this.groupBox1.Controls.Add(this.button2);
            this.groupBox1.Controls.Add(this.Send);
            this.groupBox1.Controls.Add(this.CbdetectCom);
            this.groupBox1.Controls.Add(this.btnConnect);
            this.groupBox1.Location = new System.Drawing.Point(11, 21);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(141, 335);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            // 
            // txb_save_status
            // 
            this.txb_save_status.Location = new System.Drawing.Point(33, 268);
            this.txb_save_status.Name = "txb_save_status";
            this.txb_save_status.Size = new System.Drawing.Size(75, 20);
            this.txb_save_status.TabIndex = 8;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1072, 486);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.myzed);
            this.Name = "Form1";
            this.Text = "ECGTest";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.ComboBox CbdetectCom;
        private System.IO.Ports.SerialPort Com;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button Send;
        private System.Windows.Forms.Button button2;
        private ZedGraph.ZedGraphControl myzed;
        private System.Windows.Forms.Button btnStopDraw;
        private System.Windows.Forms.Label labelStatus;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txb_save_status;
    }
}

