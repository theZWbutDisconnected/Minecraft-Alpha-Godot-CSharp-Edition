
using System;
using System.Net.Sockets;
using System.IO;

public class SocketConnection : IDisposable
{
    public const int BUFFER_SIZE = 131068;
    private bool connected;
    private Socket socket;
    public byte[] readBuffer = new byte[BUFFER_SIZE];
    public byte[] writeBuffer = new byte[BUFFER_SIZE];
    protected long lastRead;
    private IConnectionListener connectionListener;
    private int readPosition;
    private int writePosition;
    private int totalBytesWritten;
    private int maxBlocksPerIteration = 3;
    private NetworkStream stream;
    private int bytesRead;

    public SocketConnection(string ip, int port)
    {
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        socket.Connect(ip, port);
        socket.Blocking = false;
        lastRead = DateTime.Now.Ticks;
        connected = true;
        stream = new NetworkStream(socket);
        Array.Clear(readBuffer, 0, BUFFER_SIZE);
        Array.Clear(writeBuffer, 0, BUFFER_SIZE);
    }

    public SocketConnection(Socket acceptedSocket)
    {
        socket = acceptedSocket;
        socket.Blocking = false;
        lastRead = DateTime.Now.Ticks;
        connected = true;
        stream = new NetworkStream(socket);
        Array.Clear(readBuffer, 0, BUFFER_SIZE);
        Array.Clear(writeBuffer, 0, BUFFER_SIZE);
    }

    public void SetMaxBlocksPerIteration(int max) => maxBlocksPerIteration = max;

    public string GetIP() => socket.RemoteEndPoint.ToString();

    public void SetConnectionListener(IConnectionListener listener) => connectionListener = listener;

    public bool IsConnected() => connected;

    public void Dispose()
    {
        connected = false;
        try { stream?.Close(); } catch { }
        try { socket?.Shutdown(SocketShutdown.Both); } catch { }
        try { socket?.Close(); } catch { }
        stream = null;
        socket = null;
    }

    public void Tick()
    {
        if (socket.Available > 0)
        {
            bytesRead = stream.Read(readBuffer, readPosition, Math.Min(socket.Available, BUFFER_SIZE - readPosition));
            readPosition += bytesRead;
        }

        if (writePosition > 0)
        {
            totalBytesWritten += writeBuffer.Length - writePosition;
            stream.Write(writeBuffer, 0, writePosition);
            writePosition = 0;
        }

        if (readPosition > 0)
        {
            connectionListener?.Command(readBuffer[0], readPosition, readBuffer);
            Array.Copy(readBuffer, 1, readBuffer, 0, readPosition - 1);
            readPosition--;
        }
    }

    public int GetSentBytes() => totalBytesWritten;
    public int GetReadBytes() => bytesRead;
    public void ClearSentBytes() => totalBytesWritten = 0;
    public void ClearReadBytes() => bytesRead = 0;
}