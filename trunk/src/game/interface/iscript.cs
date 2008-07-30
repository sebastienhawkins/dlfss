namespace Drive_LFSS.Script_
{
    public interface IScriptSession
    {
        bool HasComeOnline();
    }
    public interface IScriptRace
    {
    }
    public interface IScriptDriver
    {
        
    }
    public interface IScriptCar
    {
        bool CarFinishRace(ICar _car);
        bool CarAcceleration_0_100(ICar _car);
    }
    public interface IScriptLicence
    {

    }
}