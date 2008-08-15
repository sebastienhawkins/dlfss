namespace Drive_LFSS.Script_
{
    public interface ISession
    {
        long GetLatency();
        int GetNbrOfDrivers();
        long GetReactionTime();
    }
}
