using System.Net;
using System.Net.Sockets;
using Yuppi.Networking.Abstract;

namespace Yuppi.Networking.Inheritance
{
    public class SocketClient : SocketNode
    {
        internal SocketClient(uint determinedIdentity, Socket createdSocket) : base(determinedIdentity, createdSocket)
        {

        }

        public SocketClient(IPEndPoint iPEndPoint) : base(iPEndPoint)
        {

        }
    }
}