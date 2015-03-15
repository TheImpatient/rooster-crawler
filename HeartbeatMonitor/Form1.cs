using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace HeartbeatMonitor
{
    public partial class Form1 : Form
    {
        private Label errorLabel;
        delegate void SetTextCallback(string text);
        private DateTime lastUpdate;
        private Socket udpServerSocket;
        private EndPoint ep = new IPEndPoint(IPAddress.Any, 10000);
        private System.Timers.Timer checkTimer = new System.Timers.Timer();
        private byte[] buffer = new byte[1024];

        public Form1()
        {
            InitializeComponent();
            errorLabel = lblError;
            udpServerSocket = new Socket(AddressFamily.InterNetwork,
                SocketType.Dgram, ProtocolType.Udp);
            udpServerSocket.Bind(ep);
            udpServerSocket.BeginReceiveFrom(buffer, 0, 1024, SocketFlags.None, ref ep, new AsyncCallback(ReceiveData), udpServerSocket);

            checkTimer.Interval = 1000;
            checkTimer.AutoReset = true;
            checkTimer.Elapsed += new System.Timers.ElapsedEventHandler(checkTimer_Elapsed);
            checkTimer.Start();
        }

        void ReceiveData(IAsyncResult iar)
        {
            // Create temporary remote end Point
            IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
            EndPoint tempRemoteEP = (EndPoint)sender;

            // Get the Socket
            Socket remote = (Socket)iar.AsyncState;

            // Call EndReceiveFrom to get the received Data
            int recv = remote.EndReceiveFrom(iar, ref tempRemoteEP);

            // Get the Data from the buffer to a string
            string stringData = Encoding.ASCII.GetString(buffer, 0, recv);
            Console.WriteLine(stringData);

            // update Timestamp
            lastUpdate = DateTime.Now.ToUniversalTime();

            // Restart receiving
            if (!this.IsDisposed)
            {
                udpServerSocket.BeginReceiveFrom(buffer, 0, 1024, SocketFlags.None, ref ep, new AsyncCallback(ReceiveData), udpServerSocket);
            }
        }

        private void checkTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            // Calculate the Timespan since the Last Update from the Client.
            TimeSpan timeSinceLastHeartbeat = DateTime.Now.ToUniversalTime() - lastUpdate;

            // Set Lable Text depending of the Timespan
            if (timeSinceLastHeartbeat > TimeSpan.FromSeconds(3))
            {
                SetText("No Answer from Crawler");
                UpdateHeartbeat(false);

            }
            else
            {
                SetText("Crawler OK");
                UpdateHeartbeat(true);
            }
        }

        private void SetText(string text)
        {
            if (errorLabel.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetText);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                switch(text)
                {
                    case "No Answer from Crawler":
                        errorLabel.Text = "No Answer from Crawler";
                        errorLabel.BackColor = Color.Red;
                        break;

                    case "Crawler OK":
                        errorLabel.Text = "Crawler OK";
                        errorLabel.BackColor = Color.Green;
                        break;
                }
            }
        }

        private void UpdateHeartbeat(bool status)
        {
            string query = String.Format("UPDATE Heartbeat SET  running = '{0}' WHERE  id = 1 ;",status?1:0);

            const string connectionString = @Credentials.ConnectionString;
            MySqlConnection connection = null;

            try
            {
                connection = new MySqlConnection(connectionString);
                connection.Open();

                var sqlCommand = new MySqlCommand(query, connection);
                sqlCommand.Prepare();
                sqlCommand.ExecuteNonQuery();
                connection.Close();
            }
            catch (MySqlException err)
            {

            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                }
            }
        }
    }
}
