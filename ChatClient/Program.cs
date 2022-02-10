using System;
using System.Threading;
using System.Net.Sockets;
using System.Text;

namespace ChatClient
{
    class Program
    {
        static string userName = "";
        static TcpClient client = new TcpClient();
        static NetworkStream stream;

        static void Main(string[] args)
        {
            
            for(; ;) {

                Console.Clear();
                Console.Write("Enter your name: ");
                userName = Console.ReadLine() ?? "";

                if(userName.Length > 0) break;
            }
            try
            {
                client.Connect("192.168.31.1", 22); 
                stream = client.GetStream();

                string message = userName;
                byte[] data = Encoding.Unicode.GetBytes(message);
                stream.Write(data, 0, data.Length);

                Thread receiveThread = new Thread(new ThreadStart(ReceiveMessage));
                receiveThread.Start();

                Console.WriteLine("Welcome, {0}", userName);
                SendMessage();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                Disconnect();
            }
        }



        static void SendMessage() {

            Console.WriteLine("Enter message: ");

            while (true) {

                string message = Console.ReadLine();
                byte[] data = Encoding.Unicode.GetBytes(message);
                stream.Write(data, 0, data.Length);
            }
        }
        static void ReceiveMessage() {
            while (true) {
                try {
                    byte[] data = new byte[64];
                    int bytes = 0;

                    do {

                     bytes = stream.Read(data, 0, data.Length);
                     new StringBuilder().Append(Encoding.Unicode.GetString(data, 0, bytes));

                    } while (stream.DataAvailable);


                    string message = new StringBuilder().ToString();
                    Console.WriteLine(message);
                }
                catch
                {
                    Console.WriteLine("Error, please try again.");
                    Console.ReadLine();
                    Disconnect();
                }
            }
        }

        static void Disconnect()
        {
            try
            {
                if (stream != null)
                    stream.Close();

                if (client != null)
                    client.Close();

                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            
        }
    }
}