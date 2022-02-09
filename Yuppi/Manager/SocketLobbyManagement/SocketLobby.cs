using Yuppi.Environment;
using Yuppi.Networking.Inheritance;

namespace Yuppi.Manager.SocketLobbyManagement
{
    public class SocketLobby
    {
        public SocketLobby(LobbySettings lobbySettings)
        {
            settings = lobbySettings;

            clients = new KeyValueSync<uint, SocketClient>();
        }
        public readonly LobbySettings settings;
        public readonly KeyValueSync<uint, SocketClient> clients;
    }
}
