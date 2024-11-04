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
using VideoProcess.View;
using System.Diagnostics;
using VideoProcessCLR;

namespace VideoProcess.ViewModel
{
    public class VideoProcessViewModel : OnPropertyChange
    {
        public ImageTool imageTool;
        public Converter converter;
        public ImageProcess imageProcess;
       /* public PreviewViewModel PreviewViewModel;
        public PreviewView previewView;*/
        Stopwatch stopwatch = new Stopwatch();
        private double scale = 1.0;
        private Class1 testClass1 = new Class1();

        public VideoProcessViewModel()
        {
            imageTool = new ImageTool();
            converter = new Converter();
            imageProcess = new ImageProcess();
            /*PreviewViewModel = new PreviewViewModel();
            previewView = new PreviewView() { DataContext = PreviewViewModel };*/
            //previewView.Show();
        }

        private ImageSource loadPicture;
        public ImageSource LoadPicture
        {
            get => loadPicture;
            set
            {
                loadPicture = value;
                //PreviewViewModel.PreviewImage = value;
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

        private string time;
        public string Time
        {
            get => time;
            set
            {
                time = value;
                OnPropertyChanged(nameof(Time));
            }
        }

        // 이미지 확대 / 축소
        public void OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var image = sender as Image;

            var position = Mouse.GetPosition(image);

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
            image.RenderTransformOrigin = new System.Windows.Point(position.X / image.ActualWidth, position.Y / image.ActualHeight);
        }

        // 이미지1 열기
        public ICommand PictureOpenCommand
        {
            get => new RelayCommand(() =>
            {
                stopwatch.Start();
                ImageSource openImage = converter.StringToImgSource(imageTool.Open());
                if(openImage != null)
                {
                    loadPicture = openImage;
                    LoadPicture = loadPicture;
                }
                stopwatch.Stop();
                Time = stopwatch.ElapsedMilliseconds.ToString();
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
                stopwatch.Start();
                if (LoadPicture != null)
                {
                    unsafe
                    {   
                        Bitmap bitmap = converter.ImgSourceToBitmap(LoadPicture);
                        byte* p = converter.ImgSourceToBytePointer(bitmap);
                        /*// 이진화 추가
                        bool check = imageProcess.CheckBinary(p, bitmap.Width, bitmap.Height, 1);
                        if (check == false)
                        {
                            bitmap = imageProcess.Binarization(p, bitmap);
                            p = converter.ImgSourceToBytePointer(bitmap);
                        }*/
                        Bitmap processedBitmap = imageProcess.Expansion(p, bitmap);
                        processedPicture = converter.BitmapToImgSource(processedBitmap);
                    }
                    ProcessedPicture = processedPicture;
                }
                stopwatch.Stop();
                Time = stopwatch.ElapsedMilliseconds.ToString();
            });
        }

        // 수축
        public ICommand Shrinkage
        {
            get => new RelayCommand(() =>
            {
                stopwatch.Start();
                if (LoadPicture != null)
                {
                    // 이진화 체크 코드 추가
                    unsafe
                    {
                        Bitmap bitmap = converter.ImgSourceToBitmap(LoadPicture);
                        byte* p = converter.ImgSourceToBytePointer(bitmap);
                        /*// 이진화 추가
                        bool check = imageProcess.CheckBinary(p, bitmap.Width, bitmap.Height, 1);
                        if (check == false)
                        {
                            bitmap = imageProcess.Binarization(p, bitmap);
                            p = converter.ImgSourceToBytePointer(bitmap);
                        }*/
                        Bitmap processedBitmap = imageProcess.Shrinkage(p, bitmap);
                        ProcessedPicture = converter.BitmapToImgSource(processedBitmap);
                    }
                    ProcessedPicture = processedPicture;
                }
                stopwatch.Stop();
                Time = stopwatch.ElapsedMilliseconds.ToString();
            });
        }

        // 히스토그램 평활화
        public ICommand Smoothing
        {
            get => new RelayCommand(() =>
            {
                stopwatch.Start();
                if (LoadPicture != null)
                {
                    unsafe
                    {
                        Bitmap bitmap = converter.ImgSourceToBitmap(LoadPicture);
                        byte* p = converter.ImgSourceToBytePointer(bitmap);
                        //Bitmap processedBitmap = imageProcess.Smoothing(p, bitmap);
                        Bitmap processedBitmap = Class1.Smoothing(p, bitmap);
                        ProcessedPicture = converter.BitmapToImgSource(processedBitmap);
                    }
                    //ProcessedPicture = processedPicture;
                }
                stopwatch.Stop();
                Time = stopwatch.ElapsedMilliseconds.ToString();
            });
        }

        // 이진화
        public ICommand Binization
        {
            get => new RelayCommand(() =>
            {
                stopwatch.Start();
                if (LoadPicture != null)
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
                stopwatch.Stop();
                Time = stopwatch.ElapsedMilliseconds.ToString();
            });
        }

        // 필터 가우스
        public ICommand Gaussian
        {
            get => new RelayCommand(() =>
            {
                stopwatch.Start();
                if (LoadPicture != null)
                {
                    unsafe
                    {
                        Bitmap bitmap = converter.ImgSourceToBitmap(LoadPicture);
                        byte* p = converter.ImgSourceToBytePointer(bitmap);
                        Bitmap processedBitmap = imageProcess.Gaussion(p, bitmap);
                        processedPicture = converter.BitmapToImgSource(processedBitmap);
                    }
                    ProcessedPicture = processedPicture;
                }
                stopwatch.Stop();
                Time = stopwatch.ElapsedMilliseconds.ToString();
            });
        }

        // 필터 라플라스
        public ICommand Laplace
        {
            get => new RelayCommand(() =>
            {
                stopwatch.Start();
                if (LoadPicture != null)
                {
                    unsafe
                    {
                        Bitmap bitmap = converter.ImgSourceToBitmap(LoadPicture);
                        byte* p = converter.ImgSourceToBytePointer(bitmap);
                        Bitmap processedBitmap = imageProcess.Laplace(p, bitmap);
                        processedPicture = converter.BitmapToImgSource(processedBitmap);
                    }
                    ProcessedPicture = processedPicture;
                }
                stopwatch.Stop();
                Time = stopwatch.ElapsedMilliseconds.ToString();
            });
        }

        // fft
        public ICommand Fft
        {
            get => new RelayCommand(() =>
            {
                if (LoadPicture != null)
                {
                    unsafe
                    {
                        Bitmap bitmap = converter.ImgSourceToBitmap(LoadPicture);
                        byte* p = converter.ImgSourceToBytePointer(bitmap);
                        Bitmap processedBitmap = imageProcess.Fft(p, bitmap);
                        processedPicture = converter.BitmapToImgSource(processedBitmap);
                    }
                    ProcessedPicture = processedPicture;
                }
            });
        }

        // 템플릿매칭
        public ICommand Matching
        {
            get => new RelayCommand(() =>
            {
                stopwatch.Start();
                if (LoadPicture != null)
                {
                    ImageSource openImage = converter.StringToImgSource(imageTool.Open());
                    processedPicture = openImage;

                    if (processedPicture != null)
                    {
                        unsafe
                        {
                            Bitmap templateBitmap = converter.ImgSourceToBitmap(processedPicture);
                            byte* tBitmap = converter.ImgSourceToBytePointer(templateBitmap);

                            Bitmap originalBitmap = converter.ImgSourceToBitmap(loadPicture);
                            byte* pBitmap = converter.ImgSourceToBytePointer(originalBitmap);

                            System.Drawing.Point bestPoint = imageProcess.Matching(templateBitmap, tBitmap, originalBitmap, pBitmap);

                            int x = bestPoint.X;
                            int y = bestPoint.Y;
                            Rectangle rectangle = new Rectangle(x, y, templateBitmap.Width, templateBitmap.Height);

                            using (Graphics g = Graphics.FromImage(originalBitmap))
                            {
                                using (System.Drawing.Pen pen = new System.Drawing.Pen(System.Drawing.Color.Red, 3))
                                {
                                    g.DrawRectangle(pen, rectangle);
                                }
                            }

                            ProcessedPicture = converter.BitmapToImgSource(originalBitmap);
                        }
                    }
                    else
                    {
                        System.Windows.MessageBox.Show("이미지를 선택해주세요.");
                        return;
                    }    
                }
                stopwatch.Stop();
                Time = stopwatch.ElapsedMilliseconds.ToString();
            });
        }
    }
}
