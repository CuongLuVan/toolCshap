using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace kalman
{
    public partial class Form1 : Form
    {
        Bitmap bm, bm1, bm2, bm3, bm4, bm5, bm6, bm7, bm8, bm9, bm10, bm11, bm12, bm13, bm14;
        Random random = new Random();
        kalmans tacx, tacx1, tacx2, tacx3, tacx4, tacx5, tacx6, tacx7, tacx8, tacx9, tacx10, tacx11, tacx12, tacx13, tacx14;
        double Q_angle = 0.005; //0.001    //0.005
        double Q_gyro = 0.003;  //0.003  //0.0003
        double R_angle = 0.03;  //0.03     //0.008

        double x_bias = 0;
        double P_00 = 0, P_01 = 0, P_10 = 0, P_11 = 0;
        double y, S;
        double K_0, K_1;
        double x_angle;
        double[] tinhieuso;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            textBox1.Text = "0.05";
            textBox2.Text = "0.05";
            textBox3.Text = "0.05";
            textBox4.Text = "0.05";
            textBox5.Text = "0.05";
            bm = new Bitmap(1300, 500);
            bm1 = new Bitmap(1300, 500);
            bm2 = new Bitmap(1300, 500);
            bm3 = new Bitmap(1300, 500);
            bm4 = new Bitmap(1300, 500);
            bm5 = new Bitmap(1300, 500);
            bm6 = new Bitmap(1300, 500);
            bm7 = new Bitmap(1300, 500);
            bm8 = new Bitmap(1300, 500);
            bm9 = new Bitmap(1300, 500);
            bm10 = new Bitmap(1300, 500);
            bm11 = new Bitmap(1300, 500);
            bm12 = new Bitmap(1300, 500);
            bm13 = new Bitmap(1300, 500);
            tacx = new kalmans();
            tacx1 = new kalmans();
            tacx2 = new kalmans();
            tacx3 = new kalmans();
            tacx4 = new kalmans();
            tacx5 = new kalmans();
            tacx6 = new kalmans();
            tacx7 = new kalmans();
            tacx8 = new kalmans();
            tacx9 = new kalmans();
            tacx10 = new kalmans();
            tacx11 = new kalmans();
            tacx12 = new kalmans();
            tacx13 = new kalmans();
            tacx14 = new kalmans();
            tinhieuso = new double[1300];
            Color c;
            int i, j;
            c = Color.FromArgb(120, 120, 120);
            for (i = 0; i < 1300; i++) for (j = 0; j < 500; j++) bm.SetPixel(i, j, c);
        }
        public void cleallall()
        {
            Color c;
            int i, j;
            c = Color.FromArgb(120, 120, 120);
            for (i = 0; i < 1300; i++) for (j = 0; j < 500; j++) {
                bm.SetPixel(i, j, c);
                bm1.SetPixel(i, j, c);
                bm2.SetPixel(i, j, c);
                bm3.SetPixel(i, j, c);
                bm4.SetPixel(i, j, c);
                bm5.SetPixel(i, j, c);
                bm6.SetPixel(i, j, c);
                bm7.SetPixel(i, j, c);
                bm8.SetPixel(i, j, c);
                bm9.SetPixel(i, j, c);
                bm10.SetPixel(i, j, c);
                bm11.SetPixel(i, j, c);
                bm12.SetPixel(i, j, c);
                bm13.SetPixel(i, j, c);
                 }
        }
        double kalmanCalculate(double newAngle, double newRate, double looptime)
        {

            double dt = (looptime) / 1000;                                    // XXXXXXX arevoir
            x_angle += dt * (newRate - x_bias);
            P_00 += -dt * (P_10 + P_01) + Q_angle * dt;
            P_01 += -dt * P_11;
            P_10 += -dt * P_11;
            P_11 += +Q_gyro * dt;
            //x=A*x(k-1)+Bu
            // P=AP(k-1)At+Q
            //Kk=P*Ht*(H*P*Ht+R)-1
            //xm=x+Kk(zk-H*x)
            //Pm=(I-Kk*H)*P
            y = newAngle - x_angle;
            S = P_00 + R_angle;
            K_0 = P_00 / S;
            K_1 = P_10 / S;

            x_angle += K_0 * y;
            x_bias += K_1 * y;
            P_00 -= K_0 * P_00;
            P_01 -= K_0 * P_01;
            P_10 -= K_1 * P_00;
            P_11 -= K_1 * P_01;

            return x_angle;
        }

        public void thaydoitinhieu(double la)
        {
            for (int i = 0; i < 1300; i++)
            {
                tinhieuso[i] = (60 * Math.Sin(la * i));

            }

        }
        public void vetinhieu(double la,int lb,double lc) {
            Color c, d, v;
            int i, j, k, m;
            double te;
            c = Color.FromArgb(10, 220, 120);
            d = Color.FromArgb(250, 220, 200);
            v = Color.FromArgb(0, 0, 0);
            
            int ca, ct;
            ca = 200; ct = 0;
            for (i = 0; i < 1300; i++)
            {
                int mau;
                j = ((int)(tinhieuso[i]));
                mau = j * 100 + random.Next(-4000, 4000);
                k = 200 + (mau / 100);
                j = 200 + j;
                ct = k - ca;
                ca = k;
                te = 0;
                if (lb == 1) te = tacx.kalmanCalculate(mau / 4000, ct / 60, i * lc);
                if (lb == 2) te = tacx1.kalmanCalculate(mau / 4000, ct / 60, i * lc);
                if (lb == 3) te = tacx2.kalmanCalculate(mau / 4000, ct / 60, i * lc);
                if (lb == 4) te = tacx3.kalmanCalculate(mau / 4000, ct / 60, i * lc);
                if (lb == 5) te = tacx4.kalmanCalculate(mau / 4000, ct / 60, i * lc);
                if (lb == 6) te = tacx5.kalmanCalculate(mau / 4000, ct / 60, i * lc);
                if (lb == 7) te = tacx6.kalmanCalculate(mau / 4000, ct / 60, i * lc);
                if (lb == 8) te = tacx7.kalmanCalculate(mau / 4000, ct / 60, i * lc);
                if (lb == 9) te = tacx8.kalmanCalculate(mau / 4000, ct / 60, i * lc);
                if (lb == 10) te = tacx9.kalmanCalculate(mau / 4000, ct / 60, i * lc);
                if (lb == 11) te = tacx10.kalmanCalculate(mau / 4000, ct / 60, i * lc);
                if (lb == 12) te = tacx11.kalmanCalculate(mau / 4000, ct / 60, i * lc);
                if (lb == 13) te = tacx12.kalmanCalculate(mau / 4000, ct / 60, i * lc);
                if (lb == 14) te = tacx13.kalmanCalculate(mau / 4000, ct / 60, i * lc);
                //te = kalmanCalculate(mau / 6000, ct / 60, i * lc);
                m = (int)(te * 60);
                m = 200 + m;
                if (lb == 1)
                {
                    bm.SetPixel(i, j, c);
                    bm.SetPixel(i, k, d);
                    bm.SetPixel(i, m, v);
                }
                if (lb == 2)
                {
                    bm1.SetPixel(i, j, c);
                    bm1.SetPixel(i, k, d);
                    bm1.SetPixel(i, m, v);
                }
                if (lb == 3)
                {
                    bm2.SetPixel(i, j, c);
                    bm2.SetPixel(i, k, d);
                    bm2.SetPixel(i, m, v);
                }
                if (lb == 4)
                {
                    bm3.SetPixel(i, j, c);
                    bm3.SetPixel(i, k, d);
                    bm3.SetPixel(i, m, v);
                }
                if (lb == 5)
                {
                    bm4.SetPixel(i, j, c);
                    bm4.SetPixel(i, k, d);
                    bm4.SetPixel(i, m, v);
                }
                if (lb == 6)
                {
                    bm5.SetPixel(i, j, c);
                    bm5.SetPixel(i, k, d);
                    bm5.SetPixel(i, m, v);
                }
                if (lb == 7)
                {
                    bm6.SetPixel(i, j, c);
                    bm6.SetPixel(i, k, d);
                    bm6.SetPixel(i, m, v);
                }
                if (lb == 8)
                {
                    bm7.SetPixel(i, j, c);
                    bm7.SetPixel(i, k, d);
                    bm7.SetPixel(i, m, v);
                }
                    if (lb == 9)
                    {
                        bm8.SetPixel(i, j, c);
                        bm8.SetPixel(i, k, d);
                        bm8.SetPixel(i, m, v);
                    }
                    if (lb == 10)
                    {
                        bm9.SetPixel(i, j, c);
                        bm9.SetPixel(i, k, d);
                        bm9.SetPixel(i, m, v);
                    }
                    if (lb == 11)
                    {
                        bm10.SetPixel(i, j, c);
                        bm10.SetPixel(i, k, d);
                        bm10.SetPixel(i, m, v);
                    }
                    if (lb == 12)
                    {
                        bm11.SetPixel(i, j, c);
                        bm11.SetPixel(i, k, d);
                        bm11.SetPixel(i, m, v);
                    }
                    if (lb == 13)
                    {
                        bm12.SetPixel(i, j, c);
                        bm12.SetPixel(i, k, d);
                        bm12.SetPixel(i, m, v);
                    }
                    if (lb == 14)
                    {
                        bm13.SetPixel(i, j, c);
                        bm13.SetPixel(i, k, d);
                        bm13.SetPixel(i, m, v);
                    }
                

            }
            /*for (i = 500; i <1000; i++)
            {
                j = 200 + ((int)(20 * Math.Sin(0.05 * i)));
                k = j + random.Next(-40, 40);

                te = kalmanCalculate(k * 1.0, 0.000001, (i-500) * 0.1);
                m = (int)te;
                bm.SetPixel(i, j, c);
                bm.SetPixel(i, k, d);
                bm.SetPixel(i, m, v);
            }*/
        }

        private void button1_Click(object sender, EventArgs e)
        {
            double la, lc;
            cleallall();
      
            
             label5.Text = "begin";
            la=double.Parse(textBox1.Text);
            thaydoitinhieu(la);
            lc = 0.0025;
            heso1.Text = lc.ToString();
            vetinhieu(la,1,lc);
            pictureBox1.Image = bm;

            lc = 0.005;
            heso2.Text = lc.ToString();
            vetinhieu(la, 2, lc);
            pictureBox2.Image = bm1;

            lc = 0.0075;
            heso3.Text = lc.ToString();
            vetinhieu(la, 3, lc);
            pictureBox3.Image = bm2;

            lc = 0.01;
            heso4.Text = lc.ToString();
            vetinhieu(la, 4, lc);
            pictureBox4.Image = bm3;

            lc = 0.02;
            heso5.Text = lc.ToString();
            vetinhieu(la, 5, lc);
            pictureBox5.Image = bm4;

            lc = 0.04;
            heso6.Text = lc.ToString();
            vetinhieu(la, 6, lc);
            pictureBox6.Image = bm5;

            lc = 0.05;
            heso7.Text = lc.ToString();
            vetinhieu(la, 7, lc);
            pictureBox7.Image = bm6;

            lc = 0.07;
            heso8.Text = lc.ToString();
            vetinhieu(la, 8, lc);
            pictureBox8.Image = bm7;
            
            lc = 0.08;
            heso9.Text = lc.ToString();
            vetinhieu(la, 9, lc);
            pictureBox9.Image = bm8;

            lc = 0.09;
            heso10.Text = lc.ToString();
            vetinhieu(la, 10, lc);
            pictureBox10.Image = bm9;

            lc = 0.1;
            heso11.Text = lc.ToString();
            vetinhieu(la, 11, lc);
            pictureBox11.Image = bm10;

            lc = 0.15;
            heso12.Text = lc.ToString();
            vetinhieu(la, 12, lc);
            pictureBox12.Image = bm11;

            lc = 0.20;
            heso13.Text = lc.ToString();
            vetinhieu(la, 13, lc);
            pictureBox13.Image = bm12;

            lc = 0.25;
            heso14.Text = lc.ToString();
            vetinhieu(la, 14, lc);
            pictureBox14.Image = bm13;

            label5.Text = "ok";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            label5.Text = "kekekke";

        }
    }
}
