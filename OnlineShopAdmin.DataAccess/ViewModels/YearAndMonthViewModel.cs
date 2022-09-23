using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShopAdmin.DataAccess.ViewModels
{
    public class YearAndMonthViewModel
    {
        public YearAndMonthViewModel()
        {
        }

        public int Year { get; set; }
        public int Month { get; set; }
        public decimal Sum { get; set; }

        public Dictionary<int, decimal> Dictionary { get; set; }
    }
}
