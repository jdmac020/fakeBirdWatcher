using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FakeBirdWatcher
{
    public class TwitterHandler
    {
        private FirefoxDriver _driver;

        private string _baseUrl = "https://twitter.com/";
        private string _userName;
        private string _password;
        private string _targetAccountName;

        public TwitterHandler(string userName, string password, string targetUsername)
        {
            _userName = userName;
            _password = password;
            _targetAccountName = targetUsername;

            _driver = InitializeDriver();
        }

        public void Quit()
        {
            _driver.Quit();
        }

        public void Login()
        {

            _driver.Navigate().GoToUrl(_baseUrl);

            Pause();

            // login
            _driver.FindElementByName("session[username_or_email]").SendKeys(_userName);

            Pause();

            var passWordBox = _driver.FindElementByName("session[password]");

            passWordBox.SendKeys(_password);

            Pause();

            passWordBox.Submit();

            Pause();
            Pause();

            var thing = _driver.Url;

            if (!_driver.Url.Equals(_baseUrl))
            {
                _driver.Quit();
                throw new TwitterException($"Your Login Did Not Work! Please Check that Password [{_password}] goes with Account [{_userName}] and try again!");
            }
                
        }

        private bool GetUserOptions()
        {
            var optionsMenu = _driver.FindElementByCssSelector("[class='user-dropdown dropdown-toggle js-dropdown-toggle js-link js-tooltip btn plain-btn']");

            optionsMenu.Click();

            return true;
        }

        public string ReportFakeAccount()
        {
            var optionsAvail = false;
            var attempts = 0;

            while (optionsAvail.Equals(false) && attempts < 5)
            {
                try
                {
                    optionsAvail = GetUserOptions();
                }
                catch (Exception)
                {
                    _driver.Navigate().Refresh();
                }
            }

            if (optionsAvail.Equals(false))
                throw new TwitterException("Twitter Obscured The Context Menu");
            
            var buttons = _driver.FindElementsByClassName("dropdown-link");

            foreach (var button in buttons)
            {
                var text = button.Text;

                if (text.Contains("Report"))
                {
                    button.Click();

                    _driver.SwitchTo().Frame("new-report-flow-frame");

                    Pause();

                    _driver.FindElementById("spam-btn").Click();

                    Pause();

                    _driver.SwitchTo().ParentFrame();

                    Pause();

                    var nextButton = _driver.FindElementById("report-flow-button-next");

                    nextButton.Click();

                    Pause();

                    nextButton.Click();

                    _driver.SwitchTo().Frame("new-report-flow-frame");

                    Pause();

                    _driver.FindElementById("block-btn").Click();

                    Pause();

                    _driver.SwitchTo().ParentFrame();

                    var userName = GetNameOfAccount(); //_driver.FindElementByCssSelector("class='username u-dir u-textTruncate'").GetAttribute("textContent");

                    return $"{userName} Blocked And Reported Successfully!";
                }
            }

            return "No One Was Blocked Or Reported...?";
        }

        public string GetNameOfAccount()
        {
            var userName = _driver.FindElementByCssSelector("[class='u-linkComplex-target']").GetAttribute("textContent");
            
            return $"@{userName}";// _driver.FindElementByClassName("u-linkComplex-target").GetAttribute("textContent");
        }

        public int GetFollowerCount()
        {
            var followerCount = string.Empty;

            try
            {
                followerCount = _driver.FindElementByCssSelector("[class='ProfileNav-item ProfileNav-item--followers']")
                    .FindElement(By.CssSelector("[class='ProfileNav-value']")).GetAttribute("textContent");
            }
            catch (NoSuchElementException) { }
            
            if (!string.IsNullOrEmpty(followerCount))
            {
                var followerNumber = int.Parse(followerCount);

                return followerNumber;
            }

            return 0;
        }

        public void NavigateToAccount(IWebElement birdToIdentify)
        {
            birdToIdentify.Click();
            Pause();
        }

        private bool IsProtectedAccount()
        {
            IWebElement protectedTimeline = null;

            try
            {
                protectedTimeline = _driver.FindElementByClassName("ProtectedTimeline");
            }
            catch (NoSuchElementException) { }

            if (protectedTimeline is null)
                return false;

            return true;
        }

        private bool IsEmptyTimeline()
        {
            IWebElement emptyTimelineMessage = null;

            try
            {
                emptyTimelineMessage = _driver.FindElementByClassName("ProfilePage-emptyModule");
            }
            catch (NoSuchElementException) { }

            if (emptyTimelineMessage is null)
                return false;

            return true;
        }

        public int GetTweetCount()
        {
            if (IsProtectedAccount())
                return -1;

            if (IsEmptyTimeline())
                return 0;

            return _driver.FindElementsByCssSelector("[class*='tweet js-stream-tweet']").Count;
        }

        public void AccessTargetFollowers()
        {
            var targetFollowerUrl = $"{_baseUrl}{_targetAccountName}/followers";

            _driver.Navigate().GoToUrl(targetFollowerUrl);

            Pause();
        }

        public IWebElement GetNoPicFollower()
        {
            IReadOnlyCollection<IWebElement> followerBatch = null;

            //var items = _driver.FindElementsByCssSelector("[class='ProfileCard-avatarLink js-nav js-tooltip']").Where(item => item.GetAttribute("src") == "https://abs.twimg.com/sticky/default_profile_images/default_profile_bigger.png");

            followerBatch = _driver.FindElementsByCssSelector("[src='https://abs.twimg.com/sticky/default_profile_images/default_profile_bigger.png']");

            return followerBatch.First();
        }

        private void Pause()
        {
            Thread.Sleep(TimeSpan.FromSeconds(1));
        }

        private FirefoxDriver InitializeDriver()
        {
            var driverService = FirefoxDriverService.CreateDefaultService();

            return new FirefoxDriver(driverService);
        }
    }
}
