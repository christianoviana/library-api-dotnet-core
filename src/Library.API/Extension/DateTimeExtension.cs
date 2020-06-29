using System;

namespace Library.API.Extension
{
    public static class DateTimeExtension
    {
        public static int GetAge(this DateTime? date)
        {
            if (date.HasValue)
            {
                DateTime currentYear = DateTime.Now.Date;
                DateTime birthday = date.Value;

                int age = currentYear.Date.Year - birthday.Date.Year;

                if (birthday.Date >= currentYear.Date)
                    --age;

                return age;
            }

            return 0;       
        }

        public static int GetAge(this DateTime date)
        {
            if (date == null || date == DateTime.MinValue)
            {
                return 0;
            }

            DateTime currentYear = DateTime.Now.Date;
            DateTime birthday = date;

            int age = currentYear.Date.Year - birthday.Date.Year;

            if (birthday.Date >= currentYear.Date)
                --age;

            return age;
        }
    }
}
