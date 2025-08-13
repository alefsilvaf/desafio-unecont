namespace UnecontScraping.Domain
{
    public class ScrapingConfig
    {
        public required string[] Categories { get; set; }
        public required PriceFilter PriceFilter { get; set; }
        public required int RatingFilter { get; set; }
    }
}