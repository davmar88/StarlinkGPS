using NLog;
using StarlinkTCPServerV2.Processor;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Text;

public class SynchronousSocketListener
{

    private static Logger logger = LogManager.GetCurrentClassLogger();
    // Incoming data from the client.  
    public static string data = null;

    public static void StartListening()
    {
        int ServerResponderCounter = 0;
        // Data buffer for incoming data.  
        byte[] bytes = new Byte[1024];
        IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
        IPAddress ipAddress = System.Net.IPAddress.Parse("154.0.161.164");
        IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 5000);
        Console.WriteLine("Starting Syncronous Server for a connection on 154.0.161.164:5000");
        // Create a TCP/IP socket.  
        Socket listener = new Socket(ipAddress.AddressFamily,SocketType.Stream, ProtocolType.Tcp);
      

        // Bind the socket to the local endpoint and   
        // listen for incoming connections.  
        try
        {
            listener.Bind(localEndPoint);
            listener.Listen(10);
           
            // Start listening for connections.  
            while (true)
            {
                Console.WriteLine("Waiting for a connection...");
                // Program is suspended while waiting for an incoming connection.  
                Socket handler = listener.Accept();
                data = null;
                DateTime ReadingDate = DateTime.Now;
                // An incoming connection needs to be processed.  
                while (true)
                {
                    Console.WriteLine("Got Something... : ");
                    int bytesRec = handler.Receive(bytes);
                    string chunk = Encoding.ASCII.GetString(bytes, 0, bytesRec);
                    Console.WriteLine(DateTime.Now.ToString("yyyy/MM/dd HH:mm:sss") + " found chunk:" + chunk);
                    data += chunk;
                    if (data.Contains("$") == false)
                    {
                        break;
                    }
                    if (data.IndexOf("\r\n") > -1 &&  DateTime.Now >  ReadingDate.AddMinutes(1))
                    {
                        break;
                    }
                    else
                    {
                        Console.WriteLine("No New Line: current data is:" + data);
                    }
                }
                // Show the data on the console.  
                Console.WriteLine(DateTime.Now.ToString("yyyy/MM/dd HH:mm:sss")+" Data To Process : {0}", data);
                
                if (data[0] == '$')
                {
                    
                    string[] LineItems = data.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
                    Console.WriteLine("Found Items:" + LineItems.Length);
                    for (int i = 0; i < LineItems.Length; i++)
                    {
                        Console.WriteLine("Processing Item " + i);
                        logger.Log(LogLevel.Info, "Processing Item :" + i);
                        string[] locarray = LineItems[i].Split(',');
                        if (locarray.Length > 4)
                        {
                            //Only check for Events Strings and Locations as part of POC 
                            if (locarray[1] == "06")
                            {
                                Console.WriteLine("it is an event :" + LineItems[i]);
                                //check that the event type is a location 01 , ingition On 04, ignition Off 05, Tracking 18, or external 33
                                if (locarray[4] == "01")
                                {
                                    Console.WriteLine("Location report");
                                    logger.Log(LogLevel.Info, "Location Report:" + LineItems[i]);
                                    StarlinkLocation loc = StarlinkLocation.parseLocationString(LineItems[i]);

                                    logger.Log(LogLevel.Info, "LAT:" + loc.Latitude.ToString("G", CultureInfo.InvariantCulture) + " Lon:" + loc.Longitude.ToString("G", CultureInfo.InvariantCulture) + " on unit : " + loc.SourceUnitId);
                                    Console.WriteLine("LAT:" + loc.Latitude.ToString("G", CultureInfo.InvariantCulture) + " Lon:" + loc.Longitude.ToString("G", CultureInfo.InvariantCulture) + " on unit : " + loc.SourceUnitId);
                                    Console.WriteLine("sending data to cmore");
                                    StarlinkLocation.SendToCmore(loc);
                                    Console.WriteLine("Sent");

                                    //Send Acknowlegdement
                                    Console.WriteLine("Acknowlegde to the Client");
                                    ServerResponderCounter++;
                                    string ResponseString = "SLU" + loc.SourceUnitId + ",02," + ServerResponderCounter + "," + loc.UnitReferenceNumber + ",01";
                                    ResponseString = ComputeCheckSum(ResponseString);
                                    byte[] msg = Encoding.UTF8.GetBytes(data);
                                    handler.Send(msg);
                                }
                                if (locarray[4] == "02")
                                {
                                    Console.WriteLine("Error report");
                                    logger.Log(LogLevel.Info, "Error Report:" + LineItems[i]);
                                    StarlinkLocation loc = StarlinkLocation.parseLocationString(LineItems[i]);

                                    //logger.Log(LogLevel.Info, "LAT:" + loc.Latitude.ToString("G", CultureInfo.InvariantCulture) + " Lon:" + loc.Longitude.ToString("G", CultureInfo.InvariantCulture) + " on unit : " + loc.SourceUnitId);
                                    //Console.WriteLine("LAT:" + loc.Latitude.ToString("G", CultureInfo.InvariantCulture) + " Lon:" + loc.Longitude.ToString("G", CultureInfo.InvariantCulture) + " on unit : " + loc.SourceUnitId);
                                    Console.WriteLine("sending data to cmore");
                                    StarlinkLocation.SendToCmore(loc);
                                    Console.WriteLine("Sent");

                                    //Send Acknowlegdement
                                    Console.WriteLine("Acknowlegde to the Client");
                                    ServerResponderCounter++;
                                    string ResponseString = "SLU" + loc.SourceUnitId + ",02," + ServerResponderCounter + "," + loc.UnitReferenceNumber + ",01";
                                    ResponseString = ComputeCheckSum(ResponseString);
                                    byte[] msg = Encoding.UTF8.GetBytes(data);
                                    handler.Send(msg);
                                }
                                else if (locarray[4] == "04")
                                {
                                    Console.WriteLine("ignition On");
                                    logger.Log(LogLevel.Info, "Ignition On:" + LineItems[i]);
                                    StarlinkLocation loc = StarlinkLocation.parseLocationString(LineItems[i]);
                                    Console.WriteLine("LAT:" + loc.Latitude.ToString("G", CultureInfo.InvariantCulture) + " Lon:" + loc.Longitude.ToString("G", CultureInfo.InvariantCulture) + " on unit : " + loc.SourceUnitId);
                                    Console.WriteLine("sending data to cmore");
                                    StarlinkLocation.SendToCmore(loc);
                                    Console.WriteLine("Sent");

                                    //Send Acknowlegdement
                                    Console.WriteLine("Acknowlegde to the Client");
                                    ServerResponderCounter++;
                                    string ResponseString = "SLU" + loc.SourceUnitId + ",02," + ServerResponderCounter + "," + loc.UnitReferenceNumber + ",01";
                                    ResponseString = ComputeCheckSum(ResponseString);
                                    byte[] msg = Encoding.UTF8.GetBytes(data);
                                    handler.Send(msg);
                                }
                                else if (locarray[4] == "05")
                                {
                                    Console.WriteLine("ignition Off");
                                    logger.Log(LogLevel.Info, "Ignition Off:" + LineItems[i]);
                                    StarlinkLocation loc = StarlinkLocation.parseLocationString(LineItems[i]);
                                    Console.WriteLine("LAT:" + loc.Latitude.ToString("G", CultureInfo.InvariantCulture) + " Lon:" + loc.Longitude.ToString("G", CultureInfo.InvariantCulture) + " on unit : " + loc.SourceUnitId);
                                    logger.Log(LogLevel.Info, "LAT:" + loc.Latitude.ToString("G", CultureInfo.InvariantCulture) + " Lon:" + loc.Longitude.ToString("G", CultureInfo.InvariantCulture) + " on unit : " + loc.SourceUnitId);
                                    Console.WriteLine("sending data to cmore");
                                    StarlinkLocation.SendToCmore(loc);
                                    Console.WriteLine("Sent");

                                    //Send Acknowlegdement
                                    Console.WriteLine("Acknowlegde to the Client");
                                    ServerResponderCounter++;
                                    string ResponseString = "SLU" + loc.SourceUnitId + ",02," + ServerResponderCounter + "," + loc.UnitReferenceNumber + ",01";
                                    ResponseString = ComputeCheckSum(ResponseString);
                                    byte[] msg = Encoding.UTF8.GetBytes(data);
                                    handler.Send(msg);
                                }
                                else if (locarray[4] == "18")
                                {
                                    Console.WriteLine("Tracking Event");
                                    logger.Log(LogLevel.Info, "Track Event:" + LineItems[i]);
                                    StarlinkLocation loc = StarlinkLocation.parseLocationString(LineItems[i]);
                                    Console.WriteLine("LAT:" + loc.Latitude.ToString("G", CultureInfo.InvariantCulture) + " Lon:" + loc.Longitude.ToString("G", CultureInfo.InvariantCulture) + " on unit : " + loc.SourceUnitId);
                                    logger.Log(LogLevel.Info, "LAT:" + loc.Latitude.ToString("G", CultureInfo.InvariantCulture) + " Lon:" + loc.Longitude.ToString("G", CultureInfo.InvariantCulture) + " on unit : " + loc.SourceUnitId);
                                    Console.WriteLine("sending data to cmore");
                                    StarlinkLocation.SendToCmore(loc);
                                    Console.WriteLine("Sent");

                                    //Send Acknowlegdement
                                    Console.WriteLine("Acknowlegde to the Client");
                                    ServerResponderCounter++;
                                    string ResponseString = "SLU" + loc.SourceUnitId + ",02," + ServerResponderCounter + "," + loc.UnitReferenceNumber + ",01";
                                    ResponseString = ComputeCheckSum(ResponseString);
                                    byte[] msg = Encoding.UTF8.GetBytes(data);
                                    handler.Send(msg);
                                }
                                else if (locarray[4] == "33")
                                {
                                    Console.WriteLine("Tracking Event");
                                    logger.Log(LogLevel.Info, "Track Event External:" + LineItems[i]);
                                    StarlinkLocation loc = StarlinkLocation.parseLocationString(LineItems[i]);
                                    Console.WriteLine("LAT:" + loc.Latitude.ToString("G", CultureInfo.InvariantCulture) + " Lon:" + loc.Longitude.ToString("G", CultureInfo.InvariantCulture) + " on unit : " + loc.SourceUnitId);
                                    logger.Log(LogLevel.Info, "LAT:" + loc.Latitude.ToString("G", CultureInfo.InvariantCulture) + " Lon:" + loc.Longitude.ToString("G", CultureInfo.InvariantCulture) + " on unit : " + loc.SourceUnitId);
                                    Console.WriteLine("sending data to cmore");
                                    StarlinkLocation.SendToCmore(loc);
                                    Console.WriteLine("Sent");

                                    //Send Acknowlegdement
                                    Console.WriteLine("Acknowlegde to the Client");
                                    ServerResponderCounter++;
                                    string ResponseString = "SLU" + loc.SourceUnitId + ",02," + ServerResponderCounter + "," + loc.UnitReferenceNumber + ",01";
                                    ResponseString = ComputeCheckSum(ResponseString);
                                    byte[] msg = Encoding.UTF8.GetBytes(data);
                                    handler.Send(msg);
                                }
                                else
                                {
                                    Console.WriteLine("Not a Valid Event for POC : Event Code is - "+ locarray[4]);

                                    Console.WriteLine(LineItems[i]);
                                }
                            }
                            else
                            {
                                Console.WriteLine("Not an event :" + LineItems[i]);
                            }
                        }
                    }
                    Console.WriteLine(DateTime.Now.ToString("yyyy/MM/dd HH:mm:sss")+"Completed Processing");
                }
                else
                {
                    Console.WriteLine("Not a Starlink Device");
                }
                // Echo the data back to the client.  
             
                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
                Console.WriteLine("Socket Closed");
            }

        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }

        Console.WriteLine("\nPress ENTER to continue...");
        Console.Read();

    }

    public static string ComputeCheckSum(string msg)
    {
        int cs = 0;
        // calculate checksum
        for (int i = 0; i < msg.Length; i++)
        {
            cs += msg[i];
        }

        // modulo 256
        cs %= 256;

        // repeat $ and *
        string msgOut = "";
        for (int i = 0; i < msg.Length; i++)
        {
            msgOut += msg[i];
            if ((msg[i] == '$') || (msg[i] == '*'))
            {
                msgOut += msg[i];
            }
        }

        // convert check-sum to a two digit hexadecimal number
        string checksum = ("0" + cs.ToString("X")).ToUpper();
        checksum = checksum.Substring(checksum.Length-2,2);

        // return result
        string pre = "$";
        string post = "";
        return pre + msgOut + '*' + checksum + post;
    }

    public static void ProcessItems(string[] LineItems)
    {
       
    }

    public static void TestCmore()
    {
        string data = "$SLU352353081322635,06,409,190730100952,33,190730100953,-2547.8659,+02819.5758,000.0,053,000011.446,00713,51351,0,12.819,03.381,,,,6,14,26.0,5,1458,78,7,0,137,5,3,5250,99,52,99*6C" + "\r\n";

        string[] LineItems = data.Split(new[] { Environment.NewLine }, StringSplitOptions.None);

        ProcessItems(LineItems);
    }

    private static byte[] Replace(byte[] input, byte[] pattern, byte[] replacement)
    {
        if (pattern.Length == 0)
        {
            return input;
        }

        List<byte> result = new List<byte>();

        int i;

        for (i = 0; i <= input.Length - pattern.Length; i++)
        {
            bool foundMatch = true;
            for (int j = 0; j < pattern.Length; j++)
            {
                if (input[i + j] != pattern[j])
                {
                    foundMatch = false;
                    break;
                }
            }

            if (foundMatch)
            {
                result.AddRange(replacement);
                i += pattern.Length - 1;
            }
            else
            {
                result.Add(input[i]);
            }
        }

        for (; i < input.Length; i++)
        {
            result.Add(input[i]);
        }

        return result.ToArray();
    }


    public static string ByteArrayToString(byte[] ba)
    {
        StringBuilder hex = new StringBuilder(ba.Length * 2);
        foreach (byte b in ba)
            hex.AppendFormat("{0:x2}", b);
        return hex.ToString();
    }

}