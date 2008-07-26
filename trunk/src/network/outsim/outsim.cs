#define START_OUTSIM_INTERFACE

#undef START_OUTSIM_INTERFACE   // Comment this line, to Load File

#if START_OUTSIM_INTERFACE
namespace Drive_LFSS.OutSim
{
    //using System.Runtime.CompilerServices;
    using Drive_LFSS.Packet_;
    using Drive_LFSS.InSimShared_;

    public sealed class OutSimInterface : Socket
    {
        private string _name = "Default OutSimInterface";

        public event OutSim_EventHandler OutSim_Received;

        public OutSimInterface(ushort Port)
        {
            base._port = Port;
        }

        protected override void DataReceived(byte[] data)
        {
            if (data.Length == 0x44)
            {
                OutSimPack destStruct = new OutSimPack();
                destStruct = (OutSimPack) PacketUtil.DataToPacket(data, destStruct);
                if (this.OutSim_Received != null)
                {
                    this.OutSim_Received(destStruct);
                }
            }
        }

        public string Name
        {
            get
            {
                return this._name;
            }
            set
            {
                this._name = value;
            }
        }

        public delegate void OutSim_EventHandler(OutSimPack os);
    }
}
#endif