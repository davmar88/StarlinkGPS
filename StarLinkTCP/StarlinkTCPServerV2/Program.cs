using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarlinkTCPServerV2
{
    class Program
    {
        static void Main(string[] args)
        {
            //SynchronousSocketListener.StartListening();
            AsynchronousSocketListener.StartListening();
            //SynchronousSocketListener.TestCmore();
            //Console.ReadLine();
        }
    }
}
