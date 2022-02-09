namespace Yuppi.ConnectionProvider.Interface
{
    public interface IConnectionProvider<ConnectionType> where ConnectionType : class
    {
        ConnectionType Socket { get; }
        int Bandwidth { get; }
        public void Write(byte[] buffer);
        public byte[] Read();
        public ConnectionType Accept();
        public void Disconnect();
    }
}