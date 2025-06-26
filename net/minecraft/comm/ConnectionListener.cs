public interface IConnectionListener
{
    void handleException(System.Exception exception);
    void command(byte cmd, int remaining, byte[] buffer);
}