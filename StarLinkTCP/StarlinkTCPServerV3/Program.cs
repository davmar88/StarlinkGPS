using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace StarlinkTCPServerV3
{
    class Program
    {

        static void Main()
        {
            StartServer();
            //byte[] buffer = new byte[] { 0x10, 0x10 };
            //buffer = BytePatternUtilities.ReplaceBytes(buffer, new byte[] { 0x10, 0x10 }, new byte[] { 0x10 });
            //buffer = BytePatternUtilities.ReplaceBytes(buffer, new byte[] { 0x10, 0x01 }, new byte[] { 0x0D });
            //buffer = BytePatternUtilities.ReplaceBytes(buffer, new byte[] { 0x10, 0x02 }, new byte[] { 0x0A });
            //buffer = BytePatternUtilities.ReplaceBytes(buffer, new byte[] { 0x10, 0x03 }, new byte[] { 0x00 });
            //buffer = BytePatternUtilities.ReplaceBytes(buffer, new byte[] { 0x10, 0x04 }, new byte[] { 0x11 });
            //buffer = BytePatternUtilities.ReplaceBytes(buffer, new byte[] { 0x10, 0x05 }, new byte[] { 0x13 });
            //buffer = BytePatternUtilities.ReplaceBytes(buffer, new byte[] { 0x24, 0x24 }, new byte[] { 0x24 });
            //buffer = BytePatternUtilities.ReplaceBytes(buffer, new byte[] { 0x2A, 0x2A }, new byte[] { 0x2A });

        }
        private static TcpListener _listener;
        public static void StartServer()
        {
            try
            {
                Console.WriteLine("Starting Server");
                 IPAddress ipAddress = System.Net.IPAddress.Parse("154.0.161.164");
                //System.Net.IPAddress localIPAddress = System.Net.IPAddress.Parse("10.0.0.15");
                IPEndPoint ipLocal = new IPEndPoint(ipAddress, 9000);
                _listener = new TcpListener(ipLocal);
                _listener.Start();

                Console.WriteLine("Started");
                WaitForClientConnect();
                Console.WriteLine("Waiting for Connection");
                Console.Read();
            }
            catch (Exception ex)
            {
                Console.WriteLine("TCP Exception: StartServer: " + ex.Message + ex.StackTrace);
            }
        }

        public static void StopServer()
        {
            try
            {
                _listener.Stop();
                if (_listener != null)
                {
                    _listener = null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("TCP Exception: StopServer: " + ex.Message + ex.StackTrace);
            }
        }
        private static void WaitForClientConnect()
        {
            try
            {
                object obj = new object();
                _listener.BeginAcceptTcpClient(new System.AsyncCallback(OnClientConnect), obj);
            }
            catch (Exception ex)
            {
                Console.WriteLine("TCP Exception:" + ex.Message + ex.StackTrace);
            }
            // WaitForClientConnect();
        }
        private static void OnClientConnect(IAsyncResult asyn)
        {
            try
            {
                if (_listener != null)
                {
                    Console.WriteLine("Client Connected");
                    TcpClient clientSocket = default(TcpClient);
                    clientSocket = _listener.EndAcceptTcpClient(asyn);
                    HandleClientRequest clientReq = new HandleClientRequest(clientSocket);
                    clientReq.StartClient();
                }
                else
                {
                    Console.WriteLine("OnClientConnect: LISTENER IS NULL");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("TCP Exception:" + ex.Message + ex.StackTrace);
            }

            WaitForClientConnect();
        }


    }


    public class HandleClientRequest
    {
        TcpClient _clientSocket;
        string data = "";
        int CommandLength = 0;
        NetworkStream _networkStream = null;
        public HandleClientRequest(TcpClient clientConnected)
        {
            this._clientSocket = clientConnected;
        }
        public void StartClient()
        {
            _networkStream = _clientSocket.GetStream();
            WaitForRequest();
        }

        public void WaitForRequest()
        {
            bool error = false;
            try
            {
                Console.WriteLine("Reading Data");
                byte[] buffer = new byte[_clientSocket.ReceiveBufferSize];

                Console.WriteLine("buffer length: " + buffer.Length);
                _networkStream.BeginRead(buffer, 0, buffer.Length, ReadCallback, buffer);
            }
            catch (Exception ex)
            {
                error = true;
               // AsyncTCPServer.MailErrors();
                Console.WriteLine("TCP Exception: WaitForRequest: " + ex.Message + ex.StackTrace);
            }

            if (error)
            {
                // StartClient();
                //try
                //{
                //    AsyncTCPServer.StopServer();
                //}
                //catch (Exception ex)
                //{
                //    Logger.LogData("TCP Exception: WaitForRequest: Could not stop server: " + ex.Message + ex.StackTrace);
                //    Console.WriteLine("TCP Exception: WaitForRequest: Could not stop server: " + ex.Message + ex.StackTrace);
                //}

                //try
                //{
                //    AsyncTCPServer.StartServer();
                //}
                //catch (Exception ex)
                //{
                //    Logger.LogData("TCP Exception: WaitForRequest: Could not start server: " + ex.Message + ex.StackTrace);
                //    Console.WriteLine("TCP Exception: WaitForRequest: Could not start server: " + ex.Message + ex.StackTrace);
                //}
            }
        }

        private void ReadCallback(IAsyncResult result)
        {
            NetworkStream networkStream = _clientSocket.GetStream();
            try
            {
                int read = networkStream.EndRead(result);
                if (read == 0)
                {
                    _networkStream.Close();
                    _clientSocket.Close();
                    return;
                }

                byte[] buffer = result.AsyncState as byte[];
                data += Encoding.Default.GetString(buffer, 0, read);

                Console.WriteLine("Data is :"+ data);

                data = null;

                        ////Return Response
                        //if (Response != "" && Response != "[]")
                        //{
                        //    byte[] msg = System.Text.Encoding.Default.GetBytes(Response);
                        //    networkStream.Write(msg, 0, msg.Length);
                        //    networkStream.Flush();

                        //    Console.WriteLine("Out -> : " + Response + " " + DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss.fff"));
                        //    Logger.LogData("Out -> : " + Response + " " + DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss.fff"));
                        //}
        

            }
            catch (Exception ex)
            {
              
                Console.WriteLine("TCP Processing Exception:" + ex.Message + ex.StackTrace);
            }

            this.WaitForRequest();
        }


        private static string ProcessData(string data)
        {
            string Response = "";
            try
            {
                if (data.Length > 2)
                {
                    //Remove Brackets First and Last Char
                    data = data.Substring(1);
                    data = data.Substring(0, data.Length - 1);
                    //Response = CommandParser.ParseCommand(data);
                }
                //Logger.LogData("Data TO PROCESS: " + data + " : data Length: " + data.Length);
            }
            catch (Exception ex)
            {
                //Logger.LogData("Process Data:" + ex.Message + ex.StackTrace);
                Console.WriteLine("Process Data:" + ex.Message + ex.StackTrace);
            }
            return Response;
        }

    }
}