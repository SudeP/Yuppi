using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Yuppi.ConnectionProvider.Inheritance;
using Yuppi.ConnectionProvider.Interface;
using Yuppi.Delegates;
using Yuppi.Environment;
using Yuppi.Pipeline.Interface;
using Yuppi.Serializer.Inheritance;
using Yuppi.Serializer.Interface;
using Yuppi.Struct;

namespace Yuppi.Pipeline.Inheritance
{
    public class SocketPipeLine : IPipeline<P2PMessage, Socket>
    {
        public SocketPipeLine(ISocketSerializer<P2PMessage> socketSerializer, IConnectionProvider<Socket> byteProvider)
        {
            serializer = socketSerializer;
            provider = byteProvider;
        }

        private readonly ISocketSerializer<P2PMessage> serializer;
        private readonly IConnectionProvider<Socket> provider;

        public ISocketSerializer<P2PMessage> Serializer => serializer;
        public IConnectionProvider<Socket> Provider => provider;

        public void SendAnyData(P2PMessage message)
        {
            byte[] byteArray = serializer.Serialize(message);

            provider.Write(byteArray);
        }

        public P2PMessage WaitUntilComeAnyData()
        {
            byte[] byteArray = provider.Read();

            P2PMessage message = serializer.Deserialize(byteArray);

            return message;
        }

        public Socket AcceptNewConnection()
        {
            return provider.Accept();
        }

        public void AcceptNewConnection(AcceptNewConnection<Socket> acceptSocket)
        {
            acceptSocket.Invoke(AcceptNewConnection());
        }

        public Task AcceptNewConnection(AcceptNewConnection<Socket> acceptSocket, CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(@delegate =>
            {
                AcceptNewConnection(((AcceptNewConnection<Socket>)@delegate));
            }, acceptSocket, cancellationToken);
        }

        public void Disconnect()
        {
            provider.Disconnect();
        }

        public static SocketPipeLine Instace(Socket socket = null, int bandwidth = Default.Bandwidth)
        {
            return new SocketPipeLine(
                new BinarySocketSerializer<P2PMessage>(),
                new SocketConnectionProvider(socket ?? new Socket(SocketType.Stream, ProtocolType.Tcp), bandwidth)
            );
        }
    }
}