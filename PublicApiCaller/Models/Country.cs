namespace PublicApiCaller.Models
{
    public class Country
    {
        public int Id { get; set; }

        public string CommonName { get; set; } = string.Empty;

        public string? OfficialName { get; set; }

        public string? Region { get; set; }

        public decimal Area { get; set; }

        public long Population { get; set; }

        public DateTime CreatedAt { get; set; }
    }

}
