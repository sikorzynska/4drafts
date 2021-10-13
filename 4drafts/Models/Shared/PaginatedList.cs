using _4drafts.Models.Genres;
using System;
using System.Collections.Generic;
using System.Linq;

namespace _4drafts.Models.Shared
{
    public class PaginatedList<T> : List<T>
    {
        public int PageIndex { get; private set; }
        public int TotalPages { get; set; }

        public IEnumerable<GenresBrowseModel> Genres { get; set; }

        public int Genre { get; set; }
        public string SortType { get; set; }
        public int Type { get; set; }

        public string ThreadSwitch { get; set; }


        public PaginatedList(List<T> items, 
            int count, int pageIndex, 
            int pageSize, 
            IEnumerable<GenresBrowseModel> genres, 
            int genre = 0, 
            string sort = null, 
            int type = 0,
            string threadSwitch = "mine")
        {
            PageIndex = pageIndex;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            Genres = genres;
            Genre = genre;
            SortType = sort;
            Type = type;
            ThreadSwitch = threadSwitch;
            this.AddRange(items);
        }

        public bool PreviousPage
        {
            get
            {
                return (PageIndex > 1);
            }
        }

        public bool NextPage
        {
            get
            {
                return (PageIndex < TotalPages);
            }
        }

        public static PaginatedList<T> Create(List<T> source, 
            int pageIndex, 
            int pageSize, 
            IEnumerable<GenresBrowseModel> genres, 
            int genre = 0, 
            string sort = null,
            int type = 0,
            string threadSwitch = "mine")
        {
            var count = source.Count();
            var items = source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            return new PaginatedList<T>(items, count, pageIndex, pageSize, genres, genre, sort, type, threadSwitch);
        }
    }
}
