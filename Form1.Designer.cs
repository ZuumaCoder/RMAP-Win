namespace DroneStation
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.label_ip = new System.Windows.Forms.ToolStripStatusLabel();
            this.label_status = new System.Windows.Forms.ToolStripStatusLabel();
            this.counts_label = new System.Windows.Forms.ToolStripStatusLabel();
            this.upDateTimer = new System.Windows.Forms.Timer(this.components);
            this.panel_Picture = new System.Windows.Forms.Panel();
            this.dronePic = new System.Windows.Forms.PictureBox();
            this.mapPic = new System.Windows.Forms.PictureBox();
            this.statusStrip1.SuspendLayout();
            this.panel_Picture.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dronePic)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mapPic)).BeginInit();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.label_ip,
            this.label_status,
            this.counts_label});
            this.statusStrip1.Location = new System.Drawing.Point(0, 889);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1243, 26);
            this.statusStrip1.TabIndex = 0;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // label_ip
            // 
            this.label_ip.Name = "label_ip";
            this.label_ip.Size = new System.Drawing.Size(167, 20);
            this.label_ip.Text = "toolStripStatusLabel1";
            // 
            // label_status
            // 
            this.label_status.Name = "label_status";
            this.label_status.Size = new System.Drawing.Size(39, 20);
            this.label_status.Text = "状态";
            // 
            // counts_label
            // 
            this.counts_label.Name = "counts_label";
            this.counts_label.Size = new System.Drawing.Size(69, 20);
            this.counts_label.Text = "接收计数";
            // 
            // upDateTimer
            // 
            this.upDateTimer.Enabled = true;
            this.upDateTimer.Interval = 50;
            this.upDateTimer.Tick += new System.EventHandler(this.upDateTimer_Tick);
            // 
            // panel_Picture
            // 
            this.panel_Picture.BackColor = System.Drawing.Color.White;
            this.panel_Picture.Controls.Add(this.dronePic);
            this.panel_Picture.Controls.Add(this.mapPic);
            this.panel_Picture.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel_Picture.Location = new System.Drawing.Point(0, 0);
            this.panel_Picture.Name = "panel_Picture";
            this.panel_Picture.Size = new System.Drawing.Size(1243, 889);
            this.panel_Picture.TabIndex = 2;
            // 
            // dronePic
            // 
            this.dronePic.BackColor = System.Drawing.Color.Transparent;
            this.dronePic.Image = ((System.Drawing.Image)(resources.GetObject("dronePic.Image")));
            this.dronePic.Location = new System.Drawing.Point(554, 426);
            this.dronePic.Name = "dronePic";
            this.dronePic.Size = new System.Drawing.Size(31, 31);
            this.dronePic.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.dronePic.TabIndex = 2;
            this.dronePic.TabStop = false;
            // 
            // mapPic
            // 
            this.mapPic.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mapPic.Location = new System.Drawing.Point(0, 0);
            this.mapPic.Name = "mapPic";
            this.mapPic.Size = new System.Drawing.Size(1243, 889);
            this.mapPic.TabIndex = 1;
            this.mapPic.TabStop = false;
            this.mapPic.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseDown);
            this.mapPic.MouseEnter += new System.EventHandler(this.pictureBox1_MouseEnter);
            this.mapPic.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseMove);
            this.mapPic.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseUp);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1243, 915);
            this.Controls.Add(this.panel_Picture);
            this.Controls.Add(this.statusStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "RMAP-Win";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.panel_Picture.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dronePic)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mapPic)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel label_ip;
        private System.Windows.Forms.ToolStripStatusLabel label_status;
        private System.Windows.Forms.Timer upDateTimer;
        private System.Windows.Forms.PictureBox mapPic;
        private System.Windows.Forms.ToolStripStatusLabel counts_label;
        private System.Windows.Forms.Panel panel_Picture;
        private System.Windows.Forms.PictureBox dronePic;
    }
}

