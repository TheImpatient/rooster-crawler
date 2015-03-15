using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace RoosterCrawler
{
    public class Heartbeat
    {
        private System.Timers.Timer heartbeatTimer = new System.Timers.Timer();
        private IPEndPoint ipep;

        public Heartbeat()
        {
            heartbeatTimer.Interval = 1000;
            heartbeatTimer.AutoReset = true;
            heartbeatTimer.Elapsed += new System.Timers.ElapsedEventHandler(heartbeatTimer_Elapsed);
            heartbeatTimer.Start();

            // Initialize IPEndPoint for loopback network adapter on Port 10000.
            ipep = new IPEndPoint(IPAddress.Loopback, 10000);           
        }

        /// <summary>
        /// Handler for timer event. Sends the Heartbeat if the Checkbox is checked.
        /// </summary>
        /// <param name="sender">Sender that occured the event; not used.</param>
        /// <param name="e">Arguments for the Elapsed Event; not used.</param>
        private void heartbeatTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            SendUdpPacket();   
        }

        /// <summary>
        /// Sends the UDP Heartbeat Packet.
        /// </summary>
        private void SendUdpPacket()
        {
            byte[] data = new byte[1024];

            // Create UDP Socket
            Socket udpClientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            // Send Application Title (Window Title) as the Data
            data = Encoding.ASCII.GetBytes("crawler");

            // Send it ...
            udpClientSocket.SendTo(data, 0, data.Length, SocketFlags.None, ipep);
        }
    }
}
