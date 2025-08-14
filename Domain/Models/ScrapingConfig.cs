namespace UnecontScraping.Domain
{
    public class ScrapingConfig
    {
        public required string[] Categories { get; set; }
        public PriceFilter? PriceFilter { get; set; }
        public int? RatingFilter { get; set; }
    }
}