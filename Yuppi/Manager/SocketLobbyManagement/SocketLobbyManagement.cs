using System;
using System.Net;
using Yuppi.Environment;
using Yuppi.Networking.Inheritance;
using Yuppi.Struct;

namespace Yuppi.Manager.SocketLobbyManagement
{
    public class SocketLobbyManagement : IDisposable
    {
        public SocketLobbyManagement(IPAddress ipAddress, int port)
        {
            server = new SocketServer(new IPEndPoint(ipAddress, port));

            server.OnReceive += Server_OnReceive;

            lobbies = new KeyValueSync<string, SocketLobby>();
        }

        public readonly SocketServer server;

        public readonly KeyValueSync<string, SocketLobby> lobbies;

        private void Server_OnReceive(P2PMessage request)
        {
            SocketClient client = server.clients.Get(request.source);

            if (request.destination == ((uint)SpecialId.Create))
            {
                P2PMessage responseMessage = CreateANewLobby(request, client);

                client.pipeline.SendAnyData(responseMessage);

                return;
            }
            else if (request.destination == ((uint)SpecialId.Join))
            {
                P2PMessage responseMessage = JoinLobbyByAuthKey(request, client);

                client.pipeline.SendAnyData(responseMessage);

                return;
            }
            else if (request.destination == ((uint)SpecialId.WithMe))
            {
                SendDataAllClients(request, false);

                return;
            }
            else if (request.destination == ((uint)SpecialId.WithoutMe))
            {
                SendDataAllClients(request, true);

                return;
            }
            else if (request.destination > ((uint)SpecialId.Server))
            {
                SendDataClientToClient(request);

                return;
            }
        }

        private P2PMessage CreateANewLobby(P2PMessage request, SocketClient client)
        {
            P2PMessage response = new P2PMessage((uint)SpecialId.Server, (uint)SpecialId.Error, null);

            //data have not be null
            if (request.data is null)
            {
                response.data = "data have not be null";
                return response;
            }

            LobbySettings settings = (LobbySettings)request.data;

            //key have not be null
            if (string.IsNullOrEmpty(settings.key) || string.IsNullOrWhiteSpace(settings.key))
            {
                response.data = "key have not be null";
                return response;
            }

            //capacity have not be less or equal to zero
            if (settings.capacity <= 0)
            {
                response.data = string.Format("capacity have not be less or equal to zero. Capacity = {0}", settings.capacity);
                return response;
            }

            //lobby have not be create same key
            if (lobbies.Has(settings.key))
            {
                response.data = string.Format("lobby have not be create same key. Key = {0}", settings.key);
                return response;
            }

            var newLobby = new SocketLobby(settings);

            //try add new client
            newLobby.clients.Add(request.source, client);

            //try add new lobby
            lobbies.TryAdd(settings.key, newLobby);

            response.destination = (uint)SpecialId.Success;
            return response;
        }

        private P2PMessage JoinLobbyByAuthKey(P2PMessage request, SocketClient client)
        {
            P2PMessage response = new P2PMessage((uint)SpecialId.Server, (uint)SpecialId.Error, null);

            //data have not be null
            if (request.data is null)
            {
                response.data = "data have not be null";
                return response;
            }

            string authorizationKey = request.data.ToString();

            //key have not be null
            if (string.IsNullOrEmpty(authorizationKey) || string.IsNullOrWhiteSpace(authorizationKey))
            {
                response.data = "key have not be null";
                return response;
            }

            //lobby can find by key
            if (!lobbies.Has(authorizationKey))
            {
                response.data = string.Format("lobby can not find by key. Key = {0}", authorizationKey);
                return response;
            }

            SocketLobby lobby = lobbies.Get(authorizationKey);

            //player have not add to lobby cause lobby full
            if (lobby.settings.capacity <= lobby.clients.Count)
            {
                response.data = string.Format("player have not add to lobby cause lobby full. Capacity = {0}", lobby.settings.capacity);
                return response;
            }

            lobby.clients.Add(request.source, client);

            response.destination = (uint)SpecialId.Success;
            return response;
        }

        private void SendDataAllClients(P2PMessage request, bool withoutMe)
        {
            SocketLobby lobby = lobbies.First((key, value) => value.clients.Has(request.source));

            if (lobby is null)
                return;

            foreach (var client in lobby.clients)
            {
                if (withoutMe && request.source == client.Key)
                    continue;

                client.Value.pipeline.SendAnyData(request);
            }
        }

        private void SendDataClientToClient(P2PMessage request)
        {
            SocketLobby lobby = lobbies.First((key, value) => value.clients.Has(request.source));

            if (lobby is null)
                return;

            SocketClient targetSocketClient = lobby.clients.Get(request.destination);

            if (targetSocketClient is null)
                return;

            targetSocketClient.pipeline.SendAnyData(request);
        }

        public void Dispose()
        {
            server.Dispose();
        }
    }
}
