using spaceapp.ViewModels;

namespace spaceapp
{
    public partial class MainPage : ContentPage
    {
        private NewsViewModel _viewModel;

        public MainPage()
        {
            InitializeComponent();
            _viewModel = new NewsViewModel();
            BindingContext = _viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.LoadNewsAsync();
        }
    }
}