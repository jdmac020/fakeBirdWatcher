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

            if (!_driver.Url.Equals(_baseUrl))
            {
                _driver.Quit();
                throw new TwitterException($"Your Login Did Not Work! Please Check that Password [{_password}] goes with Account [{_userName}] and try again!");
            }
                
        }

        public void AccessTargetFollowers()
        {
            var targetFollowerUrl = $"{_baseUrl}/{_targetAccountName}/followers";

            _driver.Navigate().GoToUrl(targetFollowerUrl);

            Pause();
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
