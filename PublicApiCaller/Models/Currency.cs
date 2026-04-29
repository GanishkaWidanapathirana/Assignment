namespace PublicApiCaller.Models
{
    public class Currency
    {
        public int Id { get; set; }

        public string Code { get; set; } = string.Empty;

        public string? Name { get; set; }

        public string? Symbol { get; set; }
    }
}
