using System;
using System.Collections.Generic;
using System.Text;

namespace sf.utils
{
    public static class DateUtils
    {
        public static DateTime ParseDate(this string strDate, DateTime defaultDate)
        {
            DateTime resultDate = default;

            if (!DateTime.TryParse(strDate, out resultDate))
                resultDate = defaultDate;
            
            return resultDate;
        }
    }
}