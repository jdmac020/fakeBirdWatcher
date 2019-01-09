using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;

namespace FakeBirdWatcher
{
    public class ConsoleService
    {
        public void PrintIntro()
        {
            WriteLine("*** *** *** *** *** *** *** *** ***");
            WriteLine("***                             ***");
            WriteLine("***           Welcome           ***");
            WriteLine("***        Bird Watcher!        ***");
            WriteLine("***                             ***");
            WriteLine("*** *** *** *** *** *** *** *** ***");
            
        }

        public void DisplayMessage(string message)
        {
            WriteLine(message);
        }

        public void SectionBreak()
        {
            WriteLine();
            WriteLine();
            WriteLine();
        }

        public void ErrorHandler(string errorMessage)
        {
            WriteLine();
            WriteLine(errorMessage);
            WriteLine();
        }

        public string GetUserAccountName()
        {
            return GetInput("Enter your account username or e-mail (no @ needed)");
        }

        public string GetAccountPassword()
        {
            string pass = "";

            Write("Enter your account password: ");

            do
            {
                ConsoleKeyInfo key = ReadKey(true);
                // Backspace Should Not Work
                if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter)
                {
                    pass += key.KeyChar;
                    Write("*");
                }
                else
                {
                    if (key.Key == ConsoleKey.Backspace && pass.Length > 0)
                    {
                        pass = pass.Substring(0, (pass.Length - 1));
                        Write("\b \b");
                    }
                    else if (key.Key == ConsoleKey.Enter)
                    {
                        break;
                    }
                }
            } while (true);

            WriteLine();

            return pass;
        }

        public string GetContinue(string continueAmount)
        {
            return GetInput($"Type \"Y\" to Keep Scanning for {continueAmount} Times, Press Enter to Exit");
        }

        private string GetInput(string userPrompt)
        {
            Write($"{userPrompt}: ");

            return ReadLine();
        }

        public void ExitApp()
        {
            Write("Press any key to exit...");
            ReadLine();
        }

        public string GetTargetAccount()
        {
            return GetInput("Enter the account you'd like to scan (eg, \"realdonaldtrump\")");
        }

        public string GetRunsToMake()
        {
            return GetInput("Enter the number of scans you'd like to make (eg, \"5\")");
        }
    }
}
