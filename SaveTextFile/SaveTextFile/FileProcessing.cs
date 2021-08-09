using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
//using System.Collections;

namespace SaveTextFile
{
    class FileProcessing
    {
       private string path_file ;
       
       private float[] coefficient;
       private int n; //so luong mau muon hien thi
          //Ham khoi tao
       public FileProcessing(string name)
       {
           path_file = name;
           
       }
        ////////////////////////
        //Ham doc file
        private void FileRead ()
        {
            StreamReader myreader = new StreamReader(path_file);
            coefficient = new float[n];
            for (int i = 0 ; i< n ; i ++)
            {
                string s = myreader.ReadLine();
                coefficient[i] = (float)(Convert.ChangeType(s,typeof(float)));

            }
            myreader.Close();  //dong file
            myreader.Dispose();//realise 
           
        }
        public void Data_Coefficient (out float [] output)
        {
            output = new float[n];
            FileRead();
            for (int i = 0 ; i < n ; i ++)
            {
                output[i] = coefficient[i];
            }
        }
        /***************************/
        //Ham luu data vao file
        public void SaveData(int[] data_save)
        {
            if (File.Exists(path_file) == true)  //kiem tra xem file co ton tai khong
            {
                File.Delete(path_file); //xoa file cu di
            }
            StreamWriter my_write = new StreamWriter(path_file, true);
            for (int i = 0; i < data_save.Length; i++)
                {
                    my_write.WriteLine(data_save[i].ToString());
                }
         my_write.WriteLine("Length : {0}", data_save.Length);
            my_write.Close();
            my_write.Dispose();
        }
    }
}


