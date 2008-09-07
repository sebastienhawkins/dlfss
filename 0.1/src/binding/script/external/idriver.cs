namespace Drive_LFSS.Script_
{
    public interface IDriver
    {
        bool AdminFlag { get; }
        string DriverName { get; }
        uint GetGuid();
        bool IsBot();
        void SendMessage(string message);
    }
}
