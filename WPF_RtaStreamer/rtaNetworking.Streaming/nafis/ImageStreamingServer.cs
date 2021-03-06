﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace rtaNetworking.Streaming.nafis {

    /// <summary>
    /// Provides a streaming server that can be used to stream any images source
    /// to any client.
    /// </summary>
    public class ImageStreamingServer : IDisposable {

        private List<Socket> _Clients;
        private Thread _Thread;
        private int myPort;
        public int Port {
            get { return myPort; }
            set { myPort = value; }
        }

        private int _maxClient = 16;

        public int MaxClients {
            get { return _maxClient; }
            set { _maxClient = value; }
        }
        



        /// <summary>
        /// Gets or sets the source of images that will be streamed to the 
        /// any connected client.
        /// </summary>
        public IEnumerable<Image> ImagesSource { get; set; }

        /// <summary>
        /// Gets or sets the interval in milliseconds (or the delay time) between 
        /// the each image and the other of the stream (the default is . 
        /// </summary>
        public int Interval { get; set; }

        /// <summary>
        /// Gets a collection of client sockets.
        /// </summary>
        public IEnumerable<Socket> Clients { get { return _Clients; } }

        /// <summary>
        /// Returns the status of the server. True means the server is currently 
        /// running and ready to serve any client requests.
        /// </summary>
        public bool IsRunning { get { return ( _Thread != null && _Thread.IsAlive ); } }

        public ImageStreamingServer()
            : this( Screen.Snapshots( 480, true ) ) {

        }

        /// <summary>
        /// ///////my function to get the port number
        /// </summary>
        /// <param name="imagesSource"></param>




        public ImageStreamingServer( IEnumerable<Image> imagesSource ) {

            _Clients = new List<Socket>();
            _Thread = null;

            this.ImagesSource = imagesSource;
            this.Interval = 67;                           //frame rate = 1000ms / Interval

        }
        public void StartWithRandomPort() {

            int safePort = ServerNetworkHelper.getAvailablePort( 8080 );                 //8080 is just the first try
            Start( safePort );

        }
        /// <summary>
        /// Starts the server to accepts any new connections on the specified port.
        /// </summary>
        /// <param name="port"></param>
        
        public void Start( int port ) {

            lock ( this ) {

                this.Port = port;
                _Thread = new Thread( new ThreadStart( ServerThread ) );
                _Thread.IsBackground = true;
                _Thread.Start();
            }

        }

        /// <summary>
        /// Starts the server to accepts any new connections on the default port (8080).
        /// </summary>
        /// this was 0 referenced
        /*  public void Start()
          {
              this.Start(8080);
          }*/

        public void Stop() {

            if ( this.IsRunning ) {
                try {
                    _Thread.Join();
                    _Thread.Abort();
                } finally {

                    lock ( _Clients ) {

                        foreach ( var s in _Clients ) {
                            try {
                                s.Close();
                            } catch { }
                        }
                        _Clients.Clear();

                    }

                    _Thread = null;
                }
            }
        }

        /// <summary>
        /// This the main thread of the server that serves all the new 
        /// connections from clients.
        /// </summary>
        /// <param name="state"></param>
        private void ServerThread() {

            try {

                Socket Server = new Socket( AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp );

                Server.Bind( new IPEndPoint( IPAddress.Any, Port ) );
                Server.Listen( MaxClients );    

                System.Diagnostics.Debug.WriteLine( string.Format( "Server started on port {0}.", Port ) );

                System.Diagnostics.Debug.WriteLine( "Checking my port : " + Port.ToString() );


                foreach ( Socket client in Server.IncommingConnections() )
                    ThreadPool.QueueUserWorkItem( new WaitCallback( ClientThread ), client );


            } catch {

                System.Diagnostics.Debug.WriteLine( "Exception Caught for multiple entry" );
            }

            this.Stop();
        }

        /// <summary>
        /// Each client connection will be served by this thread.
        /// </summary>
        /// <param name="client"></param>
        private void ClientThread( object client ) {

            Socket socket = ( Socket ) client;

            System.Diagnostics.Debug.WriteLine( string.Format( "New client from {0}", socket.RemoteEndPoint.ToString() ) );

            lock ( _Clients )
                _Clients.Add( socket );

            try {
                using ( MjpegWriter wr = new MjpegWriter( new NetworkStream( socket, true ) ) ) {

                    // Writes the response header to the client.
                    wr.WriteHeader();

                    // Streams the images from the source to the client.
                    foreach ( var imgStream in Screen.Streams( this.ImagesSource ) ) {
                        if ( this.Interval > 0 )
                            Thread.Sleep( this.Interval );

                        wr.Write( imgStream );
                    }

                }
            } catch { } finally {
                lock ( _Clients )
                    _Clients.Remove( socket );
            }
        }


        #region IDisposable Members

        public void Dispose() {
            this.Stop();
        }

        #endregion

        public string getServerURL() {

            return ServerNetworkHelper.getServerURL( this );

        }

        

    }

}
