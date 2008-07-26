#define START_OUTGAUGE_INTERFACE

#undef START_OUTGAUGE_INTERFACE     // Comment this line, to Load File

#if (START_OUTGAUGE_INTERFACE)
namespace Drive_LFSS.OutGauge
{
    using Drive_LFSS.Packet_;
    using Drive_LFSS.InSimShared_;
    //using System;
    //using System.Runtime.CompilerServices;

    public sealed class OutGaugeInterface : Socket
    {
        private string _name = "Default OutGaugeInterface";

        public event OutGauge_EventHandler OutGauge_Received;

        public OutGaugeInterface(ushort Port)
        {
            base._port = Port;
        }

        protected override void DataReceived(byte[] data)
        {
            if (data.Length == 0x60)
            {
                OutGaugePack destStruct = new OutGaugePack();
                destStruct = (OutGaugePack) PacketUtil.DataToPacket(data, destStruct);
                if (this.OutGauge_Received != null)
                {
                    this.OutGauge_Received(destStruct);
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

        public delegate void OutGauge_EventHandler(OutGaugePack og);
    }
}
#endif