using OpenQA.Selenium.Firefox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        private FirefoxDriver InitializeDriver()
        {
            var driverService = FirefoxDriverService.CreateDefaultService();

            return new FirefoxDriver(driverService);
        }
    }
}
