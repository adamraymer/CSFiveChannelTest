using canlibCLSNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CANTest
{
    class Program
    {
        private UInt16 seg_1 = 0;
        private UInt16 seg_2 = 0;
        private UInt16 SJW = 0;
        private int baudRate = Canlib.canBITRATE_500K;
        private int canFlags = Canlib.canOPEN_REQUIRE_INIT_ACCESS | Canlib.canOPEN_OVERRIDE_EXCLUSIVE | Canlib.canOPEN_REQUIRE_EXTENDED;
        int ChannelHandle;
        int channelID;
        byte[] data = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };

        public void BusOn()
        {
            Canlib.canStatus status;

            this.ChannelHandle = (int)Canlib.canOpenChannel((int)this.channelID, canFlags);
            if (!((int)this.ChannelHandle).Equals(((int)Canlib.canStatus.canERR_INVHANDLE)))
            {
                status = Canlib.canSetBusParams(this.ChannelHandle, baudRate, (int)seg_1, (int)seg_2, (int)SJW, 0, 0);
                status = Canlib.canBusOn(this.ChannelHandle);
                Console.WriteLine(status.ToString() + "Bus on : Channel_" + this.channelID);

            }
        }
        static void Main(string[] args)
        {
            int maxChannelCount = 5;
            Canlib.canStatus status;

            Canlib.canInitializeLibrary();

            List<Program> CAN = new List<Program>();
            for (int i = 0; i < maxChannelCount; i++)
            {
                Program p1 = new Program();
                p1.channelID = i + maxChannelCount;
                CAN.Add(p1);
            }


            for (int i = 0; i < maxChannelCount; i++)
            {
                CAN[i].BusOn();
            }

            int samplesent = 0;
            while (true)
            {

               foreach (var x in CAN)
                {
                    status = Canlib.canWrite(x.ChannelHandle, x.channelID, x.data, 8, Canlib.canMSG_STD);
                    if (status != Canlib.canStatus.canOK)
                    {
                        Console.WriteLine(status);
                    }
                }
                samplesent++;
                if (samplesent % 5 == 0)
                    Thread.Sleep(1);
            }

            Console.ReadKey();
        }
    }
}
