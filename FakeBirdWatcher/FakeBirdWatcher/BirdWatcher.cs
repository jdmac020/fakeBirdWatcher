using OpenQA.Selenium;
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
        private int _attemptsToMake;
        private int _count;
        private int _reported;
        private string _currentUser;
        
        public void Watch()
        {
            _count = 0;
            _reported = 0;
            
            try
            {
                InitializeRun();
                Login();

                var canRun = true;

                while (canRun)
                {
                    RunLoop();

                    canRun = CheckContinue();
                }
                
                _console.SectionBreak();
                _console.DisplayMessage($"End Of Run! {_count} Accounts Were Checked and {_reported} Accounts Were Reported This Session!");
                
            }
            catch (Exception e)
            {
                _console.ErrorHandler(e.Message);
            }
            finally
            {
                if (_twitter != null)
                    _twitter.Quit();

                _console.ExitApp();
            }
        }

        private bool CheckContinue()
        {
            _console.SectionBreak();

            var continueInput = _console.GetContinue(_attemptsToMake.ToString());

            if (string.IsNullOrEmpty(continueInput))
                return false;

            return true;
        }

        private void RunLoop()
        {
            var count = 0;
            var reported = 0;

            while (count < _attemptsToMake)
            {
                var birdOfInterest = GetBirdToIdentify();

                var isFake = IdentifyFakeBird(birdOfInterest);

                if (isFake.Equals(1))
                {
                    _console.DisplayMessage("This Account Has No Picture, 0-1 Tweets, and Less Than Five Followers. #fakeAccount");

                    try
                    {
                        ReportAsFake();
                    }
                    catch (TwitterException e)
                    {
                        _console.DisplayMessage($"Problem with Reporting: [{e.Message}]");
                    }

                    reported++;
                }
                else if (isFake.Equals(0))
                {
                    _console.DisplayMessage("Appears to be real!");
                }
                else
                {
                    count--;
                    _console.DisplayMessage("Skipping a previously checked account!");
                }

                count++;
            }

            _count += count;
            _reported += reported;
        }

        private void ReportAsFake()
        {
            _console.DisplayMessage("Reporting Account As Fake...");

            var result = _twitter.ReportFakeAccount();

            _console.DisplayMessage(result);
        }

        private int IdentifyFakeBird(IWebElement birdToIdentify)
        {
            _twitter.NavigateToAccount(birdToIdentify);

            var userName = _twitter.GetNameOfAccount();

            if (userName.Equals(_currentUser))
            {
                _console.DisplayMessage("Whoops, just checked this one...");
                return -1;
            }

            _currentUser = userName;

            _console.DisplayMessage($"Identifying bird [{userName}]...");

            var meetsLowFollowerCriteria = LessThanFiveFollowers();
            _console.DisplayMessage($"Less than 5 followers: [{meetsLowFollowerCriteria}]!");

            var meetsLowTweetCriteria = OneOrZeroTweets();
            _console.DisplayMessage($"1 or Zero Tweets: [{meetsLowTweetCriteria}]!");

            if (meetsLowFollowerCriteria && meetsLowTweetCriteria)
                return 1;

            return 0;
        }

        private bool LessThanFiveFollowers()
        {
            var followerCount = _twitter.GetFollowerCount();

            return followerCount < 5;
        }

        private bool OneOrZeroTweets()
        {
            var tweetCount = _twitter.GetTweetCount();

            switch (tweetCount)
            {
                case -1:
                    return false;

                case 1:
                    return true;

                case 0:
                    return true;

                default:
                    return false;
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
            var numberOfRuns = _console.GetRunsToMake();

            _console.SectionBreak();
            _console.DisplayMessage("Starting Firefox...");

            _attemptsToMake = int.Parse(numberOfRuns);
            _twitter = new TwitterHandler(userName, passWord, targetAccount);
            
        }

        private void Login()
        {
            _console.DisplayMessage("Logging Into Twitter...");

            _twitter.Login();
        }

        private IWebElement GetBirdToIdentify()
        {
            _console.SectionBreak();
            _console.DisplayMessage("Accessing Follower List...");

            _twitter.AccessTargetFollowers();

            _console.DisplayMessage("Getting Followers With No Pictures...");

            var noPicBird = _twitter.GetNoPicFollower();

            return noPicBird;
        }


    }
}
