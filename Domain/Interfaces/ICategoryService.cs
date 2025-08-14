namespace UnecontScraping.Domain
{
    public interface ICategoryService
    {
        Task<Dictionary<string, int>> GetCategoriesAsync();
    }
}