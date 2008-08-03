namespace Drive_LFSS.Script_
{
    public sealed class ScriptSession : IScriptSession
    {
        public bool HasComeOnline()
        {
            return false;               //Mean There is no Custom Script Processing, True will mean you have done a script proccesing!
        }
    }
    public sealed class ScriptRace : IScriptRace
    {

    }
    public sealed class ScriptDriver : IScriptDriver
    {

    }
    public sealed class ScriptCar : IScriptCar
    {
        public bool CarFinishRace(ICar _car)
        {
            return false;               //Mean There is no Custom Script Processing, True will mean you have done a script proccesing!
        }
        public bool CarAcceleration_0_100(ICar _car)
        {
            return false;               //Mean There is no Custom Script Processing, True will mean you have done a script proccesing!
        }
    }
    public sealed class ScriptLicence : IScriptLicence
    {

    }
}
