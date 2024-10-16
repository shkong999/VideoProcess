using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace VideoProcess.Model
{
    public class Picture
    {
        // 사진 열기
        public String PictureOpen()
        {
            string PicturePath = string.Empty;

            OpenFileDialog dialog = new OpenFileDialog
            {
                Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp",
                InitialDirectory = @"C:\"
            };
            // 초기 디렉토리 설정

            if (dialog.ShowDialog() == true)
            {
                PicturePath = dialog.FileName;
            }

            return PicturePath;
        }

        // 사진 저장
        public void PictureSave(BitmapImage bitmapImage)
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
