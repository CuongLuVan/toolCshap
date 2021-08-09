using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;
using System.IO;
using System.Collections;
using ZedGraph;
using System.Threading;

namespace SaveTextFile
{
    public partial class Form1 : Form
    {
        // Biến lưu data
        Thread Savefile;
        private string file_ecg = @"D:\data_ecg.txt";
        int dt,dt2;
        private double time;
        private ArrayList data_ecg;
        ArrayList list1 = new ArrayList();
        private bool is_s1 = false;
        private bool stop_draw = true;
        private int save_length = 5000;//Chiều dài để lưu
        private int f_s = 1000;
        private int index = 0;

        //private string path_file;
        private FileProcessing save_ecg;
        private bool is_save = false;
        private bool is_read = false;
        //byte[] dt;
        int[] data_save;
        int l_data = 0;
       RollingPointPairList my_line = new RollingPointPairList(200000);
        PointPairList list = new PointPairList();
       
        public Form1()
        {
            InitializeComponent();
            data_ecg = new ArrayList();
            save_ecg = new FileProcessing(file_ecg);
            data_save = new int[data_ecg.Count];

            string[] list_port = SerialPort.GetPortNames();
            CbdetectCom.Items.AddRange(list_port);

            //dt = new byte[1];
        //    time = 1.0 / f_s;
            time = 0;
            // timer1.Interval = 100;  //cứ sau khoảng thời gian này thì sự kiện timer_tick được gọi
            timer1.Interval = 100;
            //string[] ports = SerialPort.GetPortNames();//Lay tat ca com noi vao PC
            //CbdetectCom.Items.AddRange(ports);
            myzed.GraphPane.AddCurve("ECG signal", my_line, Color.Red, SymbolType.None);
            createGraph(myzed);
            Com.DataReceived += new SerialDataReceivedEventHandler(Com_DataReceived);
        }
        int intlen = 0; //Luu gia tri so Com ket noi vao may tinh


        #region Thay đổi trục thời gian của Zedgraph
        private void SetTime_ZedGraph(ZedGraphControl zed)
        {
            try
            {
                Scale xscale = zed.GraphPane.XAxis.Scale;

                // if (xscale.Max - xscale.MajorStep < time)
                if (xscale.Max < time)
                {
                    // xscale.Max = time + xscale.MajorStep;
                    xscale.Max = time;
                   // xscale.Max += 0.1;
                    xscale.Min = xscale.Max -4;//Khung cửa sổ thời gian hiện tín hiệu
                    zed.GraphPane.AxisChange();

                }

                zed.Invalidate();
            }
            catch (Exception ex) { }
        }

        private void createGraph(ZedGraphControl zg1)
        {
            //Set a reference to the GraphPane
            GraphPane myPane = zg1.GraphPane;
            //Set title
            myPane.Title.Text = " Raw ECG signal";
            myPane.XAxis.Title.Text = "Time";
            myPane.YAxis.Title.Text = "Amplitude";


        }

        #endregion

        private void timer1_Tick(object sender, EventArgs e)
        {
            /*  string[] ports = SerialPort.GetPortNames();//Lay tat ca com noi vao PC
               if (intlen != ports.Length)
               {
                   intlen = ports.Length;
                   CbdetectCom.Items.Clear();
                   for (int j = 0; j < intlen; j++)
                   {
                       CbdetectCom.Items.Add(ports[j]);

                   }
               }
             * 
             * 
               */
            if (is_read == true)
            {
              //time += 1;
               SetTime_ZedGraph(myzed);

            }

        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            is_read = true;
            Com.PortName = CbdetectCom.SelectedItem.ToString();

            // Com.PortName = CbdetectCom.SelectedItem.ToString();
            Com.Open();
            btnConnect.Visible = false;
            timer1.Start();
        }
        bool is_full = false;
        private void Send_Click(object sender, EventArgs e)
        {

            Savefile = new Thread(new ThreadStart(ThreadSaveFile));

            if (!is_full)
            {
                is_full = true;
                txb_save_status.Text = "Saving...";
            }
            else
            {
                Savefile.Start();
                is_full = false;
                txb_save_status.Text = "Done";
            }

            // is_save = true;

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Com.Close();
            btnConnect.Visible = true;
        }

        private void ThreadSaveFile()
        {
            while (true)
            {
                //if (is_full == false)
                {
                    data_save = new int[data_ecg.Count];
                    data_ecg.CopyTo(0, data_save, 0, data_ecg.Count);
                    save_ecg.SaveData(data_save);
                    data_ecg.Clear();
                    //ThreadSaveFile.
                    is_save = true;
                    Savefile.Abort();
                }


            }
        }
        bool flag_initial = true;
        bool flag1 = true;
        bool flag2 = true;
        string[] dt1;
       //string s;
        int a, b;
        private void Com_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            // string s1 = Com.ReadExisting();
            //for (int i = 0; i<10; i++)
            //{
            if (flag1)
            {
                string s = Com.ReadExisting();//Read data from Com port
                //  flag1 = false;
                flag1 = false;
            }
                
            //}
            
            //string s1 = Com.ReadLine();
            //s0 = Com.ReadExisting();
            // string number= null;%
            //   const char kytu = '\n';

            /*char[] newArray = new char[]
               {
                   kytu
               };*/
            //   dt1 = s0.Split(kytu);
            //   string Output = "";
           // int l_data = Com.BytesToRead;
           // byte[] data = new byte[l_data];
           //// byte[] data1 = new byte[2];

           // Com.Read(data, 0, l_data);
           // //Com.Read(data1, 0, 1);
           //// Com.Read
           // //   PointPairList list1 = new PointPairList();
           // for (int i = 0; i < l_data / 2; i++)
           // { // a = 256*(Com.ReadByte());
           //     //if (data1[0]<=4)
           //     //{
           //     //    dt = data[2 * i + 1] + data[2 * i] * 256;
           //     ////    flag_initial = false;
           //     //}
           //     //else
           //     dt = data[2 * i + 1] + data[2 * i] * 256;

           //     }
            dt = 400-(Com.ReadByte());
        //}

              // dt = Convert.ToInt16(s1);
                    if (is_read)
                    {
                        my_line.Add(time, dt);
                        time += 1.0 / f_s;
                    }

                //}


            if (is_full)
            {
                data_ecg.Add(dt);

            }
        }



        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void btnStopDraw_Click(object sender, EventArgs e)
        {
            stop_draw = false;
            is_read = false;
        }

    }
}


