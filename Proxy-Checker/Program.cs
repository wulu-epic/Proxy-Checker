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

        public static int remaining;
        public static int dead;
        public static int alive;
        public static int cheked;

        public static bool Check(string proxy)
        {
            try
            {
                Ping ping = new Ping();
                var sent = ping.Send(proxy);
                if (sent.Status == IPStatus.Success)
                {
                    working.Add(proxy);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            finally
            {
                
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

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("Open a proxy file");

            Thread.Sleep(500);

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Text files (*.txt)|*.txt";
            ofd.Title = "Proxies";
            RichTextBox rtb = new RichTextBox();

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                rtb.Text = File.ReadAllText(ofd.FileName);
            }

            Console.Clear();

            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine(String.Format("{0," + Console.WindowWidth / 2 + "}", wulu));

            int amount = rtb.Lines.Count();

            List<string> wo = new List<string>();

            Parallel.ForEach(rtb.Lines, s =>
            {
                cheked += 1;

                string noport = s.Split(':')[0];

                if (Check(noport))
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine(s);
                    alive += 1;
                    wo.Add(s);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(s);
                    dead += 1;
                }
                remaining = (amount - cheked);
                Console.Title = "Proxy Checker by wulu#0827 | Remaining: " + remaining.ToString() + "/" + amount + " | DEAD: " + dead.ToString() + " | ALIVE: " + alive;

            });

            Console.ForegroundColor = ConsoleColor.Blue;

            Console.WriteLine("Finished checking proxies. Save to a file");
            Console.Beep();
            Thread.Sleep(500);

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = ofd.Filter = "Text files (*.txt)|*.txt";
            sfd.Title = "Save alive proxies";
            sfd.FileName = alive.ToString();

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                rtb.Text = "";

                foreach (string s in wo.ToArray())
                {
                    rtb.AppendText(s + "\n");
                }
                rtb.SaveFile(Path.GetFullPath(sfd.FileName), RichTextBoxStreamType.PlainText);
                Console.WriteLine("Finished saving proxies.");
            }
            Console.WriteLine("Press any key to exit.");
            Console.ReadLine();
        }
    }
}
