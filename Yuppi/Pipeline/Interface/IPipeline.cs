using System.Threading;
using System.Threading.Tasks;
using Yuppi.Delegates;

namespace Yuppi.Pipeline.Interface
{
    public interface IPipeline<MessageType, ConnectionType>
        where MessageType : struct
        where ConnectionType : class
    {
        public void SendAnyData(MessageType message);
        public MessageType WaitUntilComeAnyData();
        public ConnectionType AcceptNewConnection();
        public void AcceptNewConnection(AcceptNewConnection<ConnectionType> acceptSocket);
        public Task AcceptNewConnection(AcceptNewConnection<ConnectionType> acceptSocket, CancellationToken cancellationToken);
        public void Disconnect();
    }
}