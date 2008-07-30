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
        public bool CarFinishRace(Driver allo)
        {
            Program.log.error("CarFinishRace Race Fropm Script\r\n");
            return false;
        }
    }
    public sealed class ScriptLicence : iScriptLicence
    {

    }
}
