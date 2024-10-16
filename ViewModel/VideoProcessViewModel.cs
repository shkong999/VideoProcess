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
using VideoProcess.Model;
using VideoProcess.Tools;
using System.Windows.Media.Imaging;
using System.Drawing;
using SkiaSharp;

namespace VideoProcess.ViewModel
{
    public class VideoProcessViewModel : OnPropertyChange
    {
        public Picture picture;
        public Transformation transformation;
        public PictureProcess pictureProcess;

        public VideoProcessViewModel()
        {
            picture = new Picture();
            transformation = new Transformation();
            pictureProcess = new PictureProcess();
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

        // 이미지1 열기
        public ICommand PictureOpenCommand
        {
            get => new RelayCommand(() =>
            {
                loadPicture = transformation.StringToImgSource(picture.PictureOpen());
                LoadPicture = loadPicture;
            });
        }

        // 이미지2 저장
        public ICommand PictureSaveCommand
        {
            get => new RelayCommand(() =>
            {
                BitmapImage bmpimg = loadPicture as BitmapImage;
                picture.PictureSave(bmpimg);
            });
        }

        public ICommand PictureEnlarge
        {
            get => new RelayCommand(() =>
            {
                //picture.PictureSave(loadPicture as BitmapImage);
            });
        }

        /*public void OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            Image img = sender as Image;

            // 위로 올렸을 때
            if (e.Delta > 0)
            {

            }
            // 아래로 내렸을 때
            else
            {

            }
        }*/

        // 이진화
        public ICommand Binization
        {
            get => new RelayCommand(() =>
            {
                if(loadPicture == null)
                {
                    return;
                }

            });
        }
    }
}
