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

[assembly: ExportRenderer(typeof(PhotoViewCell), typeof(PhotoViewCellRenderer))]
namespace Sample.Droid.Cells
{
    public class PhotoViewCellRenderer : ViewCellRenderer
    {
        PhotoViewCell _formsCell;
        PhotoNativeCell _nativeCell;

        protected override Android.Views.View GetCellCore(Xamarin.Forms.Cell item, Android.Views.View convertView, Android.Views.ViewGroup parent, Android.Content.Context context)
        {
            _formsCell = item as PhotoViewCell;
            _nativeCell = convertView as PhotoNativeCell;

            if (_nativeCell == null) {
                // The process of creating a new real native cell. 新規セル生成処理。
                _nativeCell = new PhotoNativeCell(context, _formsCell);
            }
            else {
               

                // The process of initializing a cell when recycling. リサイクル時のセル初期化処理。

                // If the previous task is no completed and no canceled, the task is canceled. 前のタスクが未完了で未キャンセルの場合はキャンセルする
                if (_nativeCell.CurrentTask != null && !_nativeCell.CurrentTask.IsCancelled && !_nativeCell.CurrentTask.IsCompleted) {
                    _nativeCell.CurrentTask.Cancel();
                }

                // NativeCellが持っているformsCellの参照の更新
                _nativeCell.PhotoViewCell = _formsCell;

                _nativeCell.SetOnClickListener(null);
            }

            //読み込み完了までの画像など（ここでは透明）
            _nativeCell.ImageView.SetImageResource(global::Android.Resource.Color.Transparent);

            //イメージ読み込み開始
            _nativeCell.CurrentTask = ImageService.Instance.LoadUrl(_formsCell.PhotoItem.PhotoUrl).DownSample(640).Into(_nativeCell.ImageView);
            //Finalize時にCachedImageのメモリ上のキャッシュをクリアするためのキー
            _nativeCell.ImageView.Key = _formsCell.PhotoItem.PhotoUrl;

            //テキストの更新
            _nativeCell.Title.Text = _formsCell.PhotoItem.Title;
            _nativeCell.Date.Text = _formsCell.PhotoItem.Date;

            _nativeCell.SetOnClickListener(_nativeCell);

            return _nativeCell;
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

        public PhotoNativeCell(Context context, Cell cell) : base(context)
        {
            var view = (context as FormsAppCompatActivity).LayoutInflater.Inflate(Resource.Layout.PhotoViewCell, this, true);

            PhotoViewCell = cell as PhotoViewCell;

            ImageView = view.FindViewById<MyImageView>(Resource.Id.PhotoViewImage);
            Title = view.FindViewById<TextView>(Resource.Id.PhotoViewTitle);
            Date = view.FindViewById<TextView>(Resource.Id.PhotoViewDate);

            PhotoViewCell.PropertyChanged += (sender, e) => {
                if(e.PropertyName == PhotoViewCell.PhotoItemProperty.PropertyName)
                {
                    //前のタスクが未完了で未キャンセルの場合はキャンセルする
                    if (CurrentTask != null && !CurrentTask.IsCancelled && !CurrentTask.IsCompleted) {
                        CurrentTask.Cancel();
                    }

                    ImageView.SetImageResource(global::Android.Resource.Color.Transparent);

                    //イメージ読み込み開始
                    // TODO: DownSampleで大きめを指定すると読み込みエラーが多発する問題あり
                    CurrentTask = ImageService.Instance.LoadUrl(PhotoViewCell.PhotoItem.PhotoUrl).DownSample(640).Into(ImageView);
                    //Finalize時にCachedImageのメモリ上のキャッシュをクリアするためのキー
                    ImageView.Key = PhotoViewCell.PhotoItem.PhotoUrl;

                    //テキストの更新
                    Title.Text = PhotoViewCell.PhotoItem.Title;
                    Date.Text = PhotoViewCell.PhotoItem.Date;
                }
            };
        }

        public void UpdateCell()
        {
            //前のタスクが未完了で未キャンセルの場合はキャンセルする
            if (CurrentTask != null && !CurrentTask.IsCancelled && !CurrentTask.IsCompleted) {
                CurrentTask.Cancel();
            }

            ImageView.SetImageResource(global::Android.Resource.Color.Transparent);

            //イメージ読み込み開始
            // TODO: DownSampleで大きめを指定すると読み込みエラーが多発する問題あり
            CurrentTask = ImageService.Instance.LoadUrl(PhotoViewCell.PhotoItem.PhotoUrl).DownSample(640).Into(ImageView);
            //Finalize時にCachedImageのメモリ上のキャッシュをクリアするためのキー
            ImageView.Key = PhotoViewCell.PhotoItem.PhotoUrl;

            //テキストの更新
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
