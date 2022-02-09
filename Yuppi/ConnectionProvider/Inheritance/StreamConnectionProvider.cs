using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using Yuppi.ConnectionProvider.Interface;
using Yuppi.Environment;

namespace Yuppi.ConnectionProvider.cs.Inheritance
{
    public class StreamConnectionProvider : IConnectionProvider<Socket>
    {
        public StreamConnectionProvider(Socket socket, int socketBandwidth = Default.Bandwidth)
        {
            this.socket = socket;

            stream = new NetworkStream(socket);

            bandwidth = socketBandwidth;
        }

        private readonly Socket socket;
        private readonly int bandwidth;
        private readonly NetworkStream stream;

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

                stream.Write(buffer, offset, size);

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

                int receieved = stream.Read(buffer, 0, buffer.Length);

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
