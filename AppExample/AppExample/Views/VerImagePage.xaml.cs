using AppExample.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AppExample.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class VerImagePage : ContentPage
    {
        public VerImagePage()
        {
            InitializeComponent();




        }

        public async Task<string> AsignarImagen()
        {
            var ms = new MemoryStream();
            byte[] bytes = Encoding.UTF8.GetBytes("Hola Mundo despues de un siglo ya se puede imprimir" + System.Environment.NewLine);
            string fileName2 = Path.Combine(App.Directorio, "Tiquete.png");
            //imagenDir.Source = fileName2;


            imagenDir.Source = ImageSource.FromStream(() =>
            {
                var imagebytes = File.ReadAllBytes(fileName2);
                ms.Write(imagebytes, 0, imagebytes.Length);

                //ms.Write(bytes, 0, bytes.Length);

                ms.Seek(0, SeekOrigin.Begin);
                return ms;
            });

            await Task.Delay(9000);

            return "ftyjfy";
        }


        //public async Task<string> safsf()
        //{
        //    byte[] bytes = Encoding.ASCII.GetBytes("Hola Mundo despues de un siglo ya se puede imprimir" + System.Environment.NewLine);
        //    return r;
        //}

        private async void Button_Clicked(object sender, EventArgs e)
        {
            await AsignarImagen();
        }

        private Task<string> methodAsync()
        {
            return Task.Run(() =>
            {
                //SomeLongRunningMethod();
                return "Hello";
            });
        }

        //private Task<string> methodAsync()
        //{
        //    return Task.Run(() =>
        //    {
        //        return SomeLongRunningMethodThatReturnsAString();
        //    });
        //}

        private void CrearImagen()
        {
            string str = "Hola mundo desde cartagena";
            
        }

        private void btnLocal_Clicked(object sender, EventArgs e)
        {
            ImageResourceExtension imageResourceExtension = new ImageResourceExtension();
            var fff = imageResourceExtension.Source = "logo";

                    Image image = new Image { Source = "logo" };
            imagenDir.Source = image.Source;
        }
    }
}