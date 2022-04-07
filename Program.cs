using System;
using System.IO;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading.Channels;
using Laba2_OS;

namespace ConsoleApplication1
{
    class Program
    {
        const string PATH = "passwordHashes.txt";
        static public bool foundFlag = false;

        static void printMenu()
        {
            bool flag = true;
            while (flag)
            {
                Console.WriteLine("1. Выполнение.");
                Console.WriteLine("2. Очистить консоль.");
                Console.WriteLine("3. Exit");
                Console.Write("Выберите пункт меню: ");
                int choice = int.Parse(Console.ReadLine());
                switch (choice)
                {
                    case 1:
                        Console.WriteLine("\tВыберите по какому хеш значению подобрать пароль: ");
                        Console.WriteLine("\t1.");
                        Console.WriteLine("\t2.");
                        Console.WriteLine("\t3.");
                        Console.Write("\t---> ");
                        int sign = int.Parse(Console.ReadLine());
                        string[] readText = File.ReadAllLines(PATH);
                        string passwordHash = readText[sign - 1].ToUpper();
                        Console.Write("\tВведите количество потоков: ");
                        int countStream = int.Parse(Console.ReadLine());
                        Console.WriteLine("\t...");

                        Channel<string> channel = Channel.CreateBounded<string>(countStream);
                        Stopwatch time = new();
                        time.Reset();
                        time.Start();
                        var prod = Task.Run(() => { new Producer(channel.Writer); });
                        Task[] streams = new Task[countStream + 1];
                        streams[0] = prod;
                        for (int i = 1; i < countStream + 1; i++)
                        {
                            streams[i] = Task.Run(() => { new Consumer(channel.Reader, passwordHash); });
                        }
                        Task.WaitAny(streams);
                        time.Stop();
                        Console.WriteLine($"\tЗатраченное время на подбор: {time.Elapsed}");
                        Console.WriteLine("\tВведите ENTER, чтобы выйти в главное меню.");
                        Console.WriteLine();
                        Console.ReadKey();
                        foundFlag = false;
                        break;
                    case 2:
                        Console.Clear();
                        break;
                    case 3:
                        flag = false;
                        break;
                    default:
                        Console.WriteLine("\tВыбранного пункта нет в меню.");
                        break;
                }
            }
        }

        static public void Main()
        {
            printMenu();
        }
    }
}