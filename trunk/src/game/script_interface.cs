using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Drive_LFSS.Script_
{
    using Drive_LFSS.Game_;

    public interface iScriptSession
    {
        bool HasComeOnline();
    }
    public interface iScriptRace
    {
    }
    public interface iScriptDriver
    {
        
    }
    public interface iScriptCar
    {
        bool CarFinishRace(Car _car);
        bool CarAcceleration_0_100(Car _car);
    }
    public interface iScriptLicence
    {

    }
}
