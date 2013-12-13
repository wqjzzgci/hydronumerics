using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Browser;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using ExifLib;

namespace ExifSL
{
    public partial class MainPage : UserControl
    {
        private DispatcherTimer mOnEnterFrame;
        private double mCanvasCenterX, mCanvasCenterY;

        private double mTarget = 0;
        private double mCurrent = 0;
        private double mSpring = 0;

        private const int FPS = 24;
        private const double MAX_IMAGE_SIZE = 160;
        //private const double IMAGE_SIZE_FACTOR = 0.8;
        private const double SPRINGSPEED = 0.4;
        private const double BOUNCESPEED = 0.1;
        private const double SCALE_DOWN_FACTOR = 0.33;
        private const double OFFSET_FACTOR = 120;
        private const double OPACITY_DOWN_FACTOR = 0.33;

        private class ExifImage
        {
            public Image Image { get; private set; }
            public JpegInfo ImageInfo { get; private set; }
            public int PixelWidth { get; private set; }
            public int PixelHeight { get; private set; }
            public double AspectRatio { get; private set; }

            public ExifImage(BitmapImage imageSource, JpegInfo info)
            {
                PixelWidth = imageSource.PixelWidth;
                PixelHeight = imageSource.PixelHeight;
                AspectRatio = (double)PixelWidth / (double)PixelHeight;
                Image = new Image();
                Image.Source = imageSource;
                ImageInfo = info;
            }
        }
        private List<ExifImage> mImages = new List<ExifImage>();

		public MainPage()
		{
			// Required to initialize variables
			InitializeComponent();

            // start the main animation loop
            mOnEnterFrame = new DispatcherTimer();
            mOnEnterFrame.Interval = new TimeSpan(0, 0, 0, 0, 1000 / FPS);
            mOnEnterFrame.Tick += new EventHandler(OnEnterFrame);
            mOnEnterFrame.Start();
		}

        private void OnEnterFrame(object sender, EventArgs e)
        {
            mCanvasCenterX = ImageCanvas.ActualWidth * 0.5;
            mCanvasCenterY = ImageCanvas.ActualHeight * 0.5;

            for (int i = 0; i < mImages.Count; ++i)
            {
                PositionImage(mImages[i], i);
            }
            
            // speed = (delta) * maxSpeed + (speed * acceleration)
            mSpring = ((mTarget - mCurrent) * SPRINGSPEED) + (mSpring * BOUNCESPEED);
            // position = position + speed (where dt = 1 since it's not time based but frame based...
            // Good? not so sure. Easy? yes.)
            mCurrent += mSpring;
        }

        private void PositionImage(ExifImage image, int index)
        {
            double delta = index - mCurrent;
            double deltaAbs = System.Math.Abs(delta);
            //double maxDrawSize = IMAGE_SIZE_FACTOR * ImageCanvas.ActualHeight;
            double maxDrawSize = MAX_IMAGE_SIZE;

            // scale image
            ScaleTransform scale = new ScaleTransform();
            double ratio = 0.0;
            if (image.PixelWidth >= image.PixelHeight)
            {
                // w > h, fit width to draw size
                ratio = maxDrawSize / image.PixelWidth;
            }
            else
            {
                // fit height to draw size
                ratio = maxDrawSize / image.PixelHeight;
            }
            scale.ScaleY = ratio * (1.0 - (deltaAbs * SCALE_DOWN_FACTOR));
            scale.ScaleX = ratio * (1.0 - (deltaAbs * SCALE_DOWN_FACTOR));
            // set the transform
            image.Image.RenderTransform = scale;

            // position the image
            double left = mCanvasCenterX - (image.PixelWidth * scale.ScaleX * 0.5) + delta * OFFSET_FACTOR;
            image.Image.SetValue(Canvas.LeftProperty, left);
            double top = mCanvasCenterY - (image.PixelHeight * scale.ScaleY * 0.5) + 95;
            image.Image.SetValue(Canvas.TopProperty, top);
            // set alpha
            image.Image.Opacity = 1.0 - (deltaAbs * OPACITY_DOWN_FACTOR);
            // z-index
            image.Image.SetValue(Canvas.ZIndexProperty, (int)(maxDrawSize * (1.0 - (deltaAbs * SCALE_DOWN_FACTOR))));
        }

        private void AddImageButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Image Files (*.jpg)|*.jpg";
            ofd.Multiselect = true;

            bool? ok = ofd.ShowDialog();
            if (ok == true)
            {
                foreach (FileInfo fileInfo in ofd.Files)
                {
                    try
                    {
                        AddImage(fileInfo);
                    }
                    catch (System.OutOfMemoryException)
                    {
                        break;
                    }
                }
            }
        }

        private void AddImage(FileInfo fi)
        {
            try
            {
                DateTime then = DateTime.Now;
                JpegInfo info = ExifReader.ReadJpeg(fi);
                BitmapImage img = new BitmapImage();

                if (info.ThumbnailData != null)
                {
                    // the fast way - using the Exif thumbnail for preview
                    MemoryStream ms = new MemoryStream(info.ThumbnailData);
                    img.SetSource(ms);
                }
                else
                {
                    // the slow and expensive way - loading a monster image in memory
                    img.CreateOptions = BitmapCreateOptions.None;
                    using (FileStream fs = fi.OpenRead())
                    {
                        img.SetSource(fs);
                        fs.Close();
                    }
                }

                info.LoadTime = (DateTime.Now - then);
                ExifImage image = new ExifImage(img, info);
                ImageCanvas.Children.Add(image.Image);
                int oldCount = mImages.Count;
                mImages.Add(image);
                if (oldCount == 0) UpdateImageInfo();
            }
            catch (OutOfMemoryException)
            {
                HtmlPage.Window.Alert("You are out of memory. No more images will be loaded.");
                throw;
            }
            catch (Exception e)
            {
                // well that sucks.
                HtmlPage.Window.Alert("An error occured:\r\n" + e.ToString());
            }
            // this could be expensive, try to clean up as we go...
            GC.Collect();
        }

        private void MoveTarget(int offset)
        {
            mTarget += offset;
            mTarget = System.Math.Min(mImages.Count - 1, mTarget);
            mTarget = System.Math.Max(0.0, mTarget);
            UpdateImageInfo();
        }

        private void UpdateImageInfo()
        {
            ExifImage i = mImages[(int)mTarget];
            StringBuilder sb = new StringBuilder(256);
            sb.AppendLine(i.ImageInfo.FileName + " (" + (i.ImageInfo.FileSize / (1 << 10)) + "KB" + ")");
            sb.AppendLine("Load time: " + i.ImageInfo.LoadTime.TotalMilliseconds + "ms");
            if (i.ImageInfo.IsValid)
            {
                sb.AppendLine("Date taken: " + i.ImageInfo.DateTime);
                if (!string.IsNullOrEmpty(i.ImageInfo.Make))
                {
                    sb.AppendLine("Make: " + i.ImageInfo.Make);
                    sb.AppendLine("Model: " + i.ImageInfo.Model);
                }
                sb.AppendLine("Dimensions: " + i.ImageInfo.Width + "x" + i.ImageInfo.Height);
                if (i.ImageInfo.ExposureTime > 0.0)
                {
                    sb.AppendLine("Exposure: " + i.ImageInfo.ExposureTime + " seconds");
                    sb.AppendLine("Flash: " + i.ImageInfo.Flash.ToString());
                }
                sb.AppendLine("Thumbnail: " + ((i.ImageInfo.ThumbnailData == null) ? "No" : "Yes"));
                if (i.ImageInfo.GpsLatitudeRef != ExifGpsLatitudeRef.Unknown)
                {
                    // format GPS info as a Google Maps URL
                    double latitude = i.ImageInfo.GpsLatitude[0] +
                        i.ImageInfo.GpsLatitude[1] / 60.0 +
                        i.ImageInfo.GpsLatitude[2] / 3600.0;
                    if (i.ImageInfo.GpsLatitudeRef == ExifGpsLatitudeRef.South)
                        latitude = -latitude;
                    double longitude = i.ImageInfo.GpsLongitude[0] +
                        i.ImageInfo.GpsLongitude[1] / 60.0 +
                        i.ImageInfo.GpsLongitude[2] / 3600.0;
                    if (i.ImageInfo.GpsLongitudeRef == ExifGpsLongitudeRef.West)
                        longitude = -longitude;
                    sb.AppendLine("http://www.bing.com/maps/?v=2&cp=" + latitude + "~" + longitude + "&lvl=16&sty=r");
                    sb.AppendLine("http://maps.google.com/maps?q=" + latitude + "," + longitude);
                }
            }
            else
            {
                sb.AppendLine("Unrecognized format");
            }
            this.ImageInfoText.Text = sb.ToString();
        }

        private void UserControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MoveTarget(1);
        }

        private void UserControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Right)
            {
                MoveTarget(1);
            }
            else if (e.Key == Key.Left)
            {
                MoveTarget(-1);
            }
        }

        private void ImageCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point pt = e.GetPosition(ImageCanvas);
            MoveTarget((pt.X > mCanvasCenterX) ? 1 : -1);
        }

        private void ImageCanvas_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Left:
                case Key.Down:
                    MoveTarget(-1);
                    break;
                case Key.Right:
                case Key.Up:
                    MoveTarget(1);
                    break;
                case Key.Home:
                    mTarget = 0;
                    UpdateImageInfo();
                    break;
                case Key.End:
                    if (mImages.Count > 0)
                    {
                        mTarget = (mImages.Count - 1);
                        UpdateImageInfo();
                    }
                    break;
                default: break;
            }
        }
	}
}
