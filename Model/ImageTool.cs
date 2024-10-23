using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace VideoProcess.Model
{
    public class ImageTool
    {
        // 사진 열기
        public String Open()
        {
            string PicturePath = string.Empty;

            OpenFileDialog dialog = new OpenFileDialog
            {
                Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp",
                InitialDirectory = @"C:\"
            };

            if (dialog.ShowDialog() == true)
            {
                PicturePath = dialog.FileName;
            }

            return PicturePath;
        }
        
        // 사진 저장
        public void Save(BitmapImage bitmapImage)
        {
            var encoder = new BmpBitmapEncoder();
            string filePath = string.Empty;
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                DefaultExt = "bmp",
                Filter = "BMP Files (*.bmp)|*.bmp|All Files (*.*)|*.*",
                InitialDirectory = @"C:\"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                filePath = saveFileDialog.FileName;
            }

            if (filePath != string.Empty)
            {
                encoder.Frames.Add(BitmapFrame.Create(bitmapImage));
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    encoder.Save(fileStream);
                }

                if (File.Exists(filePath))
                {
                    MessageBox.Show("완료되었습니다.");
                }
            }
            else
            {
                MessageBox.Show("파일명을 입력해주세요.");
            }
        }
    }
}
