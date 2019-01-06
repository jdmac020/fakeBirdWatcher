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
    }
}
