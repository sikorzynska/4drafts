using _4drafts.Models.Threads;
using _4drafts.Models.Users;
using System.Collections.Generic;

namespace _4drafts.Models.Shared
{
    public class StatisticsViewModel
    {
        public List<ThreadsBrowseModel> Threads = new List<ThreadsBrowseModel>();
        public List<UserViewModel> Users = new List<UserViewModel>();
        public List<string> Rules = new List<string>();
    }
}
