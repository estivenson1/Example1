using MvvmHelpers;
using Xamarin.Forms;

namespace AppExample.ViewModels
{
    public class ViewModelBase : BaseViewModel
    {
        protected Page page;

        public ViewModelBase(Page page)
        {
            this.page = page;
        }
    }
}
