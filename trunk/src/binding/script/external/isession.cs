namespace Drive_LFSS.Script_
{
    public interface ISession
    {
        long GetLatency();
        long GetReactionTime();
        int GetNbrOfDrivers();
        uint GetRaceGuid();
        string GetRaceTrackPrefix();
        string GetSessionNameForLog();
    }
}
