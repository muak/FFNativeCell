using System;
using Xamarin.Forms;
using Sample.ViewModels;
using System.Windows.Input;

namespace Sample.Cells
{
    public class PhotoViewCell : ViewCell
    {
        public static BindableProperty PhotoItemProperty =
            BindableProperty.Create(
                nameof(PhotoItem),
                typeof(MainPageViewModel.PhotoItem),
                typeof(PhotoViewCell),
                null,
                defaultBindingMode: BindingMode.OneWay
            );

        public MainPageViewModel.PhotoItem PhotoItem {
            get { return (MainPageViewModel.PhotoItem)GetValue(PhotoItemProperty); }
            set { SetValue(PhotoItemProperty, value); }
        }

        public static BindableProperty CommandProperty =
            BindableProperty.Create(
                nameof(Command),
                typeof(ICommand),
                typeof(PhotoViewCell),
                null,
                defaultBindingMode: BindingMode.OneWay
            );

        public ICommand Command {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }
    }
}
