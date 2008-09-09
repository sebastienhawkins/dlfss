namespace Drive_LFSS.Script_
{
    public interface ICar
    {
        byte CarId { get; }
        bool IsOnTrack();
        string CarName { get; }
    }
}
