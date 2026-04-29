namespace PublicApiCaller.Models
{
    public class CountryNativeName
    {
        public int Id { get; set; }

        public int CountryId { get; set; }

        public string? LanguageCode { get; set; }

        public string? OfficialName { get; set; }

        public string? CommonName { get; set; }
    }
}
