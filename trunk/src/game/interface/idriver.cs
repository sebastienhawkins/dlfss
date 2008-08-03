namespace Drive_LFSS.Script_
{
    public interface IDriver
    {
        bool IsBot();
        bool AdminFlag { get; }
        string DriverName { get; }
    }
}
