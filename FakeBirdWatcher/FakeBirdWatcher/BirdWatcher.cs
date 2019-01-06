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
        
        public void Watch()
        {
            var count = 0;
            var reported = 0;
            
            try
            {
                InitializeRun();
                Login();

                while (count < _attemptsToMake)
                {
                    var birdOfInterest = GetBirdToIdentify();

                    var isFake = IdentifyFakeBird(birdOfInterest);

                    if (isFake)
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
                    else
                    {
                        _console.DisplayMessage("Appears to be real!");
                    }

                    count++;
                }

                _console.SectionBreak();
                _console.DisplayMessage($"End Of Run! {reported} Accounts Were Reported!");
                
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

        private void ReportAsFake()
        {
            _console.DisplayMessage("Reporting Account As Fake...");

            var result = _twitter.ReportFakeAccount();

            _console.DisplayMessage(result);
        }

        private bool IdentifyFakeBird(IWebElement birdToIdentify)
        {
            _twitter.NavigateToAccount(birdToIdentify);

            var userName = _twitter.GetNameOfAccount();

            _console.DisplayMessage($"Identifying bird [{userName}]...");

            var meetsLowFollowerCriteria = LessThanFiveFollowers();
            _console.DisplayMessage($"Less than 5 followers: [{meetsLowFollowerCriteria}]!");

            var meetsLowTweetCriteria = OneOrZeroTweets();
            _console.DisplayMessage($"1 or Zero Tweets: [{meetsLowTweetCriteria}]!");

            return (meetsLowFollowerCriteria && meetsLowTweetCriteria);
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
