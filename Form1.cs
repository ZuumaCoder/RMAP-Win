using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.IO;
using Newtonsoft.Json;
//using OpenCvSharp;
//using OpenCvSharp.Extensions;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

namespace DroneStation
{
    public partial class Form1 : Form
    {
        //int deltaZoomW = (int)(mapPic.Width * (mapImgZoomX - 1) / (2 * mapImgZoomX));
        //int deltaZoomH = (int)(mapPic.Height * (mapImgZoomY - 1) / (2 * mapImgZoomY));


        //int startX = (int)(mapInfoList[i].DroneInMap.X) + (int)(mapInfoList[i].MapInPanel.X - mapPic.Location.X);
        //int startY = (int)(mapInfoList[i].DroneInMap.Y) + (int)(mapInfoList[i].MapInPanel.Y - mapPic.Location.Y);

        //int endX = (int)(mapInfoList[i + 1].DroneInMap.X) + (int)(mapInfoList[i + 1].MapInPanel.X - mapPic.Location.X);
        //int endY = (int)(mapInfoList[i + 1].DroneInMap.Y) + (int)(mapInfoList[i + 1].MapInPanel.Y - mapPic.Location.Y);


        private int[]  mapDataSets;
        private int mapHeight=0;
        private int mapWidth=0;
        private int mapSize=0;
        private float mapX, mapY,odomX,odomY,rt_Yaw;
        private bool isNewMap = false;
        private Bitmap mapBackImg;
        private int originDroneWidth = 0;
        private int originDroneHeight = 0;

        public System.Drawing.Point mouseDownPoint;//存储鼠标焦点的全局变量
        public bool isSelected = false;
        public int imgTranslateX = 0, imgTranslateY = 0;
        public int mapImgWidth = 0, mapImgHeight = 0;
        public float mapImgZoomX = 1, mapImgZoomY = 1;
        public int originX, originY;
        public int zeroX, zeroY;
        public System.Drawing.Point lastDronePositon = new System.Drawing.Point(0, 0);
        public List<MapInfo> mapInfoList = new List<MapInfo>();

        int framesCount = 0;
        byte[] imgData;
        //int imgDataWidth, imgDataHeight;
        //float mapOriginX, mapOriginY;
        //float odomOriginX, odomOriginY;

        public class MapInfo
        {
            public int Height { get; set; }
            public int Width { get; set; }
            public float MapX { get; set; }
            public float MapY { get; set; }
            public float OdomX { get; set; }
            public float OdomY { get; set; }
            public float RT_Yaw { get; set; }

            public System.Drawing.Point DroneInMap { get; set; }
            public System.Drawing.Point MapInPanel { get; set; }
            public System.Drawing.Point DroneInPanel { get; set; }
            public MapInfo(int Width,int Height, float MapX, float MapY, float OdomX, float OdomY,float RT_Yaw)
            {
                this.Height = Height;
                this.Width = Width;
                this.MapX = MapX;
                this.MapY = MapY;
                this.OdomX = OdomX;
                this.OdomY = OdomY;
                this.RT_Yaw = RT_Yaw;
            }
        }


        public Form1()
        {
            InitializeComponent();
            Control.CheckForIllegalCrossThreadCalls = false;
            dronePic.Parent = mapPic;
            zeroX = panel_Picture.Width / 2;
            zeroY = panel_Picture.Height / 2;
            originX = zeroX;
            originY = zeroY;
            mapPic.SizeMode = PictureBoxSizeMode.Zoom;
            mapPic.Dock = DockStyle.None;
            this.mapPic.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseWheel);
            this.mapPic.Location = new System.Drawing.Point(0, 0);
            originDroneWidth = dronePic.Width;
            originDroneHeight = dronePic.Height;
            RotateImage(dronePic, Properties.Resources.drone, 45+90);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            label_ip.Text = localIP= getLocalIP();
            upDateTimer.Enabled = true;
            upDateTimer.Start();
            startServer();
        }


        #region Tcp服务器
        string localIP = string.Empty;
        int port = 8080;
        Thread threadWatch = null; // 负责监听客户端连接请求的 线程；
        Socket socketWatch = null;
        Dictionary<string, Socket> sockDict = new Dictionary<string, Socket>();//存放套接字
        Dictionary<string, Thread> sockDictThread = new Dictionary<string, Thread>();//存放线程




        private string getLocalIP()
        {
            //获取本地的IP地址
            string AddressIP = string.Empty;
            foreach (IPAddress _IPAddress in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
            {
                if (_IPAddress.AddressFamily.ToString() == "InterNetwork")
                {
                    AddressIP = _IPAddress.ToString();
                }
            }
            return AddressIP;
        }

        private void startServer()
        {
            socketWatch = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress address = IPAddress.Parse(localIP.Trim());
            IPEndPoint endPoint = new IPEndPoint(address, port);
            try
            {
                socketWatch.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                socketWatch.Bind(endPoint);
            }
            catch(SocketException se)
            {
                MessageBox.Show("异常：", se.Message);
                return;
            }
            socketWatch.Listen(100000);
            threadWatch = new Thread(WatchConnecting);
            threadWatch.IsBackground = true;
            threadWatch.Start();
        }


        /// <summary>
        /// 监听客户端请求的方法；
        /// </summary>
        void WatchConnecting()
        {
            while (true)  // 持续不断的监听客户端的连接请求；
            {
                // 开始监听客户端连接请求，Accept方法会阻断当前的线程；
                Socket sokConnection = socketWatch.Accept(); // 一旦监听到一个客户端的请求，就返回一个与该客户端通信的 套接字；
                // 将与客户端连接的 套接字 对象添加到集合中；
                sockDict.Add(sokConnection.RemoteEndPoint.ToString(), sokConnection);
                Thread thr = new Thread(RecMsg);
                thr.IsBackground = true;
                thr.Start(sokConnection);
                sockDictThread.Add(sokConnection.RemoteEndPoint.ToString(), thr);  //  将新建的线程 添加 到线程的集合中去。
                this.Invoke((EventHandler)delegate
                {
                    label_status.Text = sokConnection.RemoteEndPoint.ToString() + "上线";
                });
            }
        }
        /*
             {"mapHeight":522,"mapWidth":489,"PositionX":-13.35,"PositionY":-18.74,"PositionZ":0.00}
         */

        void RecMsg(object sokConnectionparn)
        {
            Socket sokClient = sokConnectionparn as Socket;
            byte[] mapData = new byte[1024 * 10000];
            int mapDataIndex = 0;

            while (true)
            {
                // 定义一个缓存区；
                byte[] arrMsgRec = new byte[1024*10000];
                // 将接受到的数据存入到输入  arrMsgRec中；
                int length = -1;
                try
                {
                    length = sokClient.Receive(arrMsgRec); // 接收数据，并返回数据的长度；
                    if (length > 0)
                    {
                        byte[] cmdBytes=new byte[150];
                        Array.Copy(arrMsgRec, 0, cmdBytes, 0, 150);
                        string cmd = ASCIIEncoding.ASCII.GetString(cmdBytes);
                        if (cmd.Substring(0, 11) == "[{\"Height\":")
                        {
                            List<MapInfo> jobInfoList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<MapInfo>>(cmd);
                            mapWidth = jobInfoList[0].Width;
                            mapHeight = jobInfoList[0].Height;
                            mapX = jobInfoList[0].MapX;
                            mapY = jobInfoList[0].MapY;
                            odomX = jobInfoList[0].OdomX;
                            odomY = jobInfoList[0].OdomY;
                            rt_Yaw= jobInfoList[0].RT_Yaw;
                            mapSize = mapHeight * mapWidth;
                            mapDataSets = new int[mapSize];
                            continue;
                        }
                        else
                        {
                            Array.Copy(arrMsgRec, 0, mapData, mapDataIndex, length);
                            mapDataIndex += length;
                            if (mapDataIndex == mapSize)
                            {
                                Array.Copy(mapData, 0, mapDataSets, 0, mapSize);
                                isNewMap = true;
                                mapDataIndex = 0;
                                mapData = new byte[1024 * 10000];
                            }
                            else if (mapDataIndex > mapSize)
                            {
                                mapDataIndex = 0;
                                mapData = new byte[1024 * 10000];
                            }
                        }
                    }
                    else
                    {
                        // 从 通信套接字 集合中删除被中断连接的通信套接字;
                        sockDict.Remove(sokClient.RemoteEndPoint.ToString());
                        // 从通信线程集合中删除被中断连接的通信线程对象;
                        sockDictThread.Remove(sokClient.RemoteEndPoint.ToString());
                        this.Invoke((EventHandler)delegate
                        {
                            label_status.Text = sokClient.RemoteEndPoint.ToString() + "断开连接";
                        });                       
                        break;
                    }
                }
                catch (SocketException se)
                {
                    // 从 通信套接字 集合中删除被中断连接的通信套接字;
                    sockDict.Remove(sokClient.RemoteEndPoint.ToString());
                    // 从通信线程集合中删除被中断连接的通信线程对象;
                    sockDictThread.Remove(sokClient.RemoteEndPoint.ToString());
                    label_status.Text = sokClient.RemoteEndPoint.ToString() + "断开,异常消息：" + se.Message;
                    break;
                }
                catch (Exception e)
                {
                    // 从 通信套接字 集合中删除被中断连接的通信套接字;
                    sockDict.Remove(sokClient.RemoteEndPoint.ToString());
                    // 从通信线程集合中删除被中断连接的通信线程对象;
                    sockDictThread.Remove(sokClient.RemoteEndPoint.ToString());
                    label_status.Text = "异常消息：" + e.Message;
                    break;
                }
            }
        }
        #endregion

        #region 地图显示


        /// <summary>
        /// 一维数组生成灰度图并在指定picturebox中显示
        /// </summary>
        /// <param name="oneDArray">一维数组</param>
        /// <param name="width">图片宽度</param>
        /// <param name="height">图片高度</param>
        /// <param name="pictureBox">picturebox控件</param>
        private void DisplayGrayImage(byte[] oneDArray, int width, int height, PictureBox pictureBox)
        {
            int[,] twoDArray = new int[width, height];

            Parallel.For(0, height, (i) => {
                Parallel.For(0, width, (j) => {
                    twoDArray[j, i] = oneDArray[i * width + j];
                });
            });
            Bitmap mapImg = new Bitmap(width, height,PixelFormat.Format24bppRgb);                
            BitmapData bitmapData = mapImg.LockBits(new Rectangle(0, 0, mapImg.Width, mapImg.Height), ImageLockMode.ReadWrite, mapImg.PixelFormat);

            unsafe
            {

                int bytesPerPixel = System.Drawing.Bitmap.GetPixelFormatSize(mapImg.PixelFormat) / 8;
                int heightInPixels = bitmapData.Height;
                int widthInBytes = bitmapData.Width * bytesPerPixel;
                byte* PtrFirstPixel = (byte*)bitmapData.Scan0;
                Parallel.For(0, heightInPixels, y =>
                {
                    byte* currentLine = PtrFirstPixel + (y * bitmapData.Stride);
                    for (int x = 0; x < widthInBytes; x = x + bytesPerPixel)
                    {
                        currentLine[x] = (byte)twoDArray[x / bytesPerPixel, y];
                        currentLine[x+1] = (byte)twoDArray[x / bytesPerPixel, y];
                        currentLine[x+2] = (byte)twoDArray[x / bytesPerPixel, y];
                    }
                });
                mapImg.UnlockBits(bitmapData);
            }

            /************************************************************************************************************************/
            /*地图相关配置*/
            mapPic.Image = mapImg;
            int deltaX = (int)(mapImgZoomX * mapInfoList[mapInfoList.Count-1].MapX / 0.05f);
            int deltaY = (int)(mapImgZoomY * mapInfoList[mapInfoList.Count-1].MapY / 0.05f);
            originX = zeroX + deltaX;
            originY = zeroY + deltaY;
            this.mapPic.Location = new System.Drawing.Point(originX + imgTranslateX, originY + imgTranslateY);
            mapPic.Width = (int)(width * mapImgZoomX);
            mapPic.Height = (int)(height * mapImgZoomY);
            //mapInfoList[mapInfoList.Count - 1].MapInPanel = new System.Drawing.Point(originX, originY);
            mapInfoList[mapInfoList.Count - 1].MapInPanel = new System.Drawing.Point(
                                                                                    zeroX + (int)(1 * mapInfoList[mapInfoList.Count - 1].MapX / 0.05f),
                                                                                    zeroY + (int)(1 * mapInfoList[mapInfoList.Count - 1].MapY / 0.05f)
                                                                                   );
            /************************************************************************************************************************/
            /*绘制飞机*/
            int droneX = (int)(mapImgZoomX * (mapInfoList[mapInfoList.Count - 1].OdomX / 0.05f) - deltaX - dronePic.Width/2);
            int droneY = (int)(mapImgZoomY * (mapInfoList[mapInfoList.Count - 1].OdomY / 0.05f) - deltaY - dronePic.Height/2);
            this.dronePic.Location = new System.Drawing.Point(droneX, droneY);
            mapInfoList[mapInfoList.Count - 1].DroneInMap = new System.Drawing.Point(
                                                                        (int)(1 * (mapInfoList[mapInfoList.Count - 1].OdomX / 0.05f) - ((int)(1 * mapInfoList[mapInfoList.Count - 1].MapX / 0.05f))),
                                                                        (int)(1 * (mapInfoList[mapInfoList.Count - 1].OdomY / 0.05f) - ((int)(1 * mapInfoList[mapInfoList.Count - 1].MapY / 0.05f)))
                                                                       );
            dronePic.Width = (int)(originDroneWidth * mapImgZoomX);
            dronePic.Height = (int)(originDroneHeight * mapImgZoomY);
            RotateImage(dronePic, Properties.Resources.drone, mapInfoList[mapInfoList.Count - 1].RT_Yaw+90);
            //dronePic.Image.Dispose();
            /************************************************************************************************************************/
            /*绘制轨迹*/
            if (mapInfoList.Count > 1)
            {
                for (int i = 0; i < mapInfoList.Count - 1; i++)
                {
                    int startX = (int)(mapInfoList[i].DroneInMap.X) + (int)((mapInfoList[i].MapInPanel.X - mapInfoList[mapInfoList.Count - 1].MapInPanel.X));
                    int startY = (int)(mapInfoList[i].DroneInMap.Y) + (int)((mapInfoList[i].MapInPanel.Y - mapInfoList[mapInfoList.Count - 1].MapInPanel.Y));

                    int endX = (int)(mapInfoList[i + 1].DroneInMap.X) + (int)((mapInfoList[i + 1].MapInPanel.X - mapInfoList[mapInfoList.Count - 1].MapInPanel.X));
                    int endY = (int)(mapInfoList[i + 1].DroneInMap.Y) + (int)((mapInfoList[i + 1].MapInPanel.Y - mapInfoList[mapInfoList.Count - 1].MapInPanel.Y));

                    System.Drawing.Point startPoint = new System.Drawing.Point((int)(startX), (int)(startY));
                    System.Drawing.Point endPoint = new System.Drawing.Point((int)(endX), (int)(endY));

                    DrawLineInPicture(mapImg, startPoint, endPoint, Color.Blue, 2, DashStyle.DashDotDot);
                }
            }
            /************************************************************************************************************************/

        }

        #region 图片旋转
        public static Bitmap RotateImage(Image image, float angle)
        {
            if (image == null)
                throw new ArgumentNullException("image");
            float dx = image.Width / 2.0f;
            float dy = image.Height / 2.0f;

            Bitmap rotatedBmp = new Bitmap(image.Width, image.Height);
            rotatedBmp.SetResolution(image.HorizontalResolution, image.VerticalResolution);
            Graphics g = Graphics.FromImage(rotatedBmp);
            g.TranslateTransform(dx, dy);
            g.RotateTransform(angle);
            g.TranslateTransform(-dx, -dy);
            g.DrawImage(image, new PointF(0, 0));
            return rotatedBmp;
        }

        private void RotateImage(PictureBox pb, Image img, float angle)
        {
            if (img == null || pb.Image == null)
                return;
            Image oldImage = pb.Image;
            pb.Image = RotateImage(img, angle);
            if (oldImage != null)
            {
                oldImage.Dispose();
            }
        }
        #endregion

        /// <summary>
        /// 在图片上画线
        /// </summary>
        /// <param name="bmp">原始图</param>
        /// <param name="p0">起始点</param>
        /// <param name="p1">终止点</param>
        /// <param name="RectColor">线的颜色</param>
        /// <param name="LineWidth">线宽</param>
        /// <param name="ds">线条样式</param>
        /// <returns>输出图</returns>
        public static Bitmap DrawLineInPicture(Bitmap bmp, System.Drawing.Point p0, System.Drawing.Point p1, Color LineColor, int LineWidth, DashStyle ds)
        {
            if (bmp == null) return null;

            //if (p0.X == p1.X || p0.Y == p1.Y) return bmp;

            Graphics g = Graphics.FromImage(bmp);

            Brush brush = new SolidBrush(LineColor);

            Pen pen = new Pen(brush, LineWidth);
            //pen.Alignment = PenAlignment.Inset;

            pen.DashStyle = ds;

            g.DrawLine(pen, p0, p1);

            g.Dispose();

            return bmp;
        }

        private bool IsMouseInPanel()
        {
            if (this.panel_Picture.Left < PointToClient(Cursor.Position).X
                    && PointToClient(Cursor.Position).X < this.panel_Picture.Left
                    + this.panel_Picture.Width && this.panel_Picture.Top
                    < PointToClient(Cursor.Position).Y && PointToClient(Cursor.Position).Y
                    < this.panel_Picture.Top + this.panel_Picture.Height)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        private void pictureBox1_MouseEnter(object sender, EventArgs e)
        {
            panel_Picture.Focus();
            panel_Picture.Cursor = Cursors.SizeAll;
        }


        //在MouseDown处获知鼠标是否按下，并记录下此时的鼠标坐标值
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                mouseDownPoint.X = Cursor.Position.X;  //注：全局变量mouseDownPoint前面已定义为Point类型  
                mouseDownPoint.Y = Cursor.Position.Y;
                isSelected = true;
            }
        }
        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            isSelected = false;
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (isSelected && IsMouseInPanel())//确定已经激发MouseDown事件，和鼠标在picturebox的范围内
            {
                imgTranslateX = imgTranslateX + (Cursor.Position.X - mouseDownPoint.X);
                imgTranslateY = imgTranslateY + (Cursor.Position.Y - mouseDownPoint.Y);
                mouseDownPoint.X = Cursor.Position.X;
                mouseDownPoint.Y = Cursor.Position.Y;
            }
        }

        void pictureBox1_MouseWheel(object sender, MouseEventArgs e)
        {
            mapImgZoomX += ((e.Delta) * 0.001f);
            mapImgZoomY += e.Delta * 0.001f;
        }



        private void upDateTimer_Tick(object sender, EventArgs e)
        {

            if (isNewMap)
            {
                imgData = new byte[mapDataSets.Length];
                MapInfo mapinfo = new MapInfo(mapWidth, mapHeight, mapX, mapY, odomX, odomY,rt_Yaw);
                mapInfoList.Add(mapinfo);
                for (int i = 0; i < mapDataSets.Length; i++)
                {
                    if (mapDataSets[i] == 255)
                    {
                        imgData[i] = 255;
                    }
                    else if (mapDataSets[i] >= 0)
                    {
                        imgData[i] = (byte)((float)mapDataSets[i] / 100.0f * 255);
                        imgData[i] = (byte)(255 - imgData[i]);
                    }
                }
                counts_label.Text = "接收计数：" + framesCount++;
                isNewMap = false;
            }                
            if (imgData != null)
                   DisplayGrayImage(imgData, mapInfoList[mapInfoList.Count - 1].Width, mapInfoList[mapInfoList.Count - 1].Height, mapPic);
            //label1.Text = odomPic.Height.ToString()+":::"+mapPic.Height.ToString();
        }
        #endregion 
    }
}
