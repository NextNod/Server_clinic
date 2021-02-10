using System;
using DataBase;
using Server;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;

namespace Server_clinic
{
    class Program
    {
        static void Main(string[] args)
        {
            TcpListener listener = new TcpListener(801);
            listener.Start();
            DataBase.DataBase dataBase = new DataBase.DataBase();

            while (true)
            {
                Console.WriteLine("Waiting client...");
                TcpClient server = listener.AcceptTcpClient();
                
                Console.WriteLine("Client accepted!");
                NetworkStream stream = server.GetStream();
                
                string type = getData(stream);

                if (type == "get")
                {
                    Console.WriteLine("type: get");
                    List<DataBase.Data> data = dataBase.GetData();
                    string dataStream = "";

                    foreach (DataBase.Data dat in data)
                    {
                        dataStream += dat.ID + " " + dat.name + ":" + dat.discription + ";";
                    }
                    dataStream += ".";

                    Thread.Sleep(5);
                    sendData(stream, Convert.ToString(Encoding.UTF8.GetBytes(dataStream).Length));
                    Thread.Sleep(5);
                    sendData(stream, dataStream);
                }
                else if (type == "ver")
                {
                    Console.WriteLine("type: ver");
                    string data = Convert.ToString(dataBase.ver);
                    Console.WriteLine(data);
                    sendData(stream, data);
                }
                else if (type == "send")
                {
                    Console.WriteLine("type: send");
                    sendData(stream, "ready");
                    string data = getData(stream), temp = "", number = "";
                    int ID = 0;

                    try
                    {
                        int i = 0;
                        while (true)
                        {
                            if (data[i] == ' ')
                            {
                                ID = Convert.ToInt32(temp);
                                temp = "";
                                i++;

                                while (true)
                                {
                                    temp += data[i];
                                    i++;
                                }
                            }

                            temp += data[i];
                            i++;
                        }
                    }
                    catch
                    {
                        number = temp;
                    }

                    dataBase.createNote(ID, number);
                }
                else if (type == "orderDate")
                {
                    sendData(stream, "OK");
                    int docId = Convert.ToInt32(getData(stream));
                    List<string> dates = dataBase.orderDates(docId);
                    string data = "";

                    for (int i = 0; i < dates.Count; i++)
                    {
                        data += dates[i] + ",";
                    }

                    sendData(stream, Convert.ToString(Encoding.UTF8.GetBytes(data).Length));
                    getData(stream);
                    sendData(stream, data);
                }
                else if (type == "order") 
                {
                    sendData(stream, "OK");
                    int lenght = Convert.ToInt32(getData(stream));
                    sendData(stream, "OK");
                    string data = getData(stream, lenght), temp = "";

                    string[] res = data.Split(':');

                    dataBase.newOrder(Convert.ToInt32(res[0]), res[1], res[2], res[3], res[4], res[5]);
                }

                stream.Close();
                server.Close();
            }
        }


        private static string getData(NetworkStream stream, int lenght = 255)
        {
            byte[] vs = new byte[lenght];
            stream.Read(vs, 0, vs.Length);
            string data = Encoding.UTF8.GetString(vs), res = "";
            data += "\0";


            for (int i = 0; data[i] != '\0'; i++)
            {
                res += data[i];
            }

            return res;
        }

        private static void sendData(NetworkStream stream, string data) 
        {
            byte[] vs = new byte[255];
            vs = Encoding.UTF8.GetBytes(data);
            stream.Write(vs, 0, vs.Length);
        }
    }
}
