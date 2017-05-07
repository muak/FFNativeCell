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
		public ObservableCollection<PhotoItem> Items{get;set;}

		private string _title;
		public string Title {
			get { return _title; }
			set { SetProperty(ref _title, value); }
		}

		public MainPageViewModel()
		{
			Items = new ObservableCollection<PhotoItem>();
			for(var i=0;i<20;i++){
				Items.Add(new PhotoItem{
					PhotoUrl=$"http://kamusoft.jp/openimage/nativecell/{i+1}.jpg",
					Title = "たいとる",
					Date = "2017-01-11 11:11:11"
				});
			}
		}

		public void OnNavigatedFrom(NavigationParameters parameters)
		{

		}

		public void OnNavigatedTo(NavigationParameters parameters)
		{
			if (parameters.ContainsKey("title"))
				Title = (string)parameters["title"] + " and Prism";
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

