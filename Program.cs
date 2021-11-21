using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ProjectTheta
{
    internal static class String
    {
        // https://stackoverflow.com/a/1120277
        public static string RemoveSpecialCharacters(this string str)
        {
            StringBuilder sb = new();
            foreach (char c in str)
            {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == ' ')
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }
    }

    class Program
    {
        public static List<string> WordBank;
        public static List<string> Transcript;

        static DateTime SessionStartTime;

        static void Main()
        {
            Console.WriteLine("\aProject Theta | Meowcat McMeow XVIII (C) 2021 - 2022\n");
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(ExitHandler);
            WordBank = new();
            Transcript = new();
            SessionStartTime = DateTime.Now;

            Load();

            Theta();

            Save();

            return;
        }

        static void Load()
        {
            if (!File.Exists(@".\word_bank.txt"))
            {
                return;
            }

            foreach (string Word in File.ReadAllText(@".\word_bank.txt").ToString().Split(' '))
            {
                WordBank.Add(Word.ToLowerInvariant());
            }
            return;
        }

        static void Theta()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Theta is online!");
            Console.ResetColor();
            Console.WriteLine("Type \"/?\" for a list of commands.\n");

            bool Exit = false;
            string Input;
            Random RNG = new();
            while (!Exit)
            {
                // User's turn
                Input = Console.ReadLine();
                
                if (Input.StartsWith('/'))
                {
                    switch (Input)
                    {
                        default:
                            Console.WriteLine("\n(Command unrecognized. Type \"/?\" for a list of commands.)\n");
                            break;

                        case "/?" or "/help":
                            Console.WriteLine("\n[Chat commands]\n" +
                                              "/reset\t\t| Makes Theta forget all the words they learned.\n" +
                                              "/save\t\t| Saves Theta's word bank to a file in the install directory.\n" +
                                              "/transcript\t| Exports a transcript of the current chat session.\n");
                            break;

                        case "/exit":
                            Console.WriteLine("\n(This command is currently broken. Use [Ctrl]+[C] instead.)\n");
                            break;

                        case "/reset":
                            WordBank = new();
                            File.Delete(@".\word_bank.txt");
                            Console.WriteLine("\n(Theta's word bank has been reset.)\n");
                            break;

                        case "/save":
                            Save();
                            Console.WriteLine("\n(Theta's word bank has been saved.)\n");
                            break;

                        case "/transcript":
                            Console.WriteLine("\n(Preparing and exporting transcript...)");
                            ExportTranscript();
                            Console.WriteLine("(Transcript has been exported to your desktop.)\n");
                            break;
                    }
                }
                else
                {
                    Transcript.Add("[User]  " + Input);
                    foreach (string Word in Input.ToLowerInvariant().RemoveSpecialCharacters().Trim().Split(' '))
                    {
                        WordBank.Add(Word);
                    }

                    // Theta's turn
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write('\n');

                    int WordCount = RNG.Next(0, 16);

                    string Output = string.Empty;

                    for (int i = 0; i < WordCount; i++)
                    {
                        Output += (WordBank[RNG.Next(0, WordBank.Count)] + ' ');
                    }

                    Transcript.Add("[Theta] " + Output + '.');
                    Console.WriteLine($"{Output}.\n");
                    Console.ResetColor();
                }
            }
        }

        static void Save()
        {
            string TempFile = Path.GetTempFileName();
            File.WriteAllText(TempFile, string.Join(' ', WordBank.ToArray().Distinct()));
            if (File.Exists(@".\word_bank.txt"))
            {
                File.Delete(@".\word_bank.txt");
            }
            File.Move(TempFile, @".\word_bank.txt");
            return;
        }

        static void ExportTranscript()
        {
            DateTime TimeOfRequest = DateTime.Now;
            string Target = Environment.GetEnvironmentVariable("UserProfile") + @"\Desktop\theta_transcript.log";
            string Temp = Path.GetTempFileName();

            using StreamWriter LogWriter = new(File.OpenWrite(Temp));
            LogWriter.WriteLine("Project Theta - Conversation Transcript\n");
            LogWriter.WriteLine($"[Session Start | {SessionStartTime.ToShortDateString() + ' ' + SessionStartTime.ToShortTimeString()}]\n");
            for (int i = 0; i < Transcript.Count; i++)
            {
                LogWriter.WriteLine($"{Transcript[i]}\n");
            }
            LogWriter.WriteLine($"\n[End of Transcript | File requested at {TimeOfRequest.ToShortTimeString()}]\n");
            LogWriter.WriteLine("Project Theta | Meowcat McMeow XVIII (C) 2021 - 2022");
            LogWriter.Flush();
            LogWriter.Close();
            LogWriter.Dispose();

            if (File.Exists(Target))
            {
                File.Delete(Target);
            }

            File.Move(Temp, Target);
        }

        static void ExitHandler(object Sender, EventArgs EA)
        {
            Save();
            Environment.Exit(0);
        }
    }
}
