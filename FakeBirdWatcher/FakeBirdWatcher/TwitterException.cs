using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FakeBirdWatcher
{
    public class TwitterException : Exception
    {
        public TwitterException(string message):base(message) { }
    }
}
