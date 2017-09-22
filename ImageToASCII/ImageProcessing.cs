using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace pngtoascii
{
    static class ImageProcessing
    {
        //begin for convert bitmap to imagesource
        [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeleteObject([In] IntPtr hObject);

        //end

        public static byte[] GetBytesFromBitmap(Bitmap image)
        {
            // Lock the bitmap's bits.

            BitmapData bits = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadOnly, image.PixelFormat);
            IntPtr ptr = bits.Scan0;
            // Declare an array to hold the bytes of the bitmap.
            int numbytes = bits.Stride * image.Height;
            byte[] bytes = new byte[numbytes];
            // Copy the RGB values into the array.
            Marshal.Copy(ptr, bytes, 0, numbytes);
            // Unlock the bits.
            image.UnlockBits(bits);
            return bytes;
        }

        public static void SetBytesToBmp(Bitmap image, byte[] bytes)
        {
            BitmapData bits = image.LockBits(new System.Drawing.Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadOnly, image.PixelFormat);
            IntPtr ptr = bits.Scan0;
            int numbytes = bits.Stride * image.Height;
            bits = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.WriteOnly, image.PixelFormat);
            ptr = bits.Scan0;
            // Copy the array of color values into bitmap
            Marshal.Copy(bytes, 0, ptr, numbytes);
            // Unlock the bits.
            image.UnlockBits(bits);

        }

        public static Bitmap MakeGray(Bitmap image)
        {
            //create a blank bitmap the same size as original
            Bitmap newBitmap = new Bitmap(image.Width, image.Height);

            //get a graphics object from the new image
            Graphics g = Graphics.FromImage(newBitmap);

            //create the grayscale ColorMatrix
            ColorMatrix colorMatrix = new ColorMatrix(
            new float[][]
            {
                new float[] {.3f, .3f, .3f, 0, 0},
                new float[] {.59f, .59f, .59f, 0, 0},
                new float[] {.11f, .11f, .11f, 0, 0},
                new float[] {0, 0, 0, 1, 0},
                new float[] {0, 0, 0, 0, 1}
            });

            //create some image attributes
            ImageAttributes attributes = new ImageAttributes();

            //set the color matrix attribute
            attributes.SetColorMatrix(colorMatrix);

            //draw the original image on the new image
            //using the grayscale color matrix
            g.DrawImage(image, new Rectangle(0, 0, image.Width, image.Height),
               0, 0, image.Width, image.Height, GraphicsUnit.Pixel, attributes);

            //dispose the Graphics object
            g.Dispose();
            return newBitmap;
        }

        public static Bitmap ResizeBitmap(Bitmap sourceBMP, int width, int height)
        {
            Bitmap result = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(result))
                g.DrawImage(sourceBMP, 0, 0, width, height);
            return result;
        }

        public static Bitmap ConvertPixelFormat(Bitmap image)
        {
            Bitmap clone = new Bitmap(image.Width, image.Height,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            using (Graphics gr = Graphics.FromImage(clone))
            {
                gr.DrawImage(image, new Rectangle(0, 0, clone.Width, clone.Height));
            }
            return clone;
        }

        public static ImageSource ImageSourceForBitmap(Bitmap image)
        {
            var handle = image.GetHbitmap();
            try
            {
                return Imaging.CreateBitmapSourceFromHBitmap(handle, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            }
            finally { DeleteObject(handle); }
        }

       // public s
    }
}
