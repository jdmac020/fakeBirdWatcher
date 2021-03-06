﻿using BirdWatcher;
using BirdWatcher.Dto;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BirdWatcher
{
    public class BirdWatcher
    {
        private ConsoleService _console = new ConsoleService();
        private CsvHandler _csv = new CsvHandler();
        private TwitterHandler _twitter;
        private int _attemptsToMake;
        private string _currentUser;
        private List<ScannedAccount> _scannedAccounts = new List<ScannedAccount>();
        
        public void Watch()
        {
            
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

                try
                {
                    _csv.WriteListToFile(_scannedAccounts);
                }
                catch (Exception e)
                {
                    _console.DisplayMessage($"Encountered a problem logging results: {e.Message}");
                }
                
                _console.DisplayMessage($"End Of Run! {_scannedAccounts.Count} Accounts Were Checked and {_scannedAccounts.Where(sa => sa.ReportedAndBlocked).Count()} Accounts Were Reported This Session!");
                
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
        }

        private void ReportAsFake()
        {
            _console.DisplayMessage("Reporting Account As Fake...");

            var result = string.Empty;

            try
            {
                result = _twitter.ReportFakeAccount();

                var account = _scannedAccounts.Last();
                account.ReportedAndBlocked = true;
                account.ReportedTimeStamp = DateTime.Now.ToString("yyyyMMdd HH:mm:ss");
            }
            catch (Exception e)
            {
                result = $"Something Went Wrong While Reporting or Blocking: {e.Message} -- Will Attempt to Continue Scanning...";
            } 

            _console.DisplayMessage(result);
        }

        private int IdentifyFakeBird(IWebElement birdToIdentify)
        {
            var account = new ScannedAccount();
            _scannedAccounts.Add(account);
            var returnValue = 0;

            _twitter.NavigateToAccount(birdToIdentify);

            account.UserName = _twitter.GetNameOfAccount();

            if (account.UserName.Equals(_currentUser))
            {
                _console.DisplayMessage($"Whoops, just checked {account.UserName}...");
                _scannedAccounts.Remove(account);
                return -1;
            }
            
            _currentUser = account.UserName;

            // check for suspended account

            _console.DisplayMessage($"Identifying bird [{account.UserName}]...");

            account.MeetsFakeFollowerCount = LessThanFiveFollowers();
            _console.DisplayMessage($"Less than 5 followers: [{account.MeetsFakeFollowerCount}]!");

            account.MeetsFakeTweetCount = OneOrZeroTweets();
            _console.DisplayMessage($"1 or Zero Tweets: [{account.MeetsFakeTweetCount}]!");

            if (account.MeetsFakeFollowerCount && account.MeetsFakeTweetCount)
            {
                returnValue = 1;
                account.DeterminedFake = true;
            }

            account.ScannedTimeStamp = DateTime.Now.ToString("yyyyMMdd HH:mm:ss");
            
            return returnValue;
        }

        private bool LessThanFiveFollowers()
        {
            var account = _scannedAccounts.Last();

            account.FollowerCount = _twitter.GetFollowerCount();

            return account.FollowerCount < 5;
        }

        private bool OneOrZeroTweets()
        {
            var account = _scannedAccounts.Last();

            account.TweetCount = _twitter.GetTweetCount();

            switch (account.TweetCount)
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
            var protectedAccountInput = _console.GetPrivateAccount();
            var targetAccount = _console.GetTargetAccount();
            var numberOfRuns = _console.GetRunsToMake();

            _console.SectionBreak();
            _console.DisplayMessage("Starting Firefox...");

            var protectedAccountBool = !string.IsNullOrEmpty(protectedAccountInput);
            _attemptsToMake = int.Parse(numberOfRuns);
            _twitter = new TwitterHandler(userName, passWord, protectedAccountBool, targetAccount);
            
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
