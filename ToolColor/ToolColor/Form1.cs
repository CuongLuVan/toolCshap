using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ToolColor
{
    public partial class Form1 : Form
    {
        Bitmap bm;
        public Form1()
        {
            InitializeComponent();
        }
        void resetColor()
        {
            Color c;
            int i, j;
            c = Color.FromArgb(0, 0, 0);
            for (i = 0; i < 100; i++) 
            for (j = 0; j < 100; j++) bm.SetPixel(i, j, c);
            pictureBox1.Image = bm;

        }
        private void Form1_Load(object sender, EventArgs e)
        {
            bm = new Bitmap(100, 100);
            resetColor();
            for (int t = 0; t < 40; t++) comboBox1.Items.Add("COM" + t.ToString());

        }

        private void button1_Click(object sender, EventArgs e)
        {
            label1.Text = "ban da chon cong com :" + comboBox1.SelectedItem.ToString() + "mjfjfjf";
            serialPort1.PortName = comboBox1.SelectedItem.ToString();
            serialPort1.Open();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            serialPort1.Dispose();
            serialPort1.Close();
            label1.Text = "dong lai cong comn";
        }

        private void showData(string data) {

            
            try
            {
                string[] xa = data.Split(new string[] { ":" }, StringSplitOptions.None);
                
                if (xa.Length > 2) {
                    label2.Text = data;
                    Int32 num1 = Int32.Parse(xa[0]);
                    Int32 num2 = Int32.Parse(xa[1]);
                    Int32 num3 = Int32.Parse(xa[2]);
                    Color c = Color.FromArgb(num1, num2, num3);
                    for (int i = 0; i < 100; i++) {
                        for (int j = 0; j < 100; j++)
                        {
                            bm.SetPixel(i, j, c);
                        }
                    }
                    pictureBox1.Image = bm;
                        
                }

            }
            catch (Exception ie) {
                MessageBox.Show("loi "+ie.ToString());
            }
          
        }

        private void serialPort1_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            string received_data = serialPort1.ReadLine();

            Invoke(new Action(() => showData(received_data)));
            //Invoke(new Action(() => label2.Text = received_data));

        }
    }
}
