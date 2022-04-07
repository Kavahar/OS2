using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleApplication1;
using System.Security.Cryptography;
using System.Threading.Channels;

namespace Laba2_OS
{
    class Consumer
    {
        private ChannelReader<string> Reader;
        private string PasswordHash;

        public Consumer(ChannelReader<string> _reader, string _passwordHash)
        {
            Reader = _reader;
            PasswordHash = _passwordHash;
            Task.WaitAll(Run());
        }

        private async Task Run()
        {
            while (await Reader.WaitToReadAsync())
            {
                if (!Program.foundFlag)
                {
                    var item = await Reader.ReadAsync();
                    if (FoundHash(item.ToString()) == PasswordHash)
                    {
                        Console.WriteLine($"\tПароль подобран - {item}");
                        Program.foundFlag = true;
                    }
                }
                else return;
            }
        }
        /// <summary>
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        static public string FoundHash(string str)
        {
            SHA256 sha256Hash = SHA256.Create();
            byte[] sourceBytes = Encoding.ASCII.GetBytes(str);
            byte[] hashBytes = sha256Hash.ComputeHash(sourceBytes);
            string hash = BitConverter.ToString(hashBytes).Replace("-", String.Empty);
            return hash;
        }

    }
}