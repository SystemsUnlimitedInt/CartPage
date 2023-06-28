using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using MySqlConnector;
using App2.Model;
using System.Runtime.CompilerServices;
using MimeKit;
using MailKit.Net.Smtp;
using System.Security.Cryptography.X509Certificates;
using System.ComponentModel;
using System.Net.Http.Headers;

namespace App2.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    
    public partial class CartPage : ContentPage
    {
        
        public string strOrderNumber5 = string.Empty;
        public string strName1 = CartViewModel.strName;
        public decimal TotalOrder { get; set; }
        public decimal Points { get; set; }
        CartViewModel ViewModel;

        public CartPage(CartViewModel viewModels)
        {
            InitializeComponent();
            BindingContext = viewModels;
            BindingContext = new CartViewModel();
            ViewModel = new CartViewModel();
            ((CartViewModel)BindingContext).LoadCartsCommand.Execute(null);
            ((CartViewModel)BindingContext).LoadTotalsCommand.Execute(null);

            txtOrderNumber.Text = SigninPage.strOrderNumber;
            //txtPoints.Text = CartViewModel.strPoints;
            //txtTotalOrder.Text = CartViewModel.strTotalOrder;

        }



        protected override void OnAppearing()
        {
            base.OnAppearing();


            ((CartViewModel)BindingContext).LoadCartsCommand.Execute(null);
            ((CartViewModel)BindingContext).LoadTotalsCommand.Execute(null);
            //txtPoints.Text = CartViewModel.strPoints;
            //txtTotalOrder.Text = CartViewModel.strTotalOrder;
            
        }
        
        


        private void Button_Clicked(object sender, EventArgs e)
        {
            
            ((CartViewModel)BindingContext).PayCommand.Execute(null);
            
        }

        public void RadioButton_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            
            if (Delivery.IsChecked == true)
            {
                ((CartViewModel)BindingContext).DeliveryCommand.Execute(null);
            }
            else
            {
                ((CartViewModel)BindingContext).CollectCommand.Execute(null);
            }
        }

        

    }
}
