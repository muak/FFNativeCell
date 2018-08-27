using System;
using Xamarin.Forms.Platform.Android;
using Android.Widget;
using Xamarin.Forms;
using Sample.Cells;
using Sample.Droid.Cells;
using FFImageLoading.Views;
using FFImageLoading;
using Android.Content;
using Android.Runtime;
using Android.Util;
using FFImageLoading.Work;
using Android.Views;
using System.ComponentModel;

[assembly: ExportRenderer(typeof(PhotoViewCell), typeof(PhotoViewCellRenderer))]
namespace Sample.Droid.Cells
{
    public class PhotoViewCellRenderer : ViewCellRenderer
    {
        protected override Android.Views.View GetCellCore(Xamarin.Forms.Cell item, Android.Views.View convertView, Android.Views.ViewGroup parent, Android.Content.Context context)
        {
            var formsCell = item as PhotoViewCell;
            var nativeCell = convertView as PhotoNativeCell;

            if (nativeCell == null) {
                // Creating a new real native cell. 
                // 新規セル生成処理
                nativeCell = new PhotoNativeCell(context, formsCell);
            }

            // Unsubscribe the privious formscell propertychanged event on the nativecell.
            nativeCell.PhotoViewCell.PropertyChanged -= nativeCell.CellPropertyChanged;

            // Update the formscell reffered by the nativecell. 
            nativeCell.PhotoViewCell = formsCell;

            // Subscribe the current formscell propertychanged event on the nativecell.
            nativeCell.PhotoViewCell.PropertyChanged += nativeCell.CellPropertyChanged;

            // Update the nativecell contents with the current formscell contents.
            nativeCell.UpdateCell();

            return nativeCell;
        }
    }

    public class PhotoNativeCell : LinearLayout, INativeElementView, Android.Views.View.IOnClickListener
    {
        public PhotoViewCell PhotoViewCell { get; set; }
        public Element Element => PhotoViewCell;
        public IScheduledWork CurrentTask { get; set; }

        public MyImageView ImageView { get; set; }
        public TextView Title { get; set; }
        public TextView Date { get; set; }

        public PhotoNativeCell(Context context, PhotoViewCell formsCell) : base(context)
        {
            var view = (context as FormsAppCompatActivity).LayoutInflater.Inflate(Resource.Layout.PhotoViewCell, this, true);

            PhotoViewCell = formsCell;

            ImageView = view.FindViewById<MyImageView>(Resource.Id.PhotoViewImage);
            Title = view.FindViewById<TextView>(Resource.Id.PhotoViewTitle);
            Date = view.FindViewById<TextView>(Resource.Id.PhotoViewDate);

            SetOnClickListener(this);
        }

        public void CellPropertyChanged(object sender,PropertyChangedEventArgs e)
        {
            if (e.PropertyName == PhotoViewCell.PhotoItemProperty.PropertyName) {
                // Update for when the cache strategy of Xamarin.Forms.ListView is RecycleElement
                // or changing dynamically Xamarin.Forms cell value.
                UpdateCell();
            }
        }

        public void UpdateCell()
        {

            // If the previous task is no completed and no canceled, the task is canceled. 
            // 前のタスクが未完了で未キャンセルの場合はキャンセルする
            if (CurrentTask != null && !CurrentTask.IsCancelled && !CurrentTask.IsCompleted) {
                CurrentTask.Cancel();
            }

            // An alternate image until the image is completely loaded (here is transparent).
            // 読み込み完了までの画像など（ここでは透明）
            ImageView.SetImageResource(global::Android.Resource.Color.Transparent);

            // Begin loading the image
            // イメージ読み込み開始
            // TODO: DownSampleで大きめを指定すると読み込みエラーが多発する問題あり
            CurrentTask = ImageService.Instance.LoadUrl(PhotoViewCell.PhotoItem.PhotoUrl).DownSample(320).Into(ImageView);
            // Set the key in order to clear the CachedImage memory cache when finalizing.
            // Finalize時にCachedImageのメモリ上のキャッシュをクリアするためのキー
            ImageView.Key = PhotoViewCell.PhotoItem.PhotoUrl;

            // Update the cell's text
            Title.Text = PhotoViewCell.PhotoItem.Title;
            Date.Text = PhotoViewCell.PhotoItem.Date;
        }

        public void OnClick(Android.Views.View v)
        {
            PhotoViewCell.Command?.Execute(PhotoViewCell.PhotoItem.PhotoUrl);
        }
    }

    [Android.Runtime.Preserve(AllMembers = true)]
    [Register("sample.droid.cells.MyImageView")]
    public class MyImageView : ImageViewAsync
    {
        public string Key { get; set; }

        public MyImageView(Context context) : base(context)
        {
        }

        public MyImageView(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public MyImageView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
        }

        protected override void JavaFinalize()
        {
            SetImageDrawable(null);
            SetImageBitmap(null);
            ImageService.Instance.InvalidateCacheEntryAsync(Key, FFImageLoading.Cache.CacheType.Memory);
            base.JavaFinalize();
        }
    }
}
