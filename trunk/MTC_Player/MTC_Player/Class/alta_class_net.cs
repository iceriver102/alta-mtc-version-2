using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Windows;
using System.IO;
using System.Windows.Media;
using System.Runtime.Serialization.Formatters.Binary;
using MTC_Player;
using Code.User;
namespace Alta.Class
{
    public class alta_class_net
    {
        private ArrayList m_aryClients = new ArrayList();
        private string keySerect;
        Socket listener;
        public _controlVLC dataControl = _controlVLC._CONTROL_FREE;
        public _controlVLC adminControl = _controlVLC._CONTROL_FREE;
        public String ipHostStream = "";
        public alta_class_net(string _key, int nPortListen = 339)
        {

            this.keySerect = _key;
            // Determine the IPAddress of this machine
            List<IPAddress> aryLocalAddr = new List<IPAddress>();
            String strHostName = "";

            try
            {
                // NOTE: DNS lookups are nice and all but quite time consuming.
                strHostName = Dns.GetHostName();
                IPHostEntry ipEntry = Dns.GetHostEntry(strHostName);
                foreach (IPAddress ip in ipEntry.AddressList)
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                    {
                        aryLocalAddr.Add(ip);
                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error trying to get local address {0} ", ex.Message);
                aryLocalAddr.Clear();
                aryLocalAddr.Add(IPAddress.Loopback);
            }

            // Verify we got an IP address. Tell the user if we did
            if (aryLocalAddr == null || aryLocalAddr.Count < 1)
            {
                Console.WriteLine("Unable to get local address");
                return;
            }
            Console.WriteLine("Listening on : [{0}] {1}:{2}", strHostName, aryLocalAddr[0], nPortListen);

            // Create the listener socket in this machines IP address
            listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            listener.Bind(new IPEndPoint(aryLocalAddr[0], nPortListen));
            //listener.Bind( new IPEndPoint( IPAddress.Loopback, 399 ) );	// For use with localhost 127.0.0.1
            listener.Listen(10);
            // Setup a callback to be notified of connection requests
            listener.BeginAccept(new AsyncCallback(this.OnConnectRequest), listener);
        }
        ~alta_class_net()
        {
            listener.Close();
            listener.Dispose();
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        /// <summary>
        /// Callback used when a client requests a connection. 
        /// Accpet the connection, adding it to our list and setup to 
        /// accept more connections.
        /// </summary>
        /// <param name="ar"></param>
        public void OnConnectRequest(IAsyncResult ar)
        {
            try
            {
                Socket listener = (Socket)ar.AsyncState;
                NewConnection(listener.EndAccept(ar));
                listener.BeginAccept(new AsyncCallback(OnConnectRequest), listener);
            }
            catch (Exception ex)
            {
#if DEBUG
                MessageBox.Show(ex.Message);
#endif
            }
        }
        /// <summary>
        /// Add the given connection to our list of clients
        /// Note we have a new friend
        /// Send a welcome to the new client
        /// Setup a callback to recieve data
        /// </summary>
        /// <param name="sockClient">Connection to keep</param>
        //public void NewConnection( TcpListener listener )
        public void NewConnection(Socket sockClient)
        {
            // Program blocks on Accept() until a client connects.
            //SocketChatClient client = new SocketChatClient( listener.AcceptSocket() );
            SocketControlClient client = new SocketControlClient(sockClient);
            m_aryClients.Add(client);
            Console.WriteLine("Client {0}, joined", client.Sock.RemoteEndPoint);
            // Get current date and time.           
            String strDateLine = "NOONCE|" + this.keySerect;

            // Convert to byte array and send.
            Byte[] byteDateLine = System.Text.Encoding.ASCII.GetBytes(strDateLine.ToCharArray());
            client.Sock.Send(byteDateLine, byteDateLine.Length, 0);
            client.SetupRecieveCallback(this);
        }
        private byte[] getByteText(String input)
        {
            byte[] array = System.Text.Encoding.Default.GetBytes(input);
            return array;
        }

        /// <summary>
        /// Get the new data and send it out to all other connections. 
        /// Note: If not data was recieved the connection has probably 
        /// died.
        /// </summary>
        /// <param name="ar"></param>
        public void OnRecievedData(IAsyncResult ar)
        {
            SocketControlClient client = (SocketControlClient)ar.AsyncState;
            byte[] aryRet = client.GetRecievedData(ar);
            if (aryRet.Length < 1)
            {
                Console.WriteLine("Client {0}, disconnected", client.Sock.RemoteEndPoint);
                client.Sock.Close();
                m_aryClients.Remove(client);
                return;
            }
            string str = System.Text.Encoding.Default.GetString(aryRet);
            Console.WriteLine(str);
            if (client.type == TYPE.NONE)
            {
                string[] dataStr = str.Split('_');
                bool flag = this.CheckControl(ref client, dataStr);
                if (!flag)
                {
                    client.Sock.Send(getByteText("FAIL|200|CONTROL"));
                    client.Sock.Close();
                    m_aryClients.Remove(client);
                }
                else
                {
                    client.Sock.Send(getByteText("OK|200|CONTROL"));
                    client.SetupRecieveCallback(this);
                }
            }
            else
            {
                #region Comment

                //    if (dataStr.Length > 1 & dataStr[0].ToUpper() == "ADMIN")
                //    {
                //        if (CommonUtilities.checkAdminColtrol(dataStr[1]))
                //        {
                //            client.flag_admin_Control = true;
                //            client.flag_can_Coltrol = true;
                //            client.Sock.Send(getByteText("OK|200|ADMIN"));
                //            client.SetupRecieveCallback(this);
                //        }
                //        else
                //        {
                //            client.flag_can_Coltrol = false;
                //            client.Sock.Send(getByteText("FAIL|205|ADMIN"));
                //            client.Sock.Close();
                //            m_aryClients.Remove(client);
                //        }
                //    }
                //    else
                //    {
                //        if (CommonUtilities.userColtrol.checkLoginNetWork(CommonUtilities.keySerect,str) && flag_login_user)
                //        {
                //            client.flag_admin_Control = false;
                //            client.flag_can_Coltrol = true;
                //            client.Sock.Send(getByteText("OK|200"));
                //            client.SetupRecieveCallback(this);

                //        }
                //        else
                //        {
                //            client.flag_admin_Control = false;
                //            client.flag_can_Coltrol = false;
                //            client.Sock.Send(getByteText("FAIL|205"));
                //            client.Sock.Close();
                //            m_aryClients.Remove(client);
                //        }

                //    }
                //}
                //else
                //{
                //    if (client.flag_admin_Control)
                //    {
                //        if (str.ToUpper() == "PLAY")
                //        {
                //            this.adminControl = _controlVLC._CONTROL_PLAY;
                //            this.PlayVlc(this, client.Sock.RemoteEndPoint);
                //        }
                //        else if (str.ToUpper() == "PAUSE")
                //        {
                //            this.adminControl = _controlVLC._CONTROL_PAUSE;
                //        }
                //        else if (str.ToUpper() == "STOP")
                //        {
                //            this.adminControl = _controlVLC._CONTROL_STOP;
                //            this.StopVlc(this, client.Sock.RemoteEndPoint);

                //        }
                //        else if (str.ToUpper() == "NEXT")
                //        {
                //            this.adminControl = _controlVLC._CONTROL_NEXT;
                //        }
                //        else if (str.ToUpper() == "BACK")
                //        {
                //            this.adminControl = _controlVLC._CONTROL_BACK;
                //        }
                //        else if (str.ToUpper() == "STREAM")
                //        {
                //            this.adminControl = _controlVLC._CONTROL_STREAM;
                //            this.ipHostStream = parseIP(client.Sock.RemoteEndPoint);
                //            if (PlayStreaming != null)
                //            {
                //                PlayStreaming(this, this.ipHostStream);
                //            }
                //        }
                //        else if(str.ToUpper()=="ABORT")
                //        {
                //            this.adminControl = _controlVLC._CONTROL_ABORT_USER;
                //        }
                //        else if (str.ToUpper() == "ACCEPT")
                //        {
                //            this.adminControl = _controlVLC._CONTROL_ACCEPT_USER;
                //        }
                //        else if (str.ToUpper() == "SCREEN")
                //        {
                //            if (SendPlaying != null)
                //            {
                //                SendPlaying(this, client.Sock.RemoteEndPoint);
                //            }
                //            this.adminControl = _controlVLC._CONTROL_SCREEN;
                //        }
                //        else if (str.ToUpper() == "TURN_OFF")
                //        {
                //            this.adminControl = _controlVLC._CONTROL_OFF;
                //            if(this.TurnOffApp!=null)
                //                TurnOffApp(this, client.Sock.RemoteEndPoint);
                //        }
                //        else if (str.ToUpper() == "STOPSTREAM")
                //        {
                //            this.dataControl = _controlVLC._CONTROL_STREAM_STOP;
                //            if (StopStreaming != null)
                //                StopStreaming(this, new EventArgs());

                //        }
                //        else
                //        {
                //            this.adminControl = _controlVLC._CONTROL_FREE;
                //        }
                //    }
                //    else
                //    {
                //        if (str.ToUpper() == "PLAY")
                //        {
                //            this.dataControl = _controlVLC._CONTROL_PLAY;
                //            if (this.PlayVlc!=null)
                //            {
                //                this.PlayVlc(this, client.Sock.RemoteEndPoint);
                //            }
                //        }
                //        else if (str.ToUpper() == "PAUSE")
                //        {
                //            this.dataControl = _controlVLC._CONTROL_PAUSE;
                //        }
                //        else if (str.ToUpper() == "STOP")
                //        {
                //            this.dataControl = _controlVLC._CONTROL_STOP;
                //            if (this.StopVlc != null)
                //            {
                //                this.StopVlc(this, client.Sock.RemoteEndPoint);
                //            }
                //        }
                //        else if (str.ToUpper() == "NEXT")
                //        {
                //            this.dataControl = _controlVLC._CONTROL_NEXT;
                //        }
                //        else if (str.ToUpper() == "BACK")
                //        {
                //            this.dataControl = _controlVLC._CONTROL_BACK;
                //        }
                //        else if (str.ToUpper() == "STREAM")
                //        {
                //            this.dataControl = _controlVLC._CONTROL_STREAM;
                //            this.ipHostStream = parseIP(client.Sock.RemoteEndPoint);
                //            if (PlayStreaming != null)
                //            {
                //                PlayStreaming(this, this.ipHostStream);
                //            }

                //        }
                //        else if (str.ToUpper() == "SCREEN"){
                //            if (SendPlaying != null)
                //            {
                //                SendPlaying(this, client.Sock.RemoteEndPoint);
                //            }
                //            this.dataControl = _controlVLC._CONTROL_SCREEN;
                //        }
                //        else if (str.ToUpper() == "STOPSTREAM")
                //        {
                //            this.dataControl = _controlVLC._CONTROL_STREAM_STOP;
                //            if (StopStreaming != null)
                //                StopStreaming(this, new EventArgs());

                //        }
                //        else
                //        {
                //            this.dataControl = _controlVLC._CONTROL_FREE;
                //        }
                //    }
                #endregion Comment

                this.RunCMD(client, str);
                client.Sock.Send(getByteText("OK|200|" + str.ToUpper()));
                client.SetupRecieveCallback(this);
            }
           
        }
        public event EventHandler<EndPoint> SendPlaying;
        public event EventHandler<EndPoint> StopVlc;
        public event EventHandler<EndPoint> PlayVlc;
        public event EventHandler<EndPoint> TurnOffApp;

        private bool CheckControl(ref SocketControlClient client, string[] cmd)
        {
            if (cmd.Length > 1 & cmd[0].ToUpper() == "ADMIN")
            {
                UserData admin = App.checkAdmin(cmd[1]);
                if (admin != null)
                {
                    client.type = TYPE.ADMIN;
                }
                else
                {
                    client.type = TYPE.NONE;
                }
            }
            else if(cmd.Length>1)
            {
                bool flag = App.checkControl(cmd[1]);
                if (flag)
                {
                    client.type = TYPE.USER;
                }
                else
                {
                    client.type = TYPE.NONE;
                }
            }
            return client.type != TYPE.NONE;
        }

        private void RunCMD(SocketControlClient client, string cmd)
        {
            if (client.type != TYPE.NONE && cmd.Length>0) 
            {
                if (cmd.ToUpper() == "PLAY")
                {
                    if (this.PlayVlc != null)
                    {
                        this.PlayVlc(this, client.Sock.RemoteEndPoint);
                    }
                }
                else if (cmd.ToUpper() == "STOP")
                {
                    if (this.StopVlc != null)
                    {
                        this.StopVlc(this, client.Sock.RemoteEndPoint);
                    }
                }
                else if (cmd.ToUpper() == "TURNOFF" && client.type== TYPE.ADMIN)
                {
                      if(this.TurnOffApp!=null)
                            TurnOffApp(this, client.Sock.RemoteEndPoint);
                }
                else if (cmd.ToUpper() == "SCREEN")
                {
                    if (this.SendPlaying != null)
                    {
                        this.SendPlaying(this, client.Sock.RemoteEndPoint);
                    }
                }
            }
        }

        /// <summary>
        /// Gui tin nhan den tat ca cac may
        /// </summary>
        /// <param name="Msg"></param>
        public void SendMsg(String Msg)
        {
            int count = this.m_aryClients.Count;
            if (count > 0)
            {
                for (int i = 0; i < count; i++)
                {
                    SocketControlClient client = this.m_aryClients[i] as SocketControlClient;
                    try
                    {
                        client.Sock.Send(getByteText(Msg));
                    }
                    catch (Exception ex)
                    {
#if DEBUG
                        MessageBox.Show(ex.Message);
#endif
                    }
                }
            }
        }

        public void SendMsg(String Msg, EndPoint ip)
        {
            int count = this.m_aryClients.Count;
            if (count > 0)
            {
                for (int i = 0; i < count; i++)
                {
                    SocketControlClient client = this.m_aryClients[i] as SocketControlClient;
                    if (client.Sock.RemoteEndPoint == ip)
                    {
                        client.Sock.Send(getByteText(Msg));
                        return;
                    }

                }
            }
        }
        /// <summary>
        /// chuyen IP thanh String;
        /// </summary>
        /// <param name="ip">IP can chuyen</param>
        /// <returns>String IP</returns>


        String parseIP(EndPoint ip)
        {
            String tmp = ip.ToString();
            string[] rIP = tmp.Split(':');
            return rIP[0];
        }

        public bool checkClient(EndPoint ip)
        {
            foreach (SocketControlClient client in this.m_aryClients)
            {
                if (client.Sock.RemoteEndPoint == ip)
                    return true;
            }
            return false;
        }

        public bool flag_login_user = true;
    }

    public enum TYPE
    {
        ADMIN,USER,NONE
    }

    internal class SocketControlClient
    {
        public TYPE type = TYPE.NONE;
        private Socket m_sock;						// Connection to the client
        private byte[] m_byBuff = new byte[1024];		// Receive data buffer
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="sock">client socket conneciton this object represents</param>
        public SocketControlClient(Socket sock)
        {
            
            m_sock = sock;
        }



        // Readonly access
        public Socket Sock
        {
            get { return m_sock; }
        }

        /// <summary>
        /// Setup the callback for recieved data and loss of conneciton
        /// </summary>
        /// <param name="app"></param>
        public void SetupRecieveCallback(alta_class_net app)
        {
            try
            {
                AsyncCallback recieveData = new AsyncCallback(app.OnRecievedData);
                m_sock.BeginReceive(m_byBuff, 0, m_byBuff.Length, SocketFlags.None, recieveData, this);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Recieve callback setup failed! {0}", ex.Message);
            }
        }

        /// <summary>
        /// Data has been recieved so we shall put it in an array and
        /// return it.
        /// </summary>
        /// <param name="ar"></param>
        /// <returns>Array of bytes containing the received data</returns>
        public byte[] GetRecievedData(IAsyncResult ar)
        {
            int nBytesRec = 0;
            try
            {
                nBytesRec = m_sock.EndReceive(ar);
            }
            catch { }
            byte[] byReturn = new byte[nBytesRec];
            Array.Copy(m_byBuff, byReturn, nBytesRec);

            /*
            // Check for any remaining data and display it
            // This will improve performance for large packets 
            // but adds nothing to readability and is not essential
            int nToBeRead = m_sock.Available;
            if( nToBeRead > 0 )
            {
                byte [] byData = new byte[nToBeRead];
                m_sock.Receive( byData );
                // Append byData to byReturn here
            }
            */
            return byReturn;
        }


    }

    public enum _controlVLC
    {
        _CONTROL_PLAY,
        _CONTROL_PAUSE,
        _CONTROL_STOP,
        _CONTROL_FREE,
        _CONTROL_STREAM,
        _CONTROL_WAIT,
        _CONTROL_OPEN,
        _CONTROL_Camera,
        _CONTROL_Connect,
        _CONTROL_NEXT,
        _CONTROL_BACK,
        _CONTROL_ABORT_USER,
        _CONTROL_ACCEPT_USER,
        _CONTROL_SCREEN,
        _CONTROL_STREAM_STOP,
        _CONTROL_OFF
    }

}
