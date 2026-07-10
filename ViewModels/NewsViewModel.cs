using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using spaceapp.Models;
using spaceapp.Services;

namespace spaceapp.ViewModels
{
    public class NewsViewModel : INotifyPropertyChanged
    {
        private readonly NewsService _newsService;
        private bool _isLoading;

        public ObservableCollection<NewsArticle> Articles { get; set; }

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                if (_isLoading == value) return;
                _isLoading = value;
                OnPropertyChanged();
            }
        }

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

            await Task.Delay(2000);   // ← Temporary 2-second delay for testing

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
            if (article is null) return;
            article.IsExpanded = !article.IsExpanded;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}