using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace CheckPwnedPasswords
{
    class Program
    {

        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("");
                Console.WriteLine("");
                Console.WriteLine("Enter password:");
                var pwd = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(pwd)) return;

                var sw = new Stopwatch();
                sw.Start();
                var result = Check(pwd, @"F:\pwned-passwords-1.0.txt");
                sw.Stop();

                if (result)
                {
                    Console.WriteLine($"{pwd} is in the main list - time taken was {sw.Elapsed.Milliseconds}ms");
                }
                else
                {
                    Console.WriteLine($"{pwd} is NOT in the main list - time taken was {sw.Elapsed.Milliseconds}ms");

                    sw = new Stopwatch();
                    sw.Start();
                    result = Check(pwd, @"F:\pwned-passwords-update-1.txt");
                    sw.Stop();

                    if (result)
                    {
                        Console.WriteLine($"{pwd} is in the update list - time taken was {sw.Elapsed.Milliseconds}ms");
                    }
                    else
                    {
                        Console.WriteLine($"{pwd} is NOT in the update list - time taken was {sw.Elapsed.Milliseconds}ms");
                    }

                }


            }
        }

        static bool Check(string password, string filename)
        {
            string ByteArrayToString(byte[] ba)
            {
                string hex = BitConverter.ToString(ba);
                return hex.Replace("-", "");
            }

            var bytes = System.Text.Encoding.UTF8.GetBytes(password);
            var sha = System.Security.Cryptography.SHA1.Create();
            var passwordBytes = sha.ComputeHash(bytes);
            var asHex = ByteArrayToString(passwordBytes);
            const int LINELENGTH = 40;

            var buffer = new byte[LINELENGTH];
            using (var sr = File.OpenRead(filename))
            {
                //Number of lines
                var high = (sr.Length / (LINELENGTH + 2)) - 1;
                var low = 0L;

                while (low <= high)
                {
                    var middle = (low + high + 1) / 2;
                    sr.Seek((LINELENGTH + 2) * ((long)middle), SeekOrigin.Begin);
                    sr.Read(buffer, 0, LINELENGTH);
                    var readLine = Encoding.ASCII.GetString(buffer);

                    switch (readLine.CompareTo(asHex))
                    {
                        case 0:
                            return true;

                        case 1:
                            high = middle - 1;
                            break;

                        case -1:
                            low = middle + 1;
                            break;

                        default:
                            break;
                    }
                }

            }

            return false;

        }
    }
}
