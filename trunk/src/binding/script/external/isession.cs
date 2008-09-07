namespace Drive_LFSS.Script_
{
    public interface ISession
    {
        long GetLatency();
        long GetReactionTime();
        byte GetNbrOfDrivers();
        byte GetNbrOfConnection();
        uint GetRaceGuid();
        string GetRaceTrackPrefix();
        string GetSessionNameForLog();
        string GetSessionName();
        void SendUpdateButtonToAll(ushort buttonEntry, string text);
        void RemoveButtonToAll(ushort buttonEntry);
        void RemoveButton(ushort buttonEntry,byte licenceId);
        void AddMessageMiddleToAll(string text, uint duration);
    }
}
