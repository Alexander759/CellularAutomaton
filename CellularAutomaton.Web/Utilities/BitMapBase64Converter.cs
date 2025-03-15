using SkiaSharp;
using static System.Net.Mime.MediaTypeNames;
using System.Buffers.Text;

namespace CellularAutomaton.Web.Utilities
{
    public static class BitMapBase64Converter
    {
        public static string ConvertSKBitmapToBase64(SKBitmap bitmap)
        {
            using (SKImage image = SKImage.FromBitmap(bitmap))
            using (SKData data = image.Encode(SKEncodedImageFormat.Png, 100))
            using (MemoryStream ms = new MemoryStream())
            {
                data.SaveTo(ms);
                byte[] byteArray = ms.ToArray();
                return Convert.ToBase64String(byteArray);
            }
        }

        public static SKBitmap ConvertBase64ToSKBitmap(string base64String)
        {
            try
            {
                byte[] byteArray = Convert.FromBase64String(base64String);
                using (MemoryStream ms = new MemoryStream(byteArray))
                {
                    return SKBitmap.Decode(ms);
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
