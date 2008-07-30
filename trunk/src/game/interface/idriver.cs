namespace Drive_LFSS.Script_
{
    public interface IDriver
    {
        bool IsBot();
        bool prAdminFlag { get; }
        string prDriverName { get; }
    }
}
