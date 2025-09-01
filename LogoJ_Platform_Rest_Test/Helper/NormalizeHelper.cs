using System;
using System.Data.SqlTypes;
using System.Drawing;
using System.IO;
using ImageMagick;

namespace LogoJ_Platform_Rest_Test.Helper
{
    internal static class NormalizeHelper
    {
        // GDI+ ile decode edilebiliyor mu?
        internal static bool CanDecodeWithGdi(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0) return false;
            try
            {
                using (MemoryStream ms = new MemoryStream(bytes))
                using (Image img = Image.FromStream(ms, false, true))
                {
                    // sadece açılıyor ve dispose ediliyor
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        // Görüntüyü normalize et (CMYK -> sRGB, baseline JPEG/PNG)
        internal static byte[] NormalizeForDisplay(byte[] input, out string ext)
        {
            ext = ".jpg";
            using (MagickImage img = new MagickImage(input))
            {
                if (img.ColorSpace != ColorSpace.sRGB)
                    img.ColorSpace = ColorSpace.sRGB;

                img.Strip(); // metadata temizle
                img.Format = MagickFormat.Jpeg;
                img.Quality = 90;

                byte[] jpegBytes = img.ToByteArray();
                if (CanDecodeWithGdi(jpegBytes))
                {
                    ext = ".jpg";
                    return jpegBytes;
                }

                // JPEG bile açılamazsa PNG fallback
                img.Format = MagickFormat.Png;
                byte[] pngBytes = img.ToByteArray();
                ext = ".png";
                return pngBytes;
            }
        }

        // DB'den gelen objeyi byte[]'e çevir
        public static byte[] AsByteArray(object v)
        {
            if (v == null)
                return null;
            if (v is byte[] b)
                return b;
            if (v is SqlBytes sb && !sb.IsNull)
                return sb.Value;
            return null;
        }
    }
}
