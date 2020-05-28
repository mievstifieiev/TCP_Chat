using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net.Http.Headers;

namespace TCP_Client
{
    class Program
    {
        static int port = 8005; // порт сервера
        static string address = "127.0.0.1"; //адрес сервера
        static string UserName;
        static Socket socket;
        static IPEndPoint ipEnd;

        static void Main()
        {
            Console.Write("Введите свое имя: ");
            UserName = Console.ReadLine();
            try
            {
                ipEnd = new IPEndPoint(IPAddress.Parse(address), port);

                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                //устанавливаем соединение
                socket.Connect(ipEnd);
                Thread reTH = new Thread(ReceiveMassege);
                reTH.Start();
                SendMessege();

                //получаем ответ
                //ReceiveMassege();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Disconnect();
        }

        static void SendMessege()
        {
            Console.WriteLine(UserName + ": ");
            while (true)
            {
                string massage = Console.ReadLine();
                byte[] data = Encoding.Unicode.GetBytes(UserName + ": " + massage);
                socket.Send(data);
            }
        }

        static void ReceiveMassege()
        {
            while (true)
            {
                try
                {
                    byte[] data = new byte[256]; // буффер для ответа
                    StringBuilder builder = new StringBuilder();
                    int bytes = 0;
                    do
                    {
                        bytes = socket.Receive(data, data.Length, 0);
                        builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                    } while (socket.Available > 0);
                    Console.WriteLine(builder.ToString());
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        static void Disconnect()
        {
            string massage = " Пользователь отключился от чата";
            byte[] data = Encoding.Unicode.GetBytes(UserName + ": " + massage);
            socket.Send(data);
            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
            Environment.Exit(0);
        }
    }
}
