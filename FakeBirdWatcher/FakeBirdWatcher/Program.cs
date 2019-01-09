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
            var watcher = new BirdWatcher();

            watcher.Watch();

            Environment.Exit(0);
        }
    }
}
