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


namespace client
{
    public partial class Form1 : Form
    {

        private IContainer componentsa;
        string server = "";
        ClientCollection clientcollection = null;
        private string local_shared_dir = "C:\\TEMP"; 
        string[] DFSCommandList;
        Socket serversocket;
       // private System.Windows.Forms.Label label1;
        Socket clientsock = null;
        string[] cmdList = null;
        private string shared_file_name;
        private string shared_file_path;
        private string shared_file_size;
        private int total_clients_connected = 0;

        public Form1()
        {
            InitializeComponent();
            this.componentsa = new System.ComponentModel.Container();
        }



        private void PopulateServer_MyFiles()
        {
            listView1.Items.Clear();
            string[] mCList = Directory.GetFiles(local_shared_dir);
            Populate(mCList);
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
                FileInfo fi = new FileInfo(mCList[i]);
                string[] mDesc = new string[3];
                mDesc[0] = fi.Name;
                mDesc[1] = fi.Length.ToString();
                mDesc[2] = fi.FullName;
                listView1.Items.Add(new ListViewItem(mDesc));

                ClientInfo obj = new ClientInfo();
                obj.username = server;
                obj.password = "";
                obj.sharedfileName = mDesc[0];
                obj.sharedfilesPath = mDesc[2];
                obj.sharedfilesSize = mDesc[1];
                clientcollection.AddClient(obj);
            }
        }




        private void button4_Click(object sender, EventArgs e)
        {
            Connect();
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

            string cmd = "CLIENT_DISCONNECTING ";
            Byte[] sb = new Byte[1024];
            sb = Encoding.ASCII.GetBytes(cmd);
            clientsock.Send(sb, sb.Length, 0);
            clientsock = null;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            textBox3.Text = server = Dns.GetHostName();
			textBox2.Text = local_shared_dir ;
			this.Text += " [ " + server + " ]" ;

			clientcollection = new ClientCollection() ;

        }


        private void Connect()
        {
            if (!Directory.Exists(local_shared_dir))
                Directory.CreateDirectory(local_shared_dir);

            try
            {
                serversocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                serversocket.Blocking = true;

                IPHostEntry IPHost = Dns.Resolve(textBox1.Text);
                string[] aliases = IPHost.Aliases;
                IPAddress[] addr = IPHost.AddressList;

                IPEndPoint ipepServer = new IPEndPoint(addr[0], 8090);
                serversocket.Connect(ipepServer);
                clientsock = serversocket;

                Thread MainThread = new Thread(new ThreadStart(listenclient));
                MainThread.Start();

                button1.Enabled = true;
                button2.Enabled = true;

                PopulateServer_MyFiles();
            }
            catch (SocketException se)
            {
                Console.WriteLine(se.Message);
                AppendText(se.Message);
            }
            catch (Exception eee)
            {
                MessageBox.Show("Socket Connect Error.\n\n" + eee.Message + "\nPossible Cause: Server Already running. Check the tasklist for running processes", "Startup Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
        }

        void listenclient()
        {
            Socket sock = clientsock;
            string cmd = server;
            byte[] sender = System.Text.Encoding.ASCII.GetBytes("CLIENT " + cmd);
            sock.Send(sender, sender.Length, 0);
            try
            {
                while (sock != null)
                {
                    cmd = "";
                    byte[] recs = new byte[32767];
                    int rcount = sock.Receive(recs, recs.Length, 0);
                    string clientmessage = System.Text.Encoding.ASCII.GetString(recs);
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
                    if (execmd == "SERVER")
                    {
                        AppendText("Connected TO Server :" + cmdList[1]);
                        continue;
                    }

                    if (execmd == "GET")
                    {
                        // GET <FileName> <FileSize>
                        for (int i = 1; i < cmdList.Length - 1; i++)
                            parm1 = parm1 + " " + cmdList[i];
                        parm1 = parm1.Trim();
                        FileInfo fi = new FileInfo(parm1);
                        if (fi.Exists)
                            cmd = "GETOK ";
                        else
                            cmd = "GETOK_FAILED ";
                        sender = System.Text.Encoding.ASCII.GetBytes(cmd);
                        sock.Send(sender, sender.Length, 0);
                        continue;
                    }

                    if (execmd == "LISTING")
                    {
                        PopulateList(clientmessage);
                        continue;
                    }

                    if (execmd == "SERVER_LISTING" || execmd == "CLIENT_LISTING")
                    {
                        sender = new Byte[32767];
                        cmd = "LISTING \r\n";
                        parm1 = cmdList[1];

                        cmd = cmd + SearchDatabase(parm1);

                        sender = System.Text.Encoding.ASCII.GetBytes(cmd);
                        sock.Send(sender, sender.Length, 0);
                        continue;
                    }

                    if (execmd == "NOOP")
                    {
                        // do nothing 
                        continue;
                    }

                    if (execmd == "DISCONNECT")
                    {
                        total_clients_connected++;
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
                        cmd = "BEGINSEND " + shared_file_path + " " + shared_file_size;
                        sender = new Byte[1024];
                        sender = Encoding.ASCII.GetBytes(cmd);
                        sock.Send(sender, sender.Length, 0);
                        CLIENT_DOWNLOADING(sock);
                        continue;
                    }

                    if (execmd == "BEGINSEND")
                    {
                        //ClientDownloadingFromServer(cmdList , sock);
                        SERVER_DOWNLOADING(cmdList, sock);
                        continue;
                    }

                    if (execmd == "CLIENT_CLOSE")
                    {
                        break;
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
            listBox1.Items.Add(mtxt);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Socket sock = serversocket;

            if (SearchText.Text == "") return;
            if (sock == null)
            {
                MessageBox.Show("Client not connected", "Connect Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            string cmd = "SERVER_LISTING " + SearchText.Text + " ";
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
        {
            listView2.Items.Clear();
            string[] lines = servermessage.Split('\n');
            for (int i = 1; i < lines.Length - 1; i++)
            {
                lines[i] = lines[i].Substring(0, lines[i].Length - 1);
                string[] listviewitems = lines[i].Split(',');
                listView2.Items.Add(new ListViewItem(listviewitems));
            }
        }

        void CLIENT_DOWNLOADING(Socket s)
        {
          //  int cnt = listView3.Items.Count;
            Socket sock = s;
            string[] mDownloading = new String[5];

            FileStream fout = new FileStream(local_shared_dir + "\\" + shared_file_name, FileMode.Create, FileAccess.Write);
            NetworkStream nfs = new NetworkStream(sock);
            long size = int.Parse(shared_file_size);
            long rby = 0;
            string login_client_machine = "";

            mDownloading[0] = shared_file_name;
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
                    //listView3.Items[cnt].SubItems[2].Text = rby.ToString();

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
                MessageBox.Show(ed.Message);
            }
        }

        void SERVER_DOWNLOADING(string[] cmdList, Socket s)
        {
            DFSCommandList = cmdList;
            //int cnt = listView4.Items.Count;
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
                string login_client_machine = "";

                mUploading[0] = parm1;
                mUploading[1] = total.ToString();
                mUploading[2] = "0";
                mUploading[3] = "";
                mUploading[4] = login_client_machine;

               // listView4.Items.Add(new ListViewItem(mUploading));

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
                MessageBox.Show(ed.Message);
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

        private void button5_Click(object sender, EventArgs e)
        {
            Disconnect();
        }




    }
}
