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
            WriteLine("***      Fake Bird Watcher      ***");
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
            WriteLine(errorMessage);
            WriteLine();
        }

        public string GetUserAccountName()
        {
            return GetInput("Enter your account username or e-mail");
        }

        public string GetAccountPassword()
        {
            return GetInput("Enter your account password");
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
            return GetInput("Enter the account you'd like to scan");
        }
    }
}
