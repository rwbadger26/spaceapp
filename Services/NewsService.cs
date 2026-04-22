using System.Net.Http.Json;
using spaceapp.Models;

namespace spaceapp.Services
{
    public class NewsService
    {
        private const string NasaApiKey = "hg3WcIJn1F0lsTC1MM4GO3ViIg0b3cjDMKDKNQ0Z";
        private const string ApodEndpoint = "https://api.nasa.gov/planetary/apod";
        private const int MaxArticles = 5;

        public async Task<List<NewsArticle>> GetNewsArticlesAsync()
        {
            try
            {
                using var httpClient = new HttpClient
                {
                    Timeout = TimeSpan.FromSeconds(20)
                };
                var endDate = DateTime.UtcNow.Date;
                var startDate = endDate.AddDays(-20);
                var requestUrl =
                    $"{ApodEndpoint}?api_key={NasaApiKey}&start_date={startDate:yyyy-MM-dd}&end_date={endDate:yyyy-MM-dd}";
                var response = await httpClient.GetAsync(requestUrl);
                response.EnsureSuccessStatusCode();

                var apodItems = await response.Content.ReadFromJsonAsync<List<NasaApodItem>>() ?? new List<NasaApodItem>();
                var articles = apodItems
                    .Where(item => !string.IsNullOrWhiteSpace(item.Title) && !string.IsNullOrWhiteSpace(item.Explanation))
                    .Select(item => new NewsArticle
                    {
                        Id = item.Date ?? Guid.NewGuid().ToString(),
                        Title = item.Title ?? "Untitled NASA Update",
                        Summary = item.Explanation ?? "No summary available.",
                        ImageUrl = item.Url ?? string.Empty,
                        PublishedDate = DateTime.TryParse(item.Date, out var parsedDate) ? parsedDate : DateTime.Now,
                        Source = "NASA"
                    })
                    .OrderByDescending(article => article.PublishedDate)
                    .Take(MaxArticles)
                    .ToList();

                return articles.Count > 0 ? articles : GetFallbackArticles();
            }
            catch
            {
                // Keep the UI populated if the API call fails (network, quota, key, etc.).
                return GetFallbackArticles();
            }
        }

        private class NasaApodItem
        {
            public string? Date { get; set; }
            public string? Title { get; set; }
            public string? Explanation { get; set; }
            public string? Url { get; set; }
        }

        private static List<NewsArticle> GetFallbackArticles()
        {
            return new List<NewsArticle>
            {
                new NewsArticle
                {
                    Id = "fallback-1",
                    Title = "Unable to fetch NASA feed right now",
                    Summary = "Check internet connection or API key status, then try again.",
                    ImageUrl = string.Empty,
                    PublishedDate = DateTime.Now,
                    Source = "Space App"
                },
                new NewsArticle
                {
                    Id = "fallback-2",
                    Title = "Artemis missions continue preparations",
                    Summary = "NASA's Artemis program remains focused on returning humans to the Moon.",
                    ImageUrl = string.Empty,
                    PublishedDate = DateTime.Now.AddDays(-1),
                    Source = "NASA"
                },
                new NewsArticle
                {
                    Id = "fallback-3",
                    Title = "International collaboration in low-Earth orbit",
                    Summary = "ISS partners continue science operations and technology demonstrations.",
                    ImageUrl = string.Empty,
                    PublishedDate = DateTime.Now.AddDays(-2),
                    Source = "ISS"
                },
                new NewsArticle
                {
                    Id = "fallback-4",
                    Title = "Commercial launch cadence remains strong",
                    Summary = "Launch providers continue supporting science and communications payloads.",
                    ImageUrl = string.Empty,
                    PublishedDate = DateTime.Now.AddDays(-3),
                    Source = "Space Industry"
                },
                new NewsArticle
                {
                    Id = "fallback-5",
                    Title = "Deep-space observation campaigns continue",
                    Summary = "Observatories are gathering new data across multiple wavelengths.",
                    ImageUrl = string.Empty,
                    PublishedDate = DateTime.Now.AddDays(-4),
                    Source = "Astronomy Community"
                }
            };
        }
    }
}