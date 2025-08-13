namespace UnecontScraping.Domain
{
    public class Book
    {
        public required string Title { get; set; }
        public required decimal Price { get; set; }
        public required int Rating { get; set; }
        public required string Category { get; set; }
        public required string Url { get; set; }
    }
}