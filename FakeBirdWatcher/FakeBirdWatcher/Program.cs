﻿using System;
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
            var startUrl = "https://twitter.com";
            var userName = ConfigurationManager.AppSettings["UserName"];
            var passWord = ConfigurationManager.AppSettings["PassWord"];

            // initialize
            var fireFoxService = FirefoxDriverService.CreateDefaultService(@"D:\GitHub\fakeBirdWatcher\FakeBirdWatcher\FakeBirdWatcher");
            fireFoxService.FirefoxBinaryPath = @"C:\Program Files\Mozilla Firefox\firefox.exe";

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

                        //done??

                        driver.SwitchTo().Frame("new-report-flow-frame");

                        //var blockButton = driver.FindElementById("block-btn");

                        //foo++;

                        //blockButton.Click();

                        //foo++;



                        //var nextBut = driver.FindElementById("report-dialog").FindElement(By.Id("report-flow-button-next"));

                        //foo++;

                        //var reportWindow = driver.FindElementByClassName("modal-content");//.FindElement(By.Id("new-report-flow-frame"));

                        //var handles = driver.WindowHandles;

                        ////var active = driver.SwitchTo();//.ActiveElement();

                        //foo++;
                        
                        ////var form = reportWindow.

                        ////var form = active.FindElement(By.Id("report_webview_form"));
                        
                        //var dialog = driver.FindElementById("report-dialog");

                        ////dialog.SendKeys(Keys.Tab);

                        //try
                        //{
                            
                        //}
                        //catch (Exception e)
                        //{
                            
                        //}


                        //foo++; //var frame = dialog.FindElement(By.Id("new-report-flow-frame"));

                        //try
                        //{
                        //    var but = driver.FindElementById("spam-btn");

                        //    foo++;

                        //    but.Click();

                        //    foo++;
                            
                        //    var nextBut = driver.FindElementById("report-dialog").FindElement(By.Id("report-flow-button-next"));

                        //    foo++;


                        //}
                        //catch (Exception e)
                        //{

                        //    //throw;
                        //}

                        //try
                        //{
                        //    var nextBut2 = driver.FindElementById("modal-content").FindElement(By.Id("report-flow-button-next"));
                        //}
                        //catch (Exception e)
                        //{
                            
                        //}

                        //try
                        //{
                        //    driver.SwitchTo().ParentFrame();
                        //}
                        //catch (Exception e)
                        //{

                        //    throw;
                        //}

                        //try
                        //{
                        //    var nextButt = driver.FindElementById("report-flow-button-next");
                            
                        //    nextButt.Click();

                        //    Thread.Sleep(500);

                        //    nextButt.Click();

                        //    driver.SwitchTo().Frame("new-report-flow-frame");

                        //    var blockButton = driver.FindElementById("block-btn");

                        //    foo++;

                        //    blockButton.Click();

                        //    foo++;
                        //}
                        //catch (Exception e)
                        //{
                            
                        //}
                        
                    }

                    
                }
            }

            driver.Navigate().GoToUrl(selectedUser);


        }
    }
}
