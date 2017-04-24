using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;

namespace rtaNetworking.Streaming.nafis {
    public class ServerNetworkHelper {

        public static string getServerIpAndPortURL( ImageStreamingServer server ) {
            string hostName = Dns.GetHostName(); // Retrive the Name of HOST

            int lastIpAddressIndex = Dns.GetHostByName( hostName ).AddressList.Length - 1; //always get the last ip for streaming

            //System.Diagnostics.Debug.WriteLine("Checking last ip index : " + lastIpAddressIndex);

            string myIP = Dns.GetHostByName( hostName ).AddressList[lastIpAddressIndex].ToString();

            string myPORT = server.getPortNumber().ToString();

            string ipAndPortURL = "http://" + myIP + ":" + myPORT;  //made like an url 
            //since the client player only accepts this format

            return ipAndPortURL;
        }


        /// <summary>
        /// to avoid used port collision
        /// </summary>
        /// <param name="port"></param>
        /// <returns></returns>
        public static int getAvailablePort( int port ) {
            Boolean isAvailable = false;  //just for debugging

            IPGlobalProperties ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();


            IPEndPoint[] objEndPoints = ipGlobalProperties.GetActiveTcpListeners();

            foreach ( IPEndPoint endPoint in objEndPoints ) {
                System.Diagnostics.Debug.WriteLine( "Checking bug : " + endPoint.Port );
                if ( endPoint.Port == port ) {
                    isAvailable = false;
                    port += 2;
                } else {
                    isAvailable = true;

                }
            }

            System.Diagnostics.Debug.WriteLine( "Checking port avaialability : " + isAvailable + " " + port );
            return port;
        }

    }
}
