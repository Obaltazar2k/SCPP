using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCPP.Utilities
{
    class Period
    {
        public static string GetPeriod()
        {
            DateTime _thisDay = DateTime.Today;
            var year = _thisDay.Year;
            var month = _thisDay.Month;
            string startMonth;
            string endtMonth;
            string startYear;
            string endYear;
            if (month < 7)
            {
                startMonth = "FEB";
                startYear = year.ToString();
                endtMonth = "JUL";
                endYear = startYear;
            }
            else
            {
                startMonth = "AGO";
                startYear = year.ToString();
                endtMonth = "ENE";
                endYear = _thisDay.AddYears(1).ToString();
            }
            return startMonth + startYear + "-" + endtMonth + endYear;
        }
    }
}
