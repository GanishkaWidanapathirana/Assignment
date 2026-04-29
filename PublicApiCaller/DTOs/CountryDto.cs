namespace PublicApiCaller.DTOs
{
    public class CountryDto
    {
        public int Id { get; set; }

        public string CommonName { get; set; } = string.Empty;

        public string? OfficialName { get; set; }

        public string? Region { get; set; }

        public decimal Area { get; set; }

        public long Population { get; set; }

        public List<string> Capitals { get; set; } = new();

        public List<string> Languages { get; set; } = new();

        public List<string> Currencies { get; set; } = new();
    }
}
