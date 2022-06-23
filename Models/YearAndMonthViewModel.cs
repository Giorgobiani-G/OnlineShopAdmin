using System.Collections.Generic;

namespace OnlineShopAdmin.Models
{
    public class YearAndMonthViewModel
    {
        public YearAndMonthViewModel()
        {
        }

        public int Year { get; set; }
        public int Month { get; set; }
        public decimal Sum { get; set; }

        public Dictionary<int,decimal> Dictionary { get; set; }
    }
}
