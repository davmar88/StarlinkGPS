using NLog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace StarlinkTCPServerV2.Processor
{

    public class StarlinkLocation
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public string SourceUnitId { get; set; }
        public int EventReport { get; set; }
        public int UnitReferenceNumber { get; set; }
        public DateTime EventTimestamp { get; set; }
        public int Tracking { get; set; }
        public DateTime LocationTimeStamp { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Speed { get; set; }
        public int Heading { get; set; }
        public double Odometer { get; set; }
        public int CellularAntennaArea { get; set; }
        public int CellularAntennaId { get; set; }
        public double MainPowerVoltage { get; set; }
        public double BackupBatteryVoltage { get; set; }

        public StarlinkLocation()
        {

        }

        public static void SendToCmore(StarlinkLocation loc)
        {
            TrackProcessor.ProcessTrackMessage(loc);
        }

        public static StarlinkLocation parseLocationString(string DeviceString)
        {
            
            try
            {
                StarlinkLocation Location = new StarlinkLocation();
                //$SLU0004D2,06,24,190725114917,18,190725114917,+3159.4880,+03445.3498,002.3,180,12345,10052,8738,12.200,03.600,0*DC
                //crashing on
                //$SLU353612085936768,06,3,191212110504,01,,,,,,000000.000,00713,51022,0,11.992,02.965,,,,,0,21.0,61,,86,0,2*6C
                //Parameter name: length
                //at System.String.Substring(Int32 startIndex, Int32 length)
                //at StarlinkTCPServerV2.Processor.StarlinkLocation.parseLocationString(String DeviceString)
                string[] DeviceParams = DeviceString.Split(',');
                
                Location.SourceUnitId = DeviceParams[0].Replace("$SLU", "");
                Location.EventReport = Convert.ToInt32(DeviceParams[1]);
                Location.UnitReferenceNumber = Convert.ToInt32(DeviceParams[2]);
                string eventTimeString = "20" + DeviceParams[3];
                // Console.WriteLine(eventTimeString);
                Location.EventTimestamp = new DateTime(Convert.ToInt32(eventTimeString.Substring(0, 4)), Convert.ToInt32(eventTimeString.Substring(4, 2)), Convert.ToInt32(eventTimeString.Substring(6, 2)), Convert.ToInt32(eventTimeString.Substring(8, 2)), Convert.ToInt32(eventTimeString.Substring(10, 2)), Convert.ToInt32(eventTimeString.Substring(12, 2)));
                Location.Tracking = Convert.ToInt32(DeviceParams[4]);
                string locationTimeString = "20" + DeviceParams[5];
                //  Console.WriteLine(locationTimeString);
                /* string one = locationTimeString.Substring(0, 4);
                 Console.WriteLine(one);
                 string two = locationTimeString.Substring(4, 2);
                 Console.WriteLine(two);
                 string three = locationTimeString.Substring(6, 2);
                 Console.WriteLine(three);
                 string four = locationTimeString.Substring(8, 2);
                 Console.WriteLine(four);
                 string five = locationTimeString.Substring(10, 2);
                 Console.WriteLine(five);
                 string six  = locationTimeString.Substring(12, 2);
                 Console.WriteLine(six);*/
                Location.LocationTimeStamp = new DateTime(Convert.ToInt32(locationTimeString.Substring(0, 4)), Convert.ToInt32(locationTimeString.Substring(4, 2)), Convert.ToInt32(locationTimeString.Substring(6, 2)), Convert.ToInt32(locationTimeString.Substring(8, 2)), Convert.ToInt32(locationTimeString.Substring(10, 2)), Convert.ToInt32(locationTimeString.Substring(12, 2)));


                string LatDegrees = DeviceParams[6].Substring(0, 3);
                string LatMinutes = DeviceParams[6].Substring(3);
                string LatString = LatDegrees + Convert.ToString(Convert.ToDouble(LatMinutes, CultureInfo.InvariantCulture) / 60).Replace("0.", ".").Replace("0,", ".");//d = M.m / 60 Decimal Degrees = Degrees + .d
                double decimalLatitudeDegrees = Convert.ToDouble(LatString, CultureInfo.InvariantCulture);
                Location.Latitude = decimalLatitudeDegrees;

                string LonDegrees = DeviceParams[7].Substring(0, 4);
                string LonMinutes = DeviceParams[7].Substring(4);
                string LonString = LonDegrees + Convert.ToString(Convert.ToDouble(LonMinutes, CultureInfo.InvariantCulture) / 60).Replace("0.", ".").Replace("0,", ".");  //d = M.m / 60 Decimal Degrees = Degrees + .d // d = M.m / 60 Decimal Degrees = Degrees + .d
                double decimalLongitudeDegrees = Convert.ToDouble(LonString, CultureInfo.InvariantCulture);

                Location.Longitude = decimalLongitudeDegrees;
                Location.Speed = Convert.ToDouble(DeviceParams[8], CultureInfo.InvariantCulture);
                Location.Heading = Convert.ToInt32(DeviceParams[9]);
                Location.Odometer = Convert.ToDouble(DeviceParams[10], CultureInfo.InvariantCulture);
                Location.CellularAntennaArea = 0;
                if (DeviceParams[11] != "")
                    Location.CellularAntennaArea = Convert.ToInt32(DeviceParams[11]);

                Location.CellularAntennaId = 0;
                if (DeviceParams[12] != "")
                    Location.CellularAntennaId = Convert.ToInt32(DeviceParams[12]);

                Location.MainPowerVoltage = Convert.ToDouble(DeviceParams[13], CultureInfo.InvariantCulture);
                Location.BackupBatteryVoltage = Convert.ToDouble(DeviceParams[14], CultureInfo.InvariantCulture);
                
            }
            catch (Exception e)
            {
                Console.WriteLine("Some Crash Occurred within Starlink Processor class: ");
                Console.WriteLine("-----------------------------------------------------");
                Console.WriteLine(e.Message);
            }
            finally
            {
                
            }
            return Location;
        }
    }
}
