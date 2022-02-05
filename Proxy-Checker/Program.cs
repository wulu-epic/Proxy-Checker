using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using System.Net.NetworkInformation;
using System.Threading;

namespace Proxy_Checker
{
    class Program
    {
        public static SynchronizedCollection<string> untested = new SynchronizedCollection<string>();
        public static SynchronizedCollection<string> working = new SynchronizedCollection<string>();
        public static List<string> wo = new List<string>();
        public static RichTextBox rtb = new RichTextBox();

        public static int remaining;
        public static int dead;
        public static int alive;
        public static int error;
        public static int cheked;
        public static int amount;

        public static string currentTmie = DateTime.Now.ToString("dd-MM-yyyy hh-mm-ss");
        public static int threads;

        public static bool Check(string proxy)
        {
            try
            {
                Ping ping = new Ping();
                var sent = ping.Send(proxy);
                if (sent.Status == IPStatus.Success)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"[#] {proxy}: has failed.", Console.ForegroundColor = ConsoleColor.Blue);
                error += 1;
                return false;
            }
        }

        public static void writeFile(string code)
        {
            try
            {
                string file = Environment.CurrentDirectory + @"\\Results\\[Alive] " + currentTmie + ".txt";
                File.AppendAllText(file, code + Environment.NewLine);
            } catch(Exception ex)
            {
                Console.WriteLine($"[#] {code}: Failed to save", Console.ForegroundColor = ConsoleColor.Blue);
                error += 1;
            }
        }

        [STAThread]
        public static void Main(string[] args)
        {
            Console.Title = "Proxy Checker by wulu#0827";
            Console.ForegroundColor = ConsoleColor.Magenta;


            string wulu = @"
                                    ____    __    ____  __    __   __       __    __  
                                    \   \  /  \  /   / |  |  |  | |  |     |  |  |  | 
                                     \   \/    \/   /  |  |  |  | |  |     |  |  |  | 
                                      \            /   |  |  |  | |  |     |  |  |  | 
                                       \    /\    /    |  `--'  | |  `----.|  `--'  | 
                                        \__/  \__/      \______/  |_______| \______/   
";
            Console.WriteLine(String.Format("{0," + Console.WindowWidth / 2 + "}", wulu));

            Console.WriteLine("[?] How many threads?", Console.ForegroundColor = ConsoleColor.Blue);
            Console.Write("> ");
            Console.ForegroundColor = ConsoleColor.Magenta;
            threads = Convert.ToInt32(Console.ReadLine());

            Console.WriteLine("[!] Open a proxy file", Console.ForegroundColor = ConsoleColor.Blue);
            Console.Write("> ");

            Thread.Sleep(500);

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Text files (*.txt)|*.txt";
            ofd.Title = "Proxies";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                rtb.Text = File.ReadAllText(ofd.FileName);
            }

            Console.Write($"Found {rtb.Lines.Count().ToString()} proxies.", Console.ForegroundColor = ConsoleColor.Magenta);
            Thread.Sleep(1000);
            Console.Clear();

            Directory.CreateDirectory(Environment.CurrentDirectory + @"\\Results\\");

            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine(String.Format("{0," + Console.WindowWidth / 2 + "}", wulu));

            amount = rtb.Lines.Count();

            Console.ForegroundColor = ConsoleColor.Blue;

            Thread thread = new Thread(loop);
            thread.Start();         
        }

        public static void loop()
        {
            ParallelLoopResult parallel = Parallel.ForEach(rtb.Lines, s =>
            {
                new ParallelOptions
                {
                    MaxDegreeOfParallelism = threads
                };

                cheked += 1;

                string noport = s.ToString().Split(':')[0];

                if (Check(noport))
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"[+] {s}");
                    alive += 1;
                    writeFile(s);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"[-] {s}");
                    dead += 1;
                }
                remaining = (amount - cheked);
                Console.Title = "Proxy Checker by wulu#0827 | Remaining: " + remaining.ToString() + "/" + amount + " | DEAD: " + dead.ToString() + " | ALIVE: " + alive + " | ERRORS: "+ error.ToString();

            });

            if (parallel.IsCompleted)
            {
                Console.WriteLine("[!] Finished checking proxies. Saved.", Console.ForegroundColor = ConsoleColor.Blue);

                Console.Beep();
                Thread.Sleep(500);

                Console.WriteLine("Press any key to exit.");
                Console.ReadLine();
            }
        }
    }
}
