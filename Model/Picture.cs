using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoProcess.Model
{
    public class Picture
    {
        public String PictureOpen()
        {
            string PicturePath = "";
            var openFileDialong = new OpenFileDialog
            {
                Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp"
            };

            if (openFileDialong.ShowDialog() == true)
            {
               PicturePath = openFileDialong.FileName;
            }
            
            return PicturePath;
        }
    }
}
