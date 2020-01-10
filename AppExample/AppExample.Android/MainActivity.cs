using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android;
using Xamarin.Forms;
using Android.Graphics;

namespace AppExample.Droid
{
    [Activity(Label = "AppExample", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);
            App.Directorio = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads).AbsolutePath;

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());

            this.RequestPermissions(new[]
            {
                Manifest.Permission.AccessCoarseLocation,
                Manifest.Permission.BluetoothPrivileged
            }, 0);

            printf_50();
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }


        public async void printf_50()
        {

            Bitmap bitmap =await BitmapFactory.DecodeResourceAsync(BaseContext.Resources, Resource.Drawable.logo);
            //Bitmap bitmap = BitmapFactory.DecodeResource(BaseContext.Resources, Resource.Drawable.logo);
            //计算居中的左边距  Calcular el margen izquierdo centrado
            var aw = bitmap.ByteCount;
            int left = getCenterLeft(48, bitmap);
            byte[] bytes = bitmap2PrinterBytes(bitmap, 1);
            App.Bytes = bytes;

            
        }

        public static int getCenterLeft(int paperWidth, Bitmap bitmap)
        {
            //计算居中的边距 Calculate the central margin
            int width = bitmap.Width;
            //计算出图片在纸上宽度 单位为mm   8指的是1mm=8px
            //Calculating the width of the picture on paper in mm 8 means 1 mm = 8 PX
            //Calcule el ancho de la imagen en papel. La unidad es mm. 8 se refiere a 1 mm = 8px.
            float bitmapPaperWidth = width / 8F;
            //79为真实纸宽
            // 79 for real paper width.
            //79 es ancho de papel real
            return (int)(paperWidth / 2F - bitmapPaperWidth / 2);
        }

        /**
 * 将bitmap对象转化成byte数组
 *Converting bitmap objects into byte arrays
 * @param bitmap
 * @param left：图片左边距  Left margin of picture
 * @return 图片居中方法：可增加左边距，让图片居中  Picture centering method: can increase the left margin, so that the picture centered
 */

  /**
  * Convertir un objeto de mapa de bits en una matriz de bytes
  * @param bitmap
  * @param izquierda: margen izquierdo de la imagen
 * @return Método de centrado de la imagen: puede aumentar el margen izquierdo, de modo que la imagen se centre
 */
        public static byte[] bitmap2PrinterBytes(Bitmap bitmap, int left)
        {
            int width = bitmap.Width;
            int height = bitmap.Height;
            byte[] imgbuf = new byte[(width / 8 + left + 4) * height];
            byte[] bitbuf = new byte[width / 8];
            int[] p = new int[8];
            int s = 0;
            Console.WriteLine("+++++++++++++++ Total Bytes: " + (width / 8 + 4) * height);

            for (int y = 0; y < height; ++y)
            {
                int n;
                for (n = 0; n < width / 8; ++n)
                {
                    int value;
                    for (value = 0; value < 8; ++value)
                    {
                        int grey = bitmap.GetPixel(n * 8 + value, y);
                        int red = ((grey & 0x00FF0000) >> 16);
                        int green = ((grey & 0x0000FF00) >> 8);
                        int blue = (grey & 0x000000FF);
                        int gray = (int)(0.29900 * red + 0.58700 * green + 0.11400 * blue); // 灰度转化公式   Fórmula de conversión gris
                        if (gray <= 190)
                        {
                            gray = 1;
                        }
                        else
                        {
                            gray = 0;
                        }
                        p[value] = gray;
                    }
                    value = p[0] * 128 + p[1] * 64 + p[2] * 32 + p[3] * 16 + p[4] * 8 + p[5] * 4 + p[6] * 2 + p[7];
                    bitbuf[n] = (byte)value;
                }

                if (y != 0)
                {
                    ++s;
                    imgbuf[s] = 22;
                }
                else
                {
                    imgbuf[s] = 22;
                }

                ++s;
                imgbuf[s] = (byte)(width / 8 + left);

                for (n = 0; n < left; ++n)
                {
                    ++s;
                    imgbuf[s] = 0;
                }

                for (n = 0; n < width / 8; ++n)
                {
                    ++s;
                    imgbuf[s] = bitbuf[n];
                }

                ++s;
                imgbuf[s] = 21;
                ++s;
                imgbuf[s] = 1;
            }

            return imgbuf;
        }

    }
}