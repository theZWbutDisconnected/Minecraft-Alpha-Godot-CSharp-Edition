public interface IConnectionListener
{
    void HandleException(System.Exception exception);
    void Command(byte cmd, int remaining, byte[] buffer);
}