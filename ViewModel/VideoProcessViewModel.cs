using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Controls;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Win32;
using VideoProcess.Tools;
using System.Windows.Media.Imaging;
using System.Drawing;
using VideoProcess.Model;
using System.Drawing.Imaging;
using Image = System.Windows.Controls.Image;
using System.Windows.Forms;

namespace VideoProcess.ViewModel
{
    public class VideoProcessViewModel : OnPropertyChange
    {
        public ImageTool imageTool;
        public Model.Converter converter;
        public ImageProcess imageProcess;
        private double scale = 1.0;

        public VideoProcessViewModel()
        {
            imageTool = new ImageTool();
            converter = new Converter();
            imageProcess = new ImageProcess();
        }

        private ImageSource loadPicture;
        public ImageSource LoadPicture
        {
            get => loadPicture;
            set
            {
                loadPicture = value;
                OnPropertyChanged(nameof(LoadPicture));
            }
        }

        private ImageSource processedPicture;
        public ImageSource ProcessedPicture
        {
            get => processedPicture;
            set
            {
                processedPicture = value;
                OnPropertyChanged(nameof(ProcessedPicture));
            }
        }

        public void OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var image = sender as Image;

            if (e.Delta > 0)
            {
                scale *= 1.1;
            }
            else
            {
                scale /= 1.1;
            }

            var transform = new ScaleTransform(scale, scale);
            image.RenderTransform = transform;
            image.RenderTransformOrigin = new System.Windows.Point(0.5, 0.5);

            //if (image.IsVisible)
            //{
            //    var scrollViewer = FindParent<ScrollViewer>(image);
            //    if (scrollViewer != null)
            //    {
            //        double newWidth = image.ActualWidth * scale;
            //        double newHeight = image.ActualHeight * scale;

            //        scrollViewer.ScrollToHorizontalOffset(scrollViewer.HorizontalOffset + (e.GetPosition(image).X - newWidth / 2) * (scale - 1));
            //        scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset + (e.GetPosition(image).Y - newHeight / 2) * (scale - 1));
            //    }
            //}
        }

        /*private T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);
            if (parentObject == null) return null;

            T parent = parentObject as T;
            return parent ?? FindParent<T>(parentObject);
        }*/

        // 이미지1 열기
        public ICommand PictureOpenCommand
        {
            get => new RelayCommand(() =>
            {
                ImageSource openImage = converter.StringToImgSource(imageTool.Open());
                if(openImage != null)
                {
                    loadPicture = openImage;
                    LoadPicture = loadPicture;
                }
            });
        }

        // 이미지2 저장
        public ICommand PictureSaveCommand
        {
            get => new RelayCommand(() =>
            {
                /*BitmapImage bitmap = loadPicture as BitmapImage;
                imageTool.Save(bitmap);*/

                if (ProcessedPicture != null)
                {
                    imageTool.Save(converter.ImgSourceToBitmapImg(ProcessedPicture));
                }
            });
        }

        // 팽창
        public ICommand Expansion
        {
            get => new RelayCommand(() =>
            {
                if (LoadPicture != null)
                {
                    // 이진화 체크 코드 추가
                    unsafe
                    {
                        Bitmap bitmap = converter.ImgSourceToBitmap(LoadPicture);
                        byte* p = converter.ImgSourceToBytePointer(bitmap);
                        // 이진화 추가
                        bool check = imageProcess.CheckBinary(p, bitmap.Width, bitmap.Height, 1);
                        if (check == false)
                        {
                            bitmap = imageProcess.Binarization(p, bitmap);
                            p = converter.ImgSourceToBytePointer(bitmap);
                        }
                        Bitmap processedBitmap = imageProcess.Expansion(p, bitmap);
                        processedPicture = converter.BitmapToImgSource(processedBitmap);
                    }
                    ProcessedPicture = processedPicture;
                }
            });
        }

        // 수축
        public ICommand Shrinkage
        {
            get => new RelayCommand(() =>
            {
                if (LoadPicture != null)
                {
                    // 이진화 체크 코드 추가
                    unsafe
                    {
                        Bitmap bitmap = converter.ImgSourceToBitmap(LoadPicture);
                        byte* p = converter.ImgSourceToBytePointer(bitmap);
                        // 이진화 추가
                        bool check = imageProcess.CheckBinary(p, bitmap.Width, bitmap.Height, 1);
                        if (check == false)
                        {
                            bitmap = imageProcess.Binarization(p, bitmap);
                            p = converter.ImgSourceToBytePointer(bitmap);
                        }
                        Bitmap processedBitmap = imageProcess.Shrinkage(p, bitmap);
                        processedPicture = converter.BitmapToImgSource(processedBitmap);
                    }
                    ProcessedPicture = processedPicture;
                }
            });
        }

        // 히스토그램 평활화
        public ICommand Smoothing
        {
            get => new RelayCommand(() =>
            {
                if (LoadPicture != null)
                {
                    unsafe
                    {
                        Bitmap bitmap = converter.ImgSourceToBitmap(LoadPicture);
                        byte* p = converter.ImgSourceToBytePointer(bitmap);
                        Bitmap processedBitmap = imageProcess.Smoothing(p, bitmap);
                        //BitmapSource test = new BitmapSource(processedBitmap);
                        ImageSource temp = converter.BitmapToImgSource(processedBitmap);
                        ProcessedPicture = temp;
                    }
                    //ProcessedPicture = processedPicture;
                }
            });
        }

        // 이진화
        public ICommand Binization
        {
            get => new RelayCommand(() =>
            {
                if(LoadPicture != null)
                {
                    unsafe
                    {
                        Bitmap bitmap = converter.ImgSourceToBitmap(LoadPicture);
                        byte* p = converter.ImgSourceToBytePointer(bitmap);
                        Bitmap processedBitmap = imageProcess.Binarization(p, bitmap);
                        processedPicture = converter.BitmapToImgSource(processedBitmap);
                    }
                    ProcessedPicture = processedPicture;
                }
            });
        }
    }
}
