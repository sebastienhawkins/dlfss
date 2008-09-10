namespace Drive_LFSS.Script_
{
    public interface IButton
    {
        void AddMessageTop(string text, uint duration);
        void AddMessageMiddle(string text, uint duration);
        void AddTimedButton(ushort buttonEntry, uint time);
        void SendButton(ushort buttonEntry);
        void SendButton(byte buttonId, ushort buttonEntry, byte styleMask,  bool isAllwaysVisible, byte maxTextLength, byte left, byte top, byte width, byte height, string text);
        byte RemoveButton(ushort buttonEntry);
        void SendGui(ushort guiEntry);
        void RemoveGui(ushort guiEntry);
    }
}
