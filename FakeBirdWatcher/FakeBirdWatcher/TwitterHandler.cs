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

        private string _baseUrl = "https://twitter.com";
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

        public void Login()
        {

            _driver.Navigate().GoToUrl(_baseUrl);

            Thread.Sleep(TimeSpan.FromSeconds(1));

            // login
            _driver.FindElementByName("session[username_or_email]").SendKeys(_userName);

            Thread.Sleep(TimeSpan.FromSeconds(1));

            var passWordBox = _driver.FindElementByName("session[password]");

            passWordBox.SendKeys(_password);

            Thread.Sleep(TimeSpan.FromSeconds(1));

            passWordBox.Submit();

            Thread.Sleep(TimeSpan.FromSeconds(1));

            if (!_driver.Url.Equals(_baseUrl))
            {
                _driver.Quit();
                throw new TwitterException($"Your Login Did Not Work! Please Check that Password [{_password}] goes with Account [{_userName}] and try again!");
            }
                
        }

        private FirefoxDriver InitializeDriver()
        {
            var driverService = FirefoxDriverService.CreateDefaultService();

            return new FirefoxDriver(driverService);
        }
    }
}
