using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace CheckPwnedPasswords
{
    class Program
    {
        static void Main(string[] args)
        {
            var files = new List<string>() { @"F:\pwned-passwords-1.0.txt", @"F:\pwned-passwords-update-1.txt", @"F:\pwned-passwords-update-2.txt" };

            while (true)
            {
                Console.WriteLine("");
                Console.WriteLine("");
                Console.WriteLine("Enter password:");
                var pwd = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(pwd)) return;

                foreach (var file in files)
                {
                    var sw = new Stopwatch();
                    sw.Start();
                    var result = Check(pwd, file);
                    sw.Stop();


                    if (result)
                    {
                        Console.WriteLine($"{pwd} is in {file} - time taken was {sw.Elapsed.Milliseconds}ms");
                        break;
                    }
                    else
                    {
                        Console.WriteLine($"{pwd} is NOT in {file} - time taken was {sw.Elapsed.Milliseconds}ms");
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

            //Length of the SHA1 hash  (we use LINELENGTH+2 is take the newline characters into account)
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
