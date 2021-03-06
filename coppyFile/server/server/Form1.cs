using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace server
{
    public partial class Form1 : Form
    {
      //  private System.ComponentModel.Container components = null;
        string server = "";
        string eu;
        ClientCollection clientcollection = null;

        string[] DFSCommandList;
        Socket serversocket;
      //  private System.Windows.Forms.Label label1;
        private string login_client_machine;
        Socket clientsock = null;
        string[] cmdList = null;
        private string shared_file_name;
        private string shared_file_path;
        private string shared_file_size;
        private string local_shared_dir = "C:\\";
        private int total_clients_connected = 0;



        public Form1()
        {
            InitializeComponent();
            textBox3.Text = server = Dns.GetHostName();
            textBox2.Text = local_shared_dir;
            this.Text += " [ " + server + " ]";

            clientcollection = new ClientCollection(); // xử lý thư mục
            // thư mục.................. hoạt động cần trỏ vào
            if (!Directory.Exists(local_shared_dir))
                Directory.CreateDirectory(local_shared_dir);

        }
        private void PopulateServer_MyFiles()
        { // hiện ra danh sách thư mục mới
            listView1.Items.Clear();
            string[] mCList = Directory.GetFiles(local_shared_dir);
            Populate(mCList); // đưa thông tinh tiệp tin ra
        }

        private void ShareClientFiles()
        {
            for (int i = 0; i < clientcollection.Count; i++)
            {
                ClientInfo obj = (ClientInfo)clientcollection.GetAt(i);
                string[] mCList = new String[4];
                mCList[0] = obj.sharedfileName;
                mCList[1] = obj.sharedfilesSize;
                mCList[2] = obj.sharedfilesPath;
                mCList[3] = obj.username;
                listView1.Items.Add(new ListViewItem(mCList));
            }
            listView1.Refresh();
        }

        private void Populate(string[] mCList)
        {
            for (int i = 0; i < mCList.Length; i++)
            {
                FileInfo fi = new FileInfo(mCList[i]);  // đưa thông tin file ra
                string[] mDesc = new string[3];
                mDesc[0] = fi.Name;                      //têm
                mDesc[1] = fi.Length.ToString();
                mDesc[2] = fi.FullName;                    // độ dài
                listView1.Items.Add(new ListViewItem(mDesc));  // thêm vào listview

                ClientInfo obj = new ClientInfo();
                obj.username = server;           // thông tin quan client
                obj.password = "";
                obj.sharedfileName = mDesc[0];
                obj.sharedfilesPath = mDesc[2];
                obj.sharedfilesSize = mDesc[1];
                clientcollection.AddClient(obj);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            PopulateServer_MyFiles();
            Thread th = new Thread(new ThreadStart(ListenForPeers)); //t ạo luồng nhận thông tiun
            th.Start(); // bắt đầu quá trình
        }
        public void ListenForPeers()
        {
            
            try
            {
                serversocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                serversocket.Blocking = true;

                IPHostEntry IPHost = Dns.Resolve(server);
                string[] aliases = IPHost.Aliases;
                IPAddress[] addr = IPHost.AddressList;
               
                IPEndPoint ipepServer = new IPEndPoint(addr[0], 8090);  // thông tin Ip và phương thức kết nối
                 IP_ADDRESS.Text= addr[0].ToString();
                serversocket.Bind(ipepServer);
                serversocket.Listen(-1);  // lắng nghe thông tin
               
                while (true)
                {
                    clientsock = serversocket.Accept();  // chaapos nhận 
                    if (clientsock.Connected)
                    {
                        total_clients_connected++;
                        AppendText("Client connected...");
                        Thread tc = new Thread(new ThreadStart(listenclient));
                        tc.Start();
                    }
                }
            }
            catch (SocketException se)
            {
                Console.WriteLine(se.Message);
            }
            catch (Exception eee)
            {
                MessageBox.Show("Socket Connect Erroraaaaaa.\n\n" + eee.Message + "\nPossible Cause: Server Already running. Check the tasklist for running processes", "Startup Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
        }

        void listenclient()
        {
            Socket sock = clientsock;
            string cmd = server;
            byte[] sender = System.Text.Encoding.ASCII.GetBytes("SERVER " + cmd);
            sock.Send(sender, sender.Length, 0); // gửi thông tin server

            try
            {
                while (sock != null)
                {
                    cmd = "";
                    byte[] recs = new byte[32767];
                    int rcount = sock.Receive(recs, recs.Length, 0);
                    string clientmessage = System.Text.Encoding.ASCII.GetString(recs); // chuyen doi snag chuoi
                    clientmessage = clientmessage.Substring(0, rcount);

                    string smk = clientmessage;

                    cmdList = null;
                    cmdList = clientmessage.Split(' ');
                    string execmd = cmdList[0];
                    AppendText("COMMAND==>" + execmd);

                    sender = null;
                    sender = new Byte[32767];

                    Console.WriteLine("CLIENT COMMAND = " + execmd + "\r\n");

                    string parm1 = "";
                    if (execmd == "USER")
                    {

                        login_client_machine = cmdList[2];
                        string pass = cmdList[3];
                        string ip = cmdList[1];
                        AppendText("Client Connected IP:" + ip + " User :" + login_client_machine);
                        continue;
                    }

                    if (execmd == "GET") // lấy file gửi qua ben kia 
                    {
                        // GET <FileName> <FileSize>
                        for (int i = 1; i < cmdList.Length - 1; i++)
                            parm1 = parm1 + " " + cmdList[i];
                        parm1 = parm1.Trim();
                        FileInfo fi = new FileInfo(parm1);
                        if (fi.Exists)  // có file thì gửi ok tự tạo thêm 1 vòng lặp nữa
                            cmd = "GETOK ";
                        else
                            cmd = "GETOK_FAILED "; // ko file thì lỗi
                        sender = System.Text.Encoding.ASCII.GetBytes(cmd);
                        sock.Send(sender, sender.Length, 0);  // gửi tin...................
                        continue;
                    }

                    if (execmd == "LISTING")
                    {
                        PopulateList(clientmessage);
                        continue;
                    }

                    if (execmd == "SERVER_LISTING")
                    {
                        sender = new Byte[32767];
                        cmd = "LISTING \r\n";
                        parm1 = cmdList[1];

                        cmd = cmd + SearchDatabase(parm1); // tìm xem danh sách file có ko, nếu ko có thù gửi lệnh cmd

                        sender = System.Text.Encoding.ASCII.GetBytes(cmd);
                        sock.Send(sender, sender.Length, 0);
                        continue;
                    }

                    if (execmd == "NOOP")
                    {
                        // do nothing 
                        continue;
                    }

                    if (execmd == "CLIENT_DISCONNECTING")
                    {
                        cmd = "CLIENT_CLOSE";
                        sender = System.Text.Encoding.ASCII.GetBytes(cmd);
                        sock.Send(sender, sender.Length, 0);

                        sock.Close();
                        AppendText("Client Disconnected");
                        continue;
                    }


                    if (execmd == "FILEOK")
                    {
                        cmd = "NOOP ";
                        sender = System.Text.Encoding.ASCII.GetBytes(cmd);
                        sock.Send(sender, sender.Length, 0);
                        continue;
                    }

                    if (execmd == "GETOK")
                    {
                        cmd = "BEGINSEND " + shared_file_path + " " + shared_file_size; //taok lệnh kết thúc nhận
                        sender = new Byte[1024];
                        sender = Encoding.ASCII.GetBytes(cmd);
                        sock.Send(sender, sender.Length, 0);
                        ServerDownloadingFromClient(sock); // tải file từ client
                        continue;
                    }

                    if (execmd == "BEGINSEND")
                    {
                        ClientDownloadingFromServer(cmdList, sock);
                        continue;
                    }
                }
            }
            catch (Exception Se)
            {
                string s = Se.Message;
                Console.WriteLine(s);
            }
        }

        void AppendText(string mtxt)
        {
            mtxt = mtxt + "\r\n";
            textBox1.AppendText(mtxt);
            textBox1.Select(textBox1.Text.Length, 0);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Socket sock = clientsock;

            if (SearchText.Text == "") return;
            if (sock == null)
            {
                MessageBox.Show("Client not connected", "Connect Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            string cmd = "CLIENT_LISTING " + SearchText.Text + " ";
            byte[] b = System.Text.Encoding.ASCII.GetBytes(cmd);
            sock.Send(b, b.Length, 0);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Socket sock = clientsock;
            int nPos = listView2.SelectedIndices.Count;
            if (nPos <= 0)
            {
                MessageBox.Show("Please select a file to download.", "Question", MessageBoxButtons.OK, MessageBoxIcon.Question);
                return;

            }

            IEnumerator selCol = listView2.SelectedItems.GetEnumerator();
            selCol.MoveNext();
            ListViewItem lv = (ListViewItem)selCol.Current;
            shared_file_name = lv.SubItems[0].Text;
            shared_file_size = lv.SubItems[1].Text;
            shared_file_path = lv.SubItems[2].Text;

            // Send a Get to command .Server will reply GETOK if the file is avaialble
            string cmd = "GET " + shared_file_path + " " + shared_file_size;
            Byte[] sb = new Byte[1024];
            sb = Encoding.ASCII.GetBytes(cmd);
            sock.Send(sb, sb.Length, 0);
        }
        void PopulateList(string servermessage)
        { // hien thi file can down ;load
            listView2.Items.Clear();
            string[] lines = servermessage.Split('\n');
            for (int i = 1; i < lines.Length - 1; i++)
            {
                lines[i] = lines[i].Substring(0, lines[i].Length - 1);
                string[] listviewitems = lines[i].Split(',');
                listView2.Items.Add(new ListViewItem(listviewitems));
            }
        }

        private void tabControl1_SelectedIndexChanged(object sender, System.EventArgs e)
        {
          //  int pos = tabControl1.SelectedIndex;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (!Directory.Exists(textBox2.Text))
            {
                MessageBox.Show("Directory does not exist", "Folder Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            local_shared_dir = textBox2.Text;
            //tabControl1.SelectedTab = tabPage2;
            PopulateServer_MyFiles();

        }

        private void Disconnect()
        {
            if (clientsock != null)
            {
                if (!clientsock.Connected)
                    return;
            }
            else
                return;

            string cmd = "SERVER_DISCONNECT ";
            Byte[] sb = new Byte[1024];
            sb = Encoding.ASCII.GetBytes(cmd);
            clientsock.Send(sb, sb.Length, 0);
            clientsock = null;
        }
        void ServerDownloadingFromClient(Socket s)
        {
          //  int cnt = listView3.Items.Count;
            Socket sock = s;
            string[] mDownloading = new String[5];

            FileStream fout = new FileStream(local_shared_dir + "\\" + shared_file_name, FileMode.Create, FileAccess.Write);
            NetworkStream nfs = new NetworkStream(sock);
            long size = int.Parse(shared_file_size);
            long rby = 0;

            mDownloading[0] = shared_file_name;  //tách thành phần file ra
            mDownloading[1] = size.ToString();
            mDownloading[2] = "0";
            mDownloading[3] = "";
            mDownloading[4] = login_client_machine;

            //listView3.Items.Add(new ListViewItem(mDownloading));

            try
            {
                //loop till the Full bytes have been read
                while (rby < size)
                {
                    byte[] buffer = new byte[1024];
                    //Read from the Network Stream
                    int i = nfs.Read(buffer, 0, buffer.Length);
                    fout.Write(buffer, 0, (int)i);
                    rby = rby + i;

                    int pc = (int)(((double)rby / (double)size) * 100.00);
                    string perc = pc.ToString() + "%";
                  //  listView3.Items[cnt].SubItems[3].Text = perc;
                 //   listView3.Items[cnt].SubItems[2].Text = rby.ToString();

                }
                fout.Close();
                string cmd = "FILEOK";
                Byte[] sender = new Byte[1024];
                sender = new Byte[1024];
                sender = Encoding.ASCII.GetBytes(cmd);
                sock.Send(sender, sender.Length, 0);
            }
            catch (Exception ed)
            {
                Console.WriteLine("A Exception occured in file transfer" + ed.ToString());
            }
        }

        void ClientDownloadingFromServer(string[] cmdList, Socket s)
        {
            DFSCommandList = cmdList;
         //   int cnt = listView4.Items.Count;
            string[] mUploading = new String[5];
            Socket sock = s;
            string parm1 = "";
            string parm2 = "";

            for (int i = 1; i < DFSCommandList.Length - 1; i++)
                parm1 = parm1 + " " + DFSCommandList[i];
            parm1 = parm1.Trim();
            parm2 = DFSCommandList[DFSCommandList.Length - 1];

            try
            {
                FileInfo ftemp = new FileInfo(parm1);
                long total = ftemp.Length;
                long rdby = 0;
                int len = 0;

                mUploading[0] = parm1;
                mUploading[1] = total.ToString();
                mUploading[2] = "0";
                mUploading[3] = "";
                mUploading[4] = login_client_machine;

                //listView4.Items.Add(new ListViewItem(mUploading));

                byte[] buffed = new byte[1024];
                //Open the file requested for download 
                FileStream fin = new FileStream(parm1, FileMode.Open, FileAccess.Read);
                //One way of transfer over sockets is Using a NetworkStream 
                //It provides some useful ways to transfer data 
                NetworkStream nfs = new NetworkStream(sock);

                //lock the Thread here
                //				lock(this)
                while (rdby < total && nfs.CanWrite)
                {
                    //Read from the File (len contains the number of bytes read)
                    len = fin.Read(buffed, 0, buffed.Length);
                    //Write the Bytes on the Socket
                    nfs.Write(buffed, 0, len);
                    //Increase the bytes Read counter
                    rdby = rdby + len;

                    int pc = (int)(((double)rdby / (double)total) * 100.00);
                    string perc = pc.ToString() + "%";
                   // listView4.Items[cnt].SubItems[3].Text = perc;
                    //listView4.Items[cnt].SubItems[2].Text = rdby.ToString();
                }
                //Display a Message Showing Sucessful File Transfer
                fin.Close();
            }
            catch (Exception ed)
            {
                Console.WriteLine("A Exception occured in transfer" + ed.ToString());
            }
        }

        string SearchDatabase(string pattern)
        {
            string retCmd = "";
            ArrayList arr = clientcollection.SearchFiles(pattern);
            for (int i = 0; i < arr.Count; i++)
            {
                ClientInfo obj = (ClientInfo)arr[i];
                retCmd = retCmd + obj.sharedfileName + "," + obj.sharedfilesSize + "," + obj.sharedfilesPath + "," + obj.username + "\r\n";
            }
            return retCmd;
        }


    }
}
