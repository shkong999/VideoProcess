﻿using System;
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

namespace VideoProcess.ViewModel
{
    public class VideoProcessViewModel : OnPropertyChange
    {
        public ImageTool imageTool;
        public Model.Converter converter;
        public ImageProcess imageProcess;

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

                if (processedPicture != null)
                {
                    imageTool.Save(converter.ImgSourceToBitmapImg(processedPicture));
                }
            });
        }

        public void OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var Image = sender as System.Windows.Controls.Image;
            double zoom = 1.0;

            // 위로 올렸을 때
            if (e.Delta > 0)
            {
                zoom *= 2;
            }
            // 아래로 내렸을 때
            else
            {
                zoom /= 2;
            }

            Image.Width = Image.Width * zoom;
            Image.Height = Image.Height * zoom;
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
                        Bitmap processedBitmap = imageProcess.Binization(p, bitmap);
                        processedPicture = converter.BitmapToImgSource(processedBitmap);
                    }
                    ProcessedPicture = processedPicture;
                }
            });
        }
    }
}
