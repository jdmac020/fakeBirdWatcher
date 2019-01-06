using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FakeBirdWatcher
{
    public class BirdWatcher
    {
        private ConsoleService _console = new ConsoleService();
        private TwitterHandler _twitter;
        
        public void Watch()
        {
            InitializeRun();

            try
            {
                Login();
                GetFollowers();
            }
            catch (Exception e)
            {
                _console.ErrorHandler(e.Message);
            }
            finally
            {
                _twitter.Quit();
                _console.ExitApp();
            }
        }

        private void InitializeRun()
        {
            _console.PrintIntro();
            _console.SectionBreak();

            _console.DisplayMessage("Your Info, Please...");

            var userName = _console.GetUserAccountName();
            var passWord = _console.GetAccountPassword();
            var targetAccount = _console.GetTargetAccount();

            _console.SectionBreak();
            _console.DisplayMessage("Starting Firefox...");

            _twitter = new TwitterHandler(userName, passWord, targetAccount);
            
        }

        private void Login()
        {
            _console.DisplayMessage("Logging Into Twitter...");

            _twitter.Login();
        }

        private void GetFollowers()
        {
            _console.SectionBreak();
            _console.DisplayMessage("Accessing Follower List...");

            _twitter.AccessTargetFollowers();
        }
    }
}
