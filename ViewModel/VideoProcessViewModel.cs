using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Win32;
using VideoProcess.Model;
using VideoProcess.Tools;

namespace VideoProcess.ViewModel
{
    class VideoProcessViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        public Picture picture = new Picture();
        public Transformation transformation = new Transformation();

        private ImageSource loadPicture;
        public ImageSource LoadPicture
        {
            get => loadPicture;
            set
            {
                loadPicture = value;
                OnPropertyChanged(nameof(loadPicture));
            }
        }

        public ICommand PictureOpenCommand
        {
            get => new RelayCommand(() =>
            {
                loadPicture = transformation.PictureTransformation(picture.PictureOpen());
                LoadPicture = loadPicture;
            });
        }
    }
}
