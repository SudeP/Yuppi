using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using Yuppi.ConnectionProvider.Interface;
using Yuppi.Environment;

namespace Yuppi.ConnectionProvider.Inheritance
{
    public class SocketConnectionProvider : IConnectionProvider<Socket>
    {
        public SocketConnectionProvider(Socket socket, int socketBandwidth = Default.Bandwidth)
        {
            if (socket is null)
                throw new ArgumentNullException(nameof(socket));

            this.socket = socket;

            if (socketBandwidth == 0 || socketBandwidth > socket.ReceiveBufferSize)
                throw new Exception("socketBandwidth have to be min 1 and max " + socket.ReceiveBufferSize);

            bandwidth = socketBandwidth;
        }

        private readonly Socket socket;
        private readonly int bandwidth;

        public Socket Socket => socket;
        public int Bandwidth => bandwidth;

        public void Write(byte[] buffer)
        {
            int offset = 0;
            int remaining = buffer.Length;

            while (remaining > 0)
            {
                int size = remaining < bandwidth ? remaining : bandwidth;

                remaining -= remaining < bandwidth ? remaining : bandwidth;

                socket.Send(buffer, offset, size, SocketFlags.None, out SocketError socketError);

                if (socketError != SocketError.Success)
                {
                    throw new SocketException((int)socketError);
                }

                offset += bandwidth;
            }
        }

        public byte[] Read()
        {
            IEnumerable<byte> tempory = new byte[0];

            int size = 0;

            do
            {
                byte[] buffer = new byte[bandwidth];

                int receieved = socket.Receive(buffer, 0, bandwidth, SocketFlags.None, out SocketError socketError);

                if (socketError != SocketError.Success)
                {
                    throw new SocketException((int)socketError);
                }

                size += receieved;

                tempory = tempory.Concat(buffer.Take(receieved));

            } while (socket.Available > 0);

            return tempory.ToArray();
        }

        public Socket Accept()
        {
            Socket acceptSocket = socket.Accept();

            return acceptSocket;
        }

        public void Disconnect()
        {
            socket.Shutdown(SocketShutdown.Both);
            socket.Disconnect(false);
            socket.Close(3);
            socket.Dispose();
        }
    }
}
