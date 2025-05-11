
using System;

public interface IServerListener
{
    void ClientConnected(SocketConnection connection);
    void ClientException(SocketConnection connection, Exception exception);
}