﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace rtaNetworking.Streaming.nafis {
    public static class SocketExtensions {

        public static IEnumerable<Socket> IncommingConnections( this Socket server ) {
            while ( true )
                yield return server.Accept();
        }

    }

}
