using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using StarlinkTCPServerV2.Processor;


// State object for reading client data asynchronously  
public class StateObject
{
    // Client  socket.  
    public Socket workSocket = null;
    // Size of receive buffer.  
    public const int BufferSize = 1024;
    // Receive buffer.  
    public byte[] buffer = new byte[BufferSize];
    // Received data string.  
    public StringBuilder sb = new StringBuilder();
}

public class AsynchronousSocketListener
{
    // Thread signal.  
    public static ManualResetEvent allDone = new ManualResetEvent(false);
    

    public AsynchronousSocketListener()
    {
    }

    /*static IPAddress ipAddress = System.Net.IPAddress.Parse("154.0.161.164");
    static IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 5000);
    // Create a TCP/IP socket.  
    static Socket listener = new Socket(ipAddress.AddressFamily,SocketType.Stream, ProtocolType.Tcp);*/

    public static void StartListening()
    {

        Console.WriteLine("Starting Asyncronous Server for a connection on 154.0.161.164:5000");
        // Establish the local endpoint for the socket.  
        // The DNS name of the computer  
        // running the listener is "host.contoso.com".  
        //IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());

        IPAddress ipAddress = System.Net.IPAddress.Parse("154.0.161.164");
        IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 5000);
        // Create a TCP/IP socket.  
        Socket listener = new Socket(ipAddress.AddressFamily,SocketType.Stream, ProtocolType.Tcp);

        //Testing and debugging purposes 
        // string dummy = "$SLU352353082679579,06,49,191212093307,02,191209185252,-2547.8637,+02819.5755,000.0,200,000000.004,00713,51022,0,11.989,03.317,0,100,0.01,9,2,25.0,60,1449,85,15,1,17,0/0/2 1/1/1/1 02C9:C74E@-85 (51) 655001 VodaCom-SA*77";
        //Marietjie testing parser
        //  TrackProcessor.ProcessTrackMessage(StarlinkLocation.parseLocationString(dummy));


        // Bind the socket to the local endpoint and listen for incoming connections.  
        try
        {
            listener.Bind(localEndPoint);
            listener.Listen(100);

            while (true)
            {
                // Set the event to nonsignaled state.  
                allDone.Reset();

                // Start an asynchronous socket to listen for connections.  
                Console.WriteLine("Waiting for a connection...");
                listener.BeginAccept(
                    new AsyncCallback(AcceptCallback),
                    listener);

                // Wait until a connection is made before continuing.  
                allDone.WaitOne();
            }

        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }

        Console.WriteLine("\nPress ENTER to continue...");
        Console.Read();

    }

    public static void AcceptCallback(IAsyncResult ar)
    {
        try
        {
            allDone.Set();

            Console.WriteLine("Starting AcceptCallback Method");
            Console.WriteLine("AcceptCallback Method Async State: " + ar.AsyncState.ToString());

            // Get the socket that handles the client request.  
            Socket listener = (Socket)ar.AsyncState;
            Socket handler = listener.EndAccept(ar);

            // Create the state object.  
            StateObject state = new StateObject();
            state.workSocket = handler;

            handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                new AsyncCallback(ReadCallback), state);
        }
        catch (Exception e)
        {

            Console.WriteLine("In the AcceptCallback Method,line 89, an error occurred: " + e.Message);
        }
        // Signal the main thread to continue.  
        
    }

    public static void ReadCallback(IAsyncResult ar)
    {
        try
        {
            Console.WriteLine("Starting ReadCallback Method");
            Console.WriteLine("ReadCallback Method Async State: " + ar.AsyncState.ToString());
            String content = String.Empty;

            // Retrieve the state object and the handler socket  
            // from the asynchronous state object.  
            StateObject state = (StateObject)ar.AsyncState;
            Socket handler = state.workSocket;

            // Read data from the client socket.   
            int bytesRead = handler.EndReceive(ar);
            Console.WriteLine("ReadCallback bytesRead total: " + bytesRead.ToString());

            if (bytesRead > 0)
            {

                Console.WriteLine("BytesRead total more that ZERO, Proceeding to content");

                // There  might be more data, so store the data received so far.  
                state.sb.Append(Encoding.ASCII.GetString(
                    state.buffer, 0, bytesRead));
                content = state.sb.ToString();
                // Check for end-of-file tag. If it is not there, read   
                // more data.  

                if (content.Contains("$"))
                {
                    if (content.IndexOf("\r\n") > -1)
                    {
                        Console.WriteLine("We are passed content IndexOff");
                        Console.WriteLine("=====================================================================================================");
                        Console.WriteLine("");
                        Console.WriteLine("");
                        Console.WriteLine("");
                        Console.WriteLine("");
                        Console.WriteLine("=====================================================================================================");
                        string[] computeString = content.Split(',');
                        string server = "$SRV";
                        string unitNr = computeString[0].Substring(4);
                        string confirmation = "02";
                        string ServerRefnr = "11";
                        string unitRefnr = computeString[2];
                        string success = "01*";


                        string[] result = content.Split(new string[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (string s in result)
                        {
                            Console.WriteLine(DateTime.Now.ToString("yyyy/MM/dd HH:mm:sss"));
                            Console.WriteLine("Content data: " + s);
                            string[] computeStringTwo = s.Split(',');

                            //  string unitNrTwo = computeString[0].Substring(4);

                            string unitRefnrTwo = computeStringTwo[2];

                            Console.WriteLine("Parsing the data");



                            string whole = server + unitNr + "," + confirmation + "," + ServerRefnr + "," + unitRefnrTwo + "," + success;
                            string checksum = CalculateChecksum(whole);


                            string response = whole + checksum;
                            Console.WriteLine("Response: " + response);

                            Console.WriteLine("");
                            Console.WriteLine("");
                            Console.WriteLine("=====================================================================================================");
                            Console.WriteLine("TrackProcessor:");
                            Console.WriteLine("");
                            Console.WriteLine("");
                            Console.WriteLine(s);
                            string dummy = "$SLU352353082679579,06,49,191212093307,02,191209185252,-2547.8637,+02819.5755,000.0,200,000000.004,00713,51022,0,11.989,03.317,0,100,0.01,9,2,25.0,60,1449,85,15,1,17,0 / 0 / 2 1 / 1 / 1 / 1 02C9: C74E@-85(51) 655001 VodaCom - SA * 77";
                            //Marietjie testing parser
                            TrackProcessor.ProcessTrackMessage(StarlinkLocation.parseLocationString(s));
                            Console.WriteLine("");
                            Console.WriteLine("");
                            Console.WriteLine("=====================================================================================================");
                            // Echo the data back to the client.  
                            //kyk na event report
                            Send(handler, response);
                        }
                    }
                    else
                    {
                        Console.WriteLine("Not a Starlink Device");
                    }







                    //Console.WriteLine("Waiting for a connection...");








                }
                else
                {
                    // Not all data received. Get more.  
                    handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                    new AsyncCallback(ReadCallback), state);
                }

                Console.WriteLine("Waiting for a connection...");
                Console.WriteLine("=====================================================================================================");


            }
        }
        catch (Exception e)
        {

            Console.WriteLine("In the ReadCallback Method,line 89, an error occurred: " + e.Message);
        }
        
    }

    private static void Send(Socket handler, String data)
    {
        // Convert the string data to byte data using ASCII encoding.  
        byte[] byteData = Encoding.ASCII.GetBytes(data);

        // Begin sending the data to the remote device.  
       // handler.Send(byteData, 0, byteData.Length, 0,
           //new AsyncCallback(SendCallback), handler);


        handler.Send(byteData);
        
        Console.WriteLine("Sent bytes to client.");
        Console.WriteLine("=====================================================================================================");
        Console.WriteLine("");
        Console.WriteLine("");
        Console.WriteLine("");
        Console.WriteLine("");
        Console.WriteLine("=====================================================================================================");
    }

    private static void SendCallback(IAsyncResult ar)
    {
        try
        {
            // Retrieve the socket from the state object.  
            Socket handler = (Socket)ar.AsyncState;

            // Complete sending the data to the remote device.  
            int bytesSent = handler.EndSend(ar);
            Console.WriteLine("Sent {0} bytes to client.", bytesSent);

           // handler.Shutdown(SocketShutdown.Both);
          //  handler.Close();

        }
        catch (Exception e)
        {
            Console.WriteLine("Error message at SendCallBack: "+e.Message);
        }
    }

    //Marietjie Davel: Adding Checksum Calculator
    private static string CalculateChecksum(string dataToCalculate)
    {
        byte[] byteToCalculate = Encoding.ASCII.GetBytes(dataToCalculate);
        int checksum = 0;
        foreach (byte chData in byteToCalculate)
        {
            checksum += chData;
        }
        checksum &= 0xff;
        return checksum.ToString("X2");
    }



   

}