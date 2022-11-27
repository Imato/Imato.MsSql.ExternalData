namespace Imato.MsSql.ExternalData.Example
{
    public class MonthDay
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public DateTime Date { get; set; }
        public bool IsDayOf { get; set; }
    }
}