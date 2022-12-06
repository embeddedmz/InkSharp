using System;
using System.Windows.Input;
using System.Windows.Media;
using Color = System.Drawing.Color;
using Microsoft.Win32;
using InkSharp.Extensions;
using InkSharp.Interfaces;
using InkSharp.Demo.Base;
using System.Drawing.Imaging;
using System.IO;
using System.Drawing;

namespace InkSharp.Demo
{
    public class MainViewModel : BaseViewModel
    {
        // Fields
        private IDrawing _signature;
        private ImageSource _printedSignature;
        private string _base64EncodedImage, _arrayString;

        // Constructors
        public MainViewModel()
        {
            _signature = new Drawing
            {
                PenColor = Color.Blue,
                DPI = 96f
            };
        }

        // Commands
        public ICommand PrintBitmapCommand => new Command(() => PrintedSignature = Signature.ToImageSource());
        public ICommand ClearCommand => new Command(() => Signature.Clear());
        public ICommand SaveCommand => new Command(() => Save());
        public ICommand LoadCommand => new Command(() => Load());
        public ICommand LoadBase64PNGCommand => new Command(() => LoadBase64PNG());
        public ICommand EncodeCommand => new Command(() => Base64EncodedImage = Signature.ToBase64());
        public ICommand ArrayCommand => new Command(() => ArrayString = Signature.ToString());

        // Properties
        public IDrawing Signature
        {
            get => _signature;
            set => SetProperty(ref _signature, value);
        }
        public ImageSource PrintedSignature
        {
            get => _printedSignature;
            set => SetProperty(ref _printedSignature, value);
        }
        public string Base64EncodedImage
        {
            get => _base64EncodedImage;
            set => SetProperty(ref _base64EncodedImage, value);
        }
        public string ArrayString
        {
            get => _arrayString;
            set => SetProperty(ref _arrayString, value);
        }

        // Private
        private void Save()
        {
            var saveFileDialog = new SaveFileDialog();
            saveFileDialog.FileName = "";
            saveFileDialog.DefaultExt = ".png";
            saveFileDialog.Filter = "PNG (.png)|*.png";

            if (saveFileDialog.ShowDialog() == true)
            {
                using (Bitmap bm = Signature.ToBitmap())
                {
                    bm.Save(saveFileDialog.FileName, ImageFormat.Png);
                }
            }
        }

        private void Load()
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.FileName = "";
            openFileDialog.DefaultExt = ".png";
            openFileDialog.Filter = "PNG (.png)|*.png";

            if (openFileDialog.ShowDialog() == true)
            {
                using (Bitmap bitmap = new Bitmap(openFileDialog.FileName))
                {
                    PrintedSignature = bitmap.ToImageSource();
                }

                byte[] bytes = File.ReadAllBytes(openFileDialog.FileName);
                Base64EncodedImage = Convert.ToBase64String(bytes);
            }
        }

        private void LoadBase64PNG()
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.FileName = "";
            openFileDialog.DefaultExt = ".txt";
            openFileDialog.Filter = "TXT (.txt)|*.txt";

            if (openFileDialog.ShowDialog() == true)
            {
                string base64String = File.ReadAllText(openFileDialog.FileName);
                byte[] pngBytes = Convert.FromBase64String(base64String);

                using (var ms = new MemoryStream(pngBytes))
                {
                    using (var bitmap = new Bitmap(ms))
                    {
                        PrintedSignature = bitmap.ToImageSource();
                    }
                }
            }
        }
    }
}
