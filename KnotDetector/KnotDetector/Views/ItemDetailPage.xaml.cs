using KnotDetector.ViewModels;
using System.ComponentModel;
using Xamarin.Forms;

namespace KnotDetector.Views
{
    public partial class ItemDetailPage : ContentPage
    {
        public ItemDetailPage()
        {
            InitializeComponent();
            BindingContext = new ItemDetailViewModel();
        }
    }
}