using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BirdWatcher.Dto
{
    public class ScannedAccount
    {
        public string UserName { get; set; }
        public int FollowerCount { get; set; }
        public int TweetCount { get; set; }
        public bool MeetsFakeTweetCount { get; set; }
        public bool MeetsFakeFollowerCount { get; set; }
        public bool DeterminedFake { get; set; }
        public bool ReportedAndBlocked { get; set; }
        public DateTime? ScannedTimeStamp { get; set; }
    }
}
