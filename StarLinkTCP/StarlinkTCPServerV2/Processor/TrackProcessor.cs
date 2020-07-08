using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using CMoreClient;
using CMoreClient.DTO;
using NLog;

namespace StarlinkTCPServerV2.Processor
{

    public class TrackProcessor
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        public TrackProcessor()
        {

        }
        public async static void ProcessTrackMessage(StarlinkLocation loc)
        {
            try
            {
                Console.WriteLine("Processing Track Messages");
                CMoreClient.DTO.TrackRequest tr = new CMoreClient.DTO.TrackRequest();
                tr.accuracy = 0;
                tr.affiliation = "FRIENDLY";
                Classification vehicleClassification = new Classification();
                vehicleClassification.battleDimension = "LAND";
                vehicleClassification.force = "NONMILITARY";
                vehicleClassification.role = "PUBLIC";
                vehicleClassification.type = "CAR";
                tr.classification = vehicleClassification;
                tr.lla =  new string[3]{ Convert.ToString(loc.Latitude, CultureInfo.InvariantCulture), Convert.ToString(loc.Longitude, CultureInfo.InvariantCulture), "0" };
                tr.speed = loc.Speed;
                //"2019-09-27T15:00:00+02:00"
                //tr.timestamp = loc.LocationTimeStamp.ToString("yyyy-MM-dd'T'HH:mm:ss")+"+02:00";//DateTimeOffset.FromUnixTimeMilliseconds(loc.LocationTimeStamp).ToString("yyyy-MM-dd'T'HH:mm:ss") + "+02:00";
                tr.timestamp = loc.LocationTimeStamp.ToString("yyyy-MM-dd'T'HH:mm:ss") + "+02:00";//DateTimeOffset.FromUnixTimeMilliseconds(loc.LocationTimeStamp).ToString("yyyy-MM-dd'T'HH:mm:ss") + "+02:00";
                tr.trackNo = loc.SourceUnitId;
                tr.trackSource = "ComixVisionDevice";
                tr.trackType = 2;
                tr.trackSourceType = "vehicle-tracking-unit";
                List<TrackRequest> TrackList = new List<TrackRequest>();
                TrackList.Add(tr);
                string Result = await CMoreClient.CMoreClient.SendTrack(TrackList); ;
           
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in TrackProcessor: "+ex.ToString());
            }
            finally
            {

            }

        }
    }
}
