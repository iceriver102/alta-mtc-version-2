using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Alta.Class
{
    public class alta_client
    {
        public event EventHandler ConnectErr;
        public event EventHandler ConnectSuccess;
        public event EventHandler Disconnect;
        public bool canControl = false;
        // My Attributes
        private Socket m_sock;						// Server connection
        private byte[] m_byBuff = new byte[512];	// Recieved data buffer
        public string sRecieved = "";
        public bool isConnected
        {
            get
            {
                if (this.m_sock == null)
                    return false;
                return m_sock.Connected;
            }
        }
        public String ip;
        public event EventHandler<string> RecieveDataEvent;
        public alta_client()
        {
            
        }
        public void sendData(String m_tbMessage)
        {
            if (m_sock == null || !m_sock.Connected)
            {
                throw new Exception("Chưa kết nối với Server");
            }

            // Read the message from the text box and send it
            try
            {
                // Convert to byte array and send.
                byte[] byteDateLine = Encoding.ASCII.GetBytes(m_tbMessage);
                m_sock.Send(byteDateLine, byteDateLine.Length, 0);
            }
            catch (Exception)
            {
            }
        }
        public void connect(string ip, int port = 339)
        {
            try
            {
               
                // Close the socket if it is still open
                if (m_sock != null && m_sock.Connected)
                {
                    m_sock.Shutdown(SocketShutdown.Both);
                    System.Threading.Thread.Sleep(10);
                    m_sock.Close();
                }
                m_sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPEndPoint epServer = new IPEndPoint(IPAddress.Parse(ip), port);
                m_sock.Blocking = false;
                AsyncCallback onconnect = new AsyncCallback(OnConnect);
                IAsyncResult result=m_sock.BeginConnect(epServer, onconnect, m_sock);
                bool success = result.AsyncWaitHandle.WaitOne(5000, true);

                if (!success)
                {
                    m_sock.Close();
                    if (this.ConnectErr != null)
                    {
                        this.ConnectErr(this, new EventArgs());
                    }
                }
            }
            catch (Exception)
            {
                if (ConnectErr != null)
                {
                    ConnectErr(this, new EventArgs());
                }
            }
            this.ip = ip;
        }
        public void OnConnect(IAsyncResult ar)
        {
            Socket sock = (Socket)ar.AsyncState;
            try
            {
                if (sock.Connected)
                {
                    if (this.ConnectSuccess != null)
                    {
                        this.ConnectSuccess(this, new EventArgs());
                    }
                    SetupRecieveCallback(sock);
                }
                else
                {
                    if (ConnectErr != null)
                        ConnectErr(this, new EventArgs());
                }
            }
            catch (Exception)
            {
                if (ConnectErr != null)
                    ConnectErr(this, new EventArgs());
            }
        }
        public void SetupRecieveCallback(Socket sock)
        {
            try
            {
                AsyncCallback recieveData = new AsyncCallback(OnRecievedData);
                sock.BeginReceive(m_byBuff, 0, m_byBuff.Length, SocketFlags.None, recieveData, sock);
            }
            catch (Exception)
            {
            }
        }

        public void disconnect()
        {
            if (this.isConnected)
            {
                this.m_sock.Shutdown(SocketShutdown.Both);
                this.m_sock.Close();
                if (this.Disconnect != null)
                {
                    this.Disconnect(this, new EventArgs());
                }
            }
        }

        public void OnRecievedData(IAsyncResult ar)
        {
            Socket sock = (Socket)ar.AsyncState;
            try
            {
                int nBytesRec = sock.EndReceive(ar);
                if (nBytesRec > 0)
                {
                    // Wrote the data to the List
                    sRecieved = Encoding.ASCII.GetString(m_byBuff, 0, nBytesRec);
                    if (RecieveDataEvent != null)
                    {
                        RecieveDataEvent(this, sRecieved);
                    }
                    SetupRecieveCallback(sock);
                }
                else
                {
                    // If no data was recieved then the connection is probably dead
                    Console.WriteLine("Client {0}, disconnected", sock.RemoteEndPoint);
                    sock.Shutdown(SocketShutdown.Both);
                    sock.Close();
                    if (this.Disconnect != null)
                    {
                        this.Disconnect(this, new EventArgs());
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.GetBaseException().ToString());
                SetupRecieveCallback(sock);
            }

        }
        ~alta_client()
        {
            if (m_sock != null && m_sock.Connected)
            {
                m_sock.Shutdown(SocketShutdown.Both);
                m_sock.Close();
                if (this.Disconnect != null)
                {
                    this.Disconnect(this, new EventArgs());
                }
            }
        }
    }
}
