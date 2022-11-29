namespace Imato.MsSql.ExternalData.Example
{
    public class DaysProcess : Process<MonthDay>
    {
        public DaysProcess(string[] args) : base(args)
        {
        }

        protected override IEnumerable<MonthDay> CreateData(
            IDictionary<string, string> parameters)
        {
            var year = int.Parse(parameters["Year"]);
            var month = int.Parse(parameters["Month"]);
            var date = new DateTime(year, month, 1);
            var lastDate = date.AddMonths(1);
            while (date < lastDate)
            {
                yield return new MonthDay
                {
                    Date = date,
                    Id = date.Day,
                    Name = date.ToString("yyyy-MM-dd"),
                    IsDayOf = date.DayOfWeek == DayOfWeek.Saturday
                        || date.DayOfWeek == DayOfWeek.Sunday
                };
                date = date.AddDays(1);
            }
        }
    }
}