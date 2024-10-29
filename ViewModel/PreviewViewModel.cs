using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using VideoProcess.Tools;

namespace VideoProcess.ViewModel
{
    public class PreviewViewModel : OnPropertyChange
    {
        private ImageSource previewImage;
        public ImageSource PreviewImage
        {
            get => previewImage;
            set
            {
                previewImage = value;
                OnPropertyChanged(nameof(PreviewImage));
            }
        }
    }
}
