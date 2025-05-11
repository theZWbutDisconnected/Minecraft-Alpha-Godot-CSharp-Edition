using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

public class SocketServer
{
    private TcpListener listener;
    private IServerListener serverListener;
    private List<SocketConnection> connections = new List<SocketConnection>();
    private byte[] bindIP;

    public SocketServer(byte[] ip, int port, IServerListener serverlistener)
    {
        serverListener = serverlistener;
        bindIP = ip;
        listener = new TcpListener(new IPAddress(bindIP), port);
        listener.Server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
        listener.Start();
        listener.Server.Blocking = false;
    }

    public void Tick()
    {
        while (listener.Pending())
        {
            try
            {
                Socket client = listener.AcceptSocket();
                client.Blocking = false;
                SocketConnection conn = new SocketConnection(client);
                connections.Add(conn);
                serverListener.ClientConnected(conn);
            }
            catch (Exception e)
            {
                // Handle exception
            }
        }

        for (int i = connections.Count - 1; i >= 0; i--)
        {
            if (!connections[i].IsConnected())
            {
                connections[i].Dispose();
                connections.RemoveAt(i);
                continue;
            }

            try
            {
                connections[i].Tick();
            }
            catch (Exception e)
            {
                serverListener.ClientException(connections[i], e);
                connections[i].Dispose();
                connections.RemoveAt(i);
            }
        }
    }
}