using System;

namespace _4drafts.Services
{
    public interface ITimeWarper
    {
        public string TimeAgo(DateTime time);
        public string FullDate(DateTime time);
    }
}
