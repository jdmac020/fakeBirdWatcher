using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;

namespace FakeBirdWatcher
{
    class Program
    {
         
        static void Main(string[] args)
        {
            var _console = new ConsoleService();

            _console.PrintIntro();
            _console.SectionBreak();

            var userName = _console.GetUserAccountName();
            var passWord = _console.GetAccountPassword();
            var targetAccount = _console.GetTargetAccount();

            _console.SectionBreak();

            var twitter = new TwitterHandler(userName, passWord, targetAccount);

            var startUrl = "https://twitter.com";
            //var userName = ConfigurationManager.AppSettings["UserName"];
            //var passWord = ConfigurationManager.AppSettings["PassWord"];

            // initialize
            //var fireFoxService = FirefoxDriverService.CreateDefaultService(@"D:\GitHub\fakeBirdWatcher\FakeBirdWatcher\FakeBirdWatcher");
            var fireFoxService = FirefoxDriverService.CreateDefaultService();
            //fireFoxService.FirefoxBinaryPath = @"C:\Program Files\Mozilla Firefox\firefox.exe";

            var driver = new FirefoxDriver(fireFoxService);

            // navigate to twitter
            driver.Navigate().GoToUrl(startUrl);

            Thread.Sleep(TimeSpan.FromSeconds(1));

            // login
            driver.FindElementByName("session[username_or_email]").SendKeys(userName);

            Thread.Sleep(TimeSpan.FromSeconds(1));

            var passWordBox = driver.FindElementByName("session[password]");

            passWordBox.SendKeys(passWord);

            Thread.Sleep(TimeSpan.FromSeconds(1));

            passWordBox.Submit();

            Thread.Sleep(TimeSpan.FromSeconds(1));

            // navigate to user's follower list
            var selectedUser = $"{startUrl}/realdonaldtrump/followers";

            driver.Navigate().GoToUrl(selectedUser);

            Thread.Sleep(TimeSpan.FromSeconds(1));

            // get group of followers

            var followerBatch = driver.FindElementsByCssSelector("[src='https://abs.twimg.com/sticky/default_profile_images/default_profile_bigger.png']");


            if (followerBatch.Count.Equals(0))
            {
                // to do--handle zero no pic profiles found
            }


            var foo = 0;

            followerBatch[0].Click();

            var canReport = false;

            Thread.Sleep(TimeSpan.FromSeconds(1));

            IWebElement protectedTimeline = null;

            try
            {
                protectedTimeline = driver.FindElementByClassName("ProtectedTimeline");
            }
            catch (NoSuchElementException)
            {

            }
            
            if (protectedTimeline is null)
            {
                var followerCount = string.Empty;

                try
                {
                    followerCount = driver.FindElementByCssSelector("[class='ProfileNav-item ProfileNav-item--followers']")
                        .FindElement(By.CssSelector("[class='ProfileNav-value']")).GetAttribute("textContent");
                }
                catch (NoSuchElementException)
                {


                }

                if (!string.IsNullOrEmpty(followerCount))
                {
                    var followerNumber = int.Parse(followerCount);

                    if (followerNumber < 5)
                    {
                        canReport = true;
                    }
                }
                else
                {
                    canReport = true;
                }

                IWebElement emptyTimelineMessage = null;

                try
                {
                    emptyTimelineMessage = driver.FindElementByClassName("ProfilePage-emptyModule");
                }
                catch (NoSuchElementException)
                {
                    
                }

                if (emptyTimelineMessage is null)
                {
                    // check # of tweets

                    var tweets = driver.FindElementsByCssSelector("[class*='tweet js-stream-tweet']");

                    if (tweets.Count > 1)
                    {
                        canReport = false;
                    }
                }
                else
                {
                    canReport = true;
                }

                foo++;
            }
            
            if (canReport)
            {
                var optionsMenu = driver.FindElementByCssSelector("[class='user-dropdown dropdown-toggle js-dropdown-toggle js-link js-tooltip btn plain-btn']");

                optionsMenu.Click();

                var buttons = driver.FindElementsByClassName("dropdown-link");

                foreach (var button in buttons)
                {
                    var text = button.Text;

                    if (text.Contains("Report"))
                    {
                        button.Click();

                        driver.SwitchTo().Frame("new-report-flow-frame");

                        driver.FindElementById("spam-btn").Click();

                        driver.SwitchTo().ParentFrame();

                        var nextButton = driver.FindElementById("report-flow-button-next");

                        nextButton.Click();

                        Thread.Sleep(500);

                        nextButton.Click();

                        driver.SwitchTo().Frame("new-report-flow-frame");

                        driver.FindElementById("block-btn").Click();
                        
                    }

                    
                }
            }

            driver.Navigate().GoToUrl(selectedUser);


        }
    }
}
