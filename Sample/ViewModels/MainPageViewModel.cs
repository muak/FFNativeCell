using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;

namespace Sample.ViewModels
{
    public class MainPageViewModel : BindableBase, INavigationAware
    {
        public ObservableCollection<PhotoItem> Items { get; set; }

        private DelegateCommand<object> _GoDetailCommand;
        public DelegateCommand<object> GoDetailCommand {
            get {
                return _GoDetailCommand = _GoDetailCommand ?? new DelegateCommand<object>((o) => {
                    _navi.NavigateAsync("DetailPage", new NavigationParameters { { "key", o.ToString() } });
                });
            }
        }

        INavigationService _navi;
        public MainPageViewModel(INavigationService navigationService)
        {
            _navi = navigationService;
            Items = new ObservableCollection<PhotoItem>();
            for (var i = 0; i < 20; i++) {
                Items.Add(new PhotoItem {
                    PhotoUrl = $"https://kamusoft.jp/openimage/nativecell/{i + 1}.jpg",
                    Title = $"Title {i + 1}",
                    Date = "2017-01-11 11:11:11",
                });
            }
        }

        public void OnNavigatedFrom(NavigationParameters parameters)
        {

        }

        public void OnNavigatedTo(NavigationParameters parameters)
        {

        }

        public void OnNavigatingTo(NavigationParameters parameters)
        {
        }

        public class PhotoItem
        {
            public string PhotoUrl { get; set; }
            public string Title { get; set; }
            public string Date { get; set; }

        }
    }
}

