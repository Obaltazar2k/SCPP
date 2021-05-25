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

        public static string CaclulateMonth(int month)
        {
            string stringMonth = "";
            switch (month)
            {
                case 1:
                    stringMonth = "enero";
                    break;

                case 2:
                    stringMonth = "febrero";
                    break;

                case 3:
                    stringMonth = "mazo";
                    break;

                case 4:
                    stringMonth = "abril";
                    break;

                case 5:
                    stringMonth = "mayo";
                    break;

                case 6:
                    stringMonth = "junio";
                    break;

                case 7:
                    stringMonth = "julio";
                    break;

                case 8:
                    stringMonth = "agosto";
                    break;

                case 9:
                    stringMonth = "septiembre";
                    break;

                case 10:
                    stringMonth = "octubre";
                    break;

                case 11:
                    stringMonth = "noviembre";
                    break;

                case 12:
                    stringMonth = "diciembre";
                    break;
            }

            return stringMonth;
        }
    }
}
