using Microsoft.Win32;
using System;
using System.Windows;
using System.Drawing;
using System.IO;
using System.Diagnostics;

namespace pngtoascii
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string filename;
        Bitmap bmporig;
        byte[] data;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void bt_loadfile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
                tb_openedimage.Text = filename = openFileDialog.FileName;
            bmporig = ImageProcessing.ConvertPixelFormat(new Bitmap(filename));
            //bmpimage.p
            bmp_image.Source = ImageProcessing.ImageSourceForBitmap(bmporig);
        }

        private char[] getcharsfromtb()
        {
            return tb_symbols.Text.ToCharArray();
        }

        private Bitmap resizebmp(Bitmap orig, int newwidth)
        {
            int newheidht = Convert.ToInt32(Math.Round(Convert.ToDouble(newwidth) / orig.Width * orig.Height));
            return ImageProcessing.ResizeBitmap(orig, newwidth, newheidht);
        }

        private void bt_bmptoascii_Click(object sender, RoutedEventArgs e)
        {
            Bitmap bmpused;
            char[] chars = getcharsfromtb();

            if (rb_BOLKNOT.IsChecked ?? false) bmpused = resizebmp(bmporig, 100);
            else if (rb_manual.IsChecked ?? false) bmpused = resizebmp(bmporig, Convert.ToInt32(tb_resultwidth.Text));
            else bmpused = bmporig;

            data = ImageProcessing.GetBytesFromBitmap(bmpused);
            try
            {
                using (StreamWriter sw = new StreamWriter(filename.Remove(filename.LastIndexOf('\\')) + @"\Result.txt"))
                {
                    int j = 0;
                    int buf=0;
                    int symbol=0;
                    int el = 255 * 3 / chars.Length;
                    for (int i = 0; i < data.Length - 3; i +=4)
                    {
                        if (j == bmpused.Width-1)
                        {
                            sw.WriteLine();
                            j = 0;
                        }else j++;

                        buf = data[i] + data[i+1] + data[i + 2];
                        symbol = buf / el - 1;
                        if (symbol < 0) symbol = 0;
                        sw.Write(chars[symbol]);
                    }

                }
            }
            catch { };
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Process PrFolder = new Process();
            ProcessStartInfo psi = new ProcessStartInfo();
            string file = filename.Remove(filename.LastIndexOf('\\')+1) + "Result.txt";
            psi.CreateNoWindow = true;
            psi.WindowStyle = ProcessWindowStyle.Normal;
            psi.FileName = "explorer";
            psi.Arguments = @"/n, /select, " + file;
            PrFolder.StartInfo = psi;
            PrFolder.Start();
        }

        private void rb_manual_Checked(object sender, RoutedEventArgs e)
        {
            tb_resultwidth.IsEnabled = true;
        }

        private void rb_manual_Unchecked(object sender, RoutedEventArgs e)
        {
            tb_resultwidth.IsEnabled = false;
        }
    }
}
