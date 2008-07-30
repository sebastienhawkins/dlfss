namespace Drive_LFSS.Script_
{
    using Drive_LFSS.Game_;
    using Drive_LFSS;

    public sealed class ScriptSession : iScriptSession
    {
        public bool HasComeOnline()
        {
            Program.log.error("Session Comming Online\r\n");
            return false;
        }
    }
    public sealed class ScriptRace : iScriptRace
    {

    }
    public sealed class ScriptDriver : iScriptDriver
    {

    }
    public sealed class ScriptCar : iScriptCar
    {
        public bool CarFinishRace(Car _car)
        {
            Program.log.error("ScriptCall: CarFinishRace\r\n");
            return false;
        }
        public bool CarAcceleration_0_100(Car _car)
        {
            Program.log.error("ScriptCall: CarAcceleration_0_100\r\n");
            return false; //Mean There is no Custom Script Processing, True will mean you have done a script proccesing!
        }
    }
    public sealed class ScriptLicence : iScriptLicence
    {

    }
}
