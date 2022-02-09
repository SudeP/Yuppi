using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Yuppi.Delegates;
using Yuppi.Pipeline.Inheritance;
using Yuppi.Struct;

namespace Yuppi.Networking.Abstract
{
    public abstract class SocketNode : IDisposable
    {
        public SocketNode(uint determinedIdentity, Socket createdSocket)
        {
            identity = determinedIdentity;

            ipEndPoint = (IPEndPoint)createdSocket.RemoteEndPoint;

            pipeline = SocketPipeLine.Instace(createdSocket);
        }

        public SocketNode(IPEndPoint iPEndPoint)
        {
            ipEndPoint = iPEndPoint;

            pipeline = SocketPipeLine.Instace();
        }

        public SocketNode(IPEndPoint iPEndPoint, int bandwidth)
        {
            ipEndPoint = iPEndPoint;

            pipeline = SocketPipeLine.Instace(bandwidth: bandwidth);
        }

        protected uint identity;
        protected Thread thread;

        public readonly IPEndPoint ipEndPoint;
        public readonly SocketPipeLine pipeline;

        public uint Identity => identity;

        public virtual event OnReceiveDelegate<P2PMessage> OnReceive;

        public void ListenRemoteEndPoint()
        {
            thread = new Thread(new ParameterizedThreadStart(HandleReceive));

            thread.Start(pipeline);
        }

        private void HandleReceive(object @object)
        {
            SocketPipeLine pipeline = (SocketPipeLine)@object;

            P2PMessage handledMessage = pipeline.WaitUntilComeAnyData();

            OnReceive?.Invoke(handledMessage);
        }

        public virtual void Dispose()
        {
            if (thread != null)
            {
                thread.Abort();
                thread = null;
            }

            pipeline.Disconnect();
        }
    }
}
