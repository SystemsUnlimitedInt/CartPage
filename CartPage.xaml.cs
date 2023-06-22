using App2.ViewModels;
using System;
using System.Collections;
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

namespace App2.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    
    public partial class CartPage : ContentPage
    {
        
        public string strOrderNumber5 = string.Empty;
        public string strName1 = CartViewModel.strName;
        public decimal TotalOrder { get; set; }
        public decimal Points { get; set; }
        
       
        public CartPage(CartViewModel viewModels)
        {
            InitializeComponent();
            BindingContext = viewModels;
            BindingContext = new CartViewModel();
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

        private void RadioButton_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {

        }

        private async void btnDelete_Clicked(object sender, EventArgs e)
        { 
            ((CartViewModel)BindingContext).LoadTotalsCommand.Execute(null);
            ((CartViewModel)BindingContext).LoadCartsCommand.Execute(null);
           


        }

        //private void btnDelete_Clicked(object sender, EventArgs e)
        //{
        //    ((CartViewModel)BindingContext).LoadTotalsCommand.Execute(null);

        //    txtPoints.Text = CartViewModel.strPoints;
        //    txtTotalOrder.Text = CartViewModel.strTotalOrder;
        //    DisplayAlert("Successful", "Item removed from Cart (▼´•̥ᴥ•̥`) ", "OK");
        //}


    }
}
