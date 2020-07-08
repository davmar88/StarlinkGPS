using System;
using System.Collections.Generic;
using System.Text;

namespace StarlinkTCPServer.Processor
{
    public class StarlinkLocation
    {

        public string SourceUnitId { get; set; }
        public int EventReport { get; set; }
        public int UnitReferenceNumber { get; set; }
        public long EventTimestamp { get; set; }
        public int Tracking { get; set; }
        public long LocationTimeStamp { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Speed { get; set; }
        public int Heading { get; set; }
        public int Odometer { get; set; }
        public int CellularAntennaArea { get; set; }
        public int CellularAntennaId { get; set; }
        public double MainPowerVoltage { get; set; }
        public double BackupBatteryVoltage { get; set; }

        public StarlinkLocation()
        {

        }

        public static StarlinkLocation parseLocationString(string DeviceString)
        {
            //$SLU0004D2,06,24,190725114917,18,190725114917,+3159.4880,+03445.3498,002.3,180,12345,10052,8738,12.200,03.600,0*DC
            string[] DeviceParams = DeviceString.Split(",");
            StarlinkLocation Location = new StarlinkLocation();
            Location.SourceUnitId = DeviceParams[0].Replace("$SLU", "");
            Location.EventReport = Convert.ToInt32(DeviceParams[1]);
            Location.UnitReferenceNumber = Convert.ToInt32(DeviceParams[2]);
            Location.EventTimestamp = Convert.ToInt64(DeviceParams[3]);
            Location.Tracking = Convert.ToInt32(DeviceParams[4]);
            Location.LocationTimeStamp = Convert.ToInt64(DeviceParams[5]);
            Location.Latitude = Convert.ToDouble(DeviceParams[6]);
            Location.Longitude = Convert.ToDouble(DeviceParams[7]);
            Location.Speed = Convert.ToDouble(DeviceParams[8]);
            Location.Heading = Convert.ToInt32(DeviceParams[9]);
            Location.Odometer = Convert.ToInt32(DeviceParams[10]);
            Location.CellularAntennaArea = Convert.ToInt32(DeviceParams[11]);
            Location.CellularAntennaId = Convert.ToInt32(DeviceParams[12]);
            Location.MainPowerVoltage = Convert.ToDouble(DeviceParams[13]);
            Location.BackupBatteryVoltage = Convert.ToDouble(DeviceParams[14]);
            return Location;
        }
    }
}
