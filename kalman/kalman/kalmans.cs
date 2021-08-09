using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace kalman
{
    class kalmans
    {
        double Q_angle = 0.003; //0.001    //0.005
        double Q_gyro = 0.003;  //0.003  //0.0003
        double R_angle = 0.03;  //0.03     //0.008

        double x_bias = 0;
        double P_00 = 0, P_01 = 0, P_10 = 0, P_11 = 0;
        double y, S;
        double K_0, K_1;
        double x_angle;
       /* kalmans() {
            Q_angle = 0.005;
            Q_gyro = 0.003;
            R_angle = 0.03;
            x_bias = 0;
            P_00 = 0;
            P_10 = 0
            P_01 = 0;
            P_11 = 0;
        }
        */
       public double kalmanCalculate(double newAngle, double newRate, double looptime)
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


    }
}
