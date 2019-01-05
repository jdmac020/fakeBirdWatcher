using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium.Firefox;

namespace FakeBirdWatcher
{
    class Program
    {
        static void Main(string[] args)
        {
            var startUrl = "https://twitter.com";
            var userName = ConfigurationManager.AppSettings["UserName"];
            var passWord = ConfigurationManager.AppSettings["PassWord"];

            // initialize
            var fireFoxService = FirefoxDriverService.CreateDefaultService(@"D:\GitHub\fakeBirdWatcher\FakeBirdWatcher\FakeBirdWatcher");
            fireFoxService.FirefoxBinaryPath = @"C:\Program Files\Mozilla Firefox\firefox.exe";

            var driver = new FirefoxDriver(fireFoxService);

            // navigate to twitter
            driver.Navigate().GoToUrl(startUrl);

            // login
            driver.FindElementByName("session[username_or_email]").SendKeys(userName);

            var passWordBox = driver.FindElementByName("session[password]");

            passWordBox.SendKeys(passWord);

            passWordBox.Submit();

            
        }
    }
}
