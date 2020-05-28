using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace TCP_Server
{
    public class ClientObject
    {
        protected internal string Id { get; private set; }
        protected internal Socket client;
        ServerObject server; // объект сервера

        public ClientObject(Socket Client, ServerObject serverObject)
        {
            Id = Guid.NewGuid().ToString();
            client = Client;
            server = serverObject;
            serverObject.AddConnection(this);
        }

        public void Process()
        {
            try
            {
                string message = "Пользователь вошел в чат";
                // посылаем сообщение о входе в чат всем подключенным пользователям
                server.BroadcastMessage(message, this.Id);
                Console.WriteLine(message);
                // в бесконечном цикле получаем сообщения от клиента
                while (true)
                {
                    try
                    {
                        string messageCli = GetMessage(client);
                        Console.WriteLine(messageCli);
                        server.BroadcastMessage(messageCli, this.Id);
                    }
                    catch
                    {

                        message = String.Format("Пользователь покинул чат");
                        Console.WriteLine(message);
                        server.BroadcastMessage(message, this.Id);
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                // в случае выхода из цикла закрываем ресурсы
                server.RemoveConnection(this.Id);
                Close();
            }
        }

        // чтение входящего сообщения и преобразование в строку
        private string GetMessage(Socket client)
        {
            byte[] data = new byte[64]; // буфер для получаемых данных
            StringBuilder builder = new StringBuilder();
            int bytes = 0;
            do
            {
                bytes = client.Receive(data, data.Length, 0);
                builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
            }
            while (client.Available>0);

            return builder.ToString();
        }

        // закрытие подключения
        protected internal void Close()
        {
                client.Shutdown(SocketShutdown.Both);
                client.Close();
            
        }
    }
}
