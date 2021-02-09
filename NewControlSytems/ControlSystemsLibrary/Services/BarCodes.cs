using BarcodeLib;
using System;
using System.Drawing;
using System.Windows;
using System.Windows.Media.Imaging;

namespace ControlSystemsLibrary.Services
{
    class BarCodes
    {
        ////public static BitmapSource GetBarcodeBitmapSource(ref bool Readiness, string BarcodeType, string Barcode)
        ////{
        ////    try
        ////    {
        ////        Barcode barcode = new Barcode()
        ////        {
        ////            IncludeLabel = true,
        ////            Alignment = AlignmentPositions.CENTER,
        ////            Width = 300,
        ////            Height = 100,
        ////            RotateFlipType = RotateFlipType.RotateNoneFlipNone,
        ////            BackColor = Color.White,
        ////            ForeColor = Color.Black,
        ////        };
        ////        Image img = barcode.Encode(GetBarcodeType(BarcodeType), Barcode);
        ////        Readiness = true;
        ////        return GetImageStream(img);
        ////    }
        ////    catch (Exception ex)
        ////    {
        ////        Readiness = false;
        ////        MessageBox.Show(ex.Message);
        ////        return null;
        ////    }
        ////}

        // 2-перегрузка

        public static bool CheckCreatedBarcode(string BarcodeType, string Barcode)
        {
            try
            {
                Barcode barcode = new Barcode()
                {
                    IncludeLabel = true,
                    RotateFlipType = RotateFlipType.RotateNoneFlipNone,
                };
                Image img = barcode.Encode(GetBarcodeType(BarcodeType), Barcode);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка создания штрих-кода");
                return false;
            }
        }

        public static BitmapSource GetBarcodeBitmapSource(string BarcodeType, string Barcode)
        {
            try
            {
                Barcode barcode = new Barcode()
                {
                    IncludeLabel = true,
                    Alignment = AlignmentPositions.CENTER,
                    Width = 300,
                    Height = 100,
                    RotateFlipType = RotateFlipType.RotateNoneFlipNone,
                    BackColor = Color.White,
                    ForeColor = Color.Black,
                };
                Image img = barcode.Encode(GetBarcodeType(BarcodeType), Barcode);
                return GetImageStream(img);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }


        private static BitmapSource GetImageStream(Image myImage)
        {
            var bitmap = new Bitmap(myImage);
            IntPtr bmpPt = bitmap.GetHbitmap();
            BitmapSource bitmapSource =
             System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                   bmpPt,
                   IntPtr.Zero,
                   Int32Rect.Empty,
                   BitmapSizeOptions.FromEmptyOptions());

            //bitmapSource.Freeze();

            return bitmapSource;
        }

        private static TYPE GetBarcodeType(string BarcodeType)
        {
            switch (BarcodeType)
            {
                case "BOOKLAND": return TYPE.BOOKLAND;
                case "Codabar": return TYPE.Codabar;
                case "CODE11": return TYPE.CODE11;
                case "CODE128": return TYPE.CODE128;
                case "CODE128A": return TYPE.CODE128A;
                case "CODE128B": return TYPE.CODE128B;
                case "CODE128C": return TYPE.CODE128C;
                case "CODE39": return TYPE.CODE39;
                case "CODE39Extended": return TYPE.CODE39Extended;
                case "CODE39_Mod43": return TYPE.CODE39_Mod43;
                case "CODE93": return TYPE.CODE93;
                case "EAN13": return TYPE.EAN13;
                case "EAN8": return TYPE.EAN8;
                case "FIM": return TYPE.FIM;
                case "Industrial2of5": return TYPE.Industrial2of5;
                case "Industrial2of5_Mod10": return TYPE.Industrial2of5_Mod10;
                case "Interleaved2of5": return TYPE.Interleaved2of5;
                case "Interleaved2of5_Mod10": return TYPE.Interleaved2of5_Mod10;
                case "ISBN": return TYPE.ISBN;
                case "ITF14": return TYPE.ITF14;
                case "JAN13": return TYPE.JAN13;
                case "LOGMARS": return TYPE.LOGMARS;
                case "Modified_Plessey": return TYPE.Modified_Plessey;
                case "MSI_2Mod10": return TYPE.MSI_2Mod10;
                case "MSI_Mod10": return TYPE.MSI_Mod10;
                case "MSI_Mod11": return TYPE.MSI_Mod11;
                case "MSI_Mod11_Mod10": return TYPE.MSI_Mod11_Mod10;
                case "PHARMACODE": return TYPE.PHARMACODE;
                case "PostNet": return TYPE.PostNet;
                case "Standard2of5": return TYPE.Standard2of5;
                case "Standard2of5_Mod10": return TYPE.Standard2of5_Mod10;
                case "TELEPEN": return TYPE.TELEPEN;
                case "UCC12": return TYPE.UCC12;
                case "UCC13": return TYPE.UCC13;
                case "UNSPECIFIED": return TYPE.UNSPECIFIED;
                case "UPCA": return TYPE.UPCA;
                case "UPCE": return TYPE.UPCE;
                case "UPC_SUPPLEMENTAL_2DIGIT": return TYPE.UPC_SUPPLEMENTAL_2DIGIT;
                case "UPC_SUPPLEMENTAL_5DIGIT": return TYPE.UPC_SUPPLEMENTAL_5DIGIT;
                case "USD8": return TYPE.USD8;
                default: return TYPE.EAN13;
            }
        }

















    }
}
