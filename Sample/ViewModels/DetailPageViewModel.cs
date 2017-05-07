using System;
using Prism.Mvvm;
using Prism.Navigation;
namespace Sample.ViewModels
{
    public class DetailPageViewModel : BindableBase, INavigationAware
    {
        private string _Image;
        public string Image {
            get { return _Image; }
            set { SetProperty(ref _Image, value); }
        }

        public DetailPageViewModel()
        {
        }

        public void OnNavigatedFrom(NavigationParameters parameters)
        {
            //throw new NotImplementedException();
        }

        public void OnNavigatedTo(NavigationParameters parameters)
        {
            //throw new NotImplementedException();
        }

        public void OnNavigatingTo(NavigationParameters parameters)
        {
            if (parameters.ContainsKey("key")) {
                Image = parameters["key"].ToString();
            }
        }
    }
}
