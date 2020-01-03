using AppExample.Views;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AppExample
{
    public partial class App : Application
    {
        public static string Directorio { get; set; }

        public App()
        {
            InitializeComponent();

            //MainPage = new MainPage();
            MainPage = new DevicesPage();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
