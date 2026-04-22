using System.Collections.ObjectModel;
using spaceapp.Models;
using spaceapp.Services;

namespace spaceapp.ViewModels
{
    public class NewsViewModel
    {
        private readonly NewsService _newsService;
        public ObservableCollection<NewsArticle> Articles { get; set; }
        public bool IsLoading { get; set; }
        public Command<NewsArticle> ToggleArticleExpansionCommand { get; }

        public NewsViewModel()
        {
            _newsService = new NewsService();
            Articles = new ObservableCollection<NewsArticle>();
            ToggleArticleExpansionCommand = new Command<NewsArticle>(ToggleArticleExpansion);
        }

        public async Task LoadNewsAsync()
        {
            IsLoading = true;
            var articles = await _newsService.GetNewsArticlesAsync();
            Articles.Clear();
            foreach (var article in articles)
            {
                Articles.Add(article);
            }
            IsLoading = false;
        }

        private void ToggleArticleExpansion(NewsArticle? article)
        {
            if (article is null)
            {
                return;
            }

            article.IsExpanded = !article.IsExpanded;
        }
    }
}