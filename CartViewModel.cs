using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Input;
using App2.Model;
using MySqlConnector;
using Xamarin.Forms;
using App2.Views;
using System.Globalization;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MimeKit;
using MailKit.Net.Smtp;


namespace App2.ViewModels
{
    public class CartViewModel : INotifyPropertyChanged
    {
        public string strOrderNumber4 = SigninPage.strOrderNumber;
        public static string strName = string.Empty;
        public string OrderNumber { get; set; }
        public decimal Points { get; set; }
        public decimal TotalOrder { get; set; }
       
        public static string strTotalOrder = string.Empty;
        public string Points1;
        public string strNewPoints = string.Empty;
        public string strOldPoints = string.Empty;

        private string _strPoints;
        public string strPoints
        {
            get
            {
                return _strPoints;
            }
            set
            {
                _strPoints = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(strPoints)));
            }
        }


        private ICommand _loadCartsCommand;
        public ICommand LoadCartsCommand
        {
            get
            {
                if (_loadCartsCommand == null)
                {
                    _loadCartsCommand = new Command(async () => await LoadCarts());
                }

                return _loadCartsCommand;
            }
        }


        private ObservableCollection<Cart> _carts;

        public ObservableCollection<Cart> Carts
        {
            get => _carts;
            set => SetProperty(ref _carts, value);
        }


        public async Task LoadCarts()
        {
            // Call the GetCategories method to load the categories from the database
            Carts = await GetCarts();
        }

        // Method for downloading an image
        private async Task<byte[]> DownloadImageAsync(string url)
        {
            using (var httpClient = new HttpClient())
            {
                return await httpClient.GetByteArrayAsync(url);
            }
        }

        // Method for getting the categories from the database
        private async Task<ObservableCollection<Cart>> GetCarts()
        {
            // Initialize the list of categories
            ObservableCollection<Cart> carts = new ObservableCollection<Cart>();

            // Connect to the database
            using (MySqlConnection connection = new MySqlConnection("mysql string"))
            {
                connection.Open();

                // Create a command to select the categories from the database
                using (MySqlCommand command = new MySqlCommand("SELECT name, image, quantity, price, total, pointvalue FROM cart WHERE ordernumber = '" + strOrderNumber4 + "' GROUP BY name;", connection))
                {
                    // Execute the command and read the results
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            //if (reader.FieldCount >= 5)

                            // Create a new category object
                            Cart cart = new Cart
                            {
                                Name = reader.GetString(0),
                                ImageUrl = reader.GetString(1),
                                Quantity = reader.GetString(2),
                                Price = reader.GetString(3),
                                TotalPrice = "R" + reader.GetString(4),
                                Points = "🐾" + reader.GetString(5),
                                Points1 = reader.GetString(5)

                            };



                            //cart.TotalPrice = double.Format(CultureInfo.CreateSpecificCulture("en-ZA"), "{0:c}", double.Parse(cart.TotalPrice));
                            // Download the image for the category
                            cart.ImageData = await DownloadImageAsync(cart.ImageUrl);

                            
                            strOldPoints = Convert.ToString(cart.Points1);
                            // Add the category to the list
                            carts.Add(cart);
                            strName = reader.GetString(0);
                        }

                    }
                }
                connection.Close();
            }

            // Return the list of categories
            return carts;
        }

        private ICommand _loadTotalsCommand;


        public ICommand LoadTotalsCommand
        {
            get
            {
                if (_loadTotalsCommand == null)
                {
                    _loadTotalsCommand = new Command(async () => await LoadTotals());
                    OnPropertyChanged();
                }

                return _loadTotalsCommand;
            }
        }

        public async Task LoadTotals()
        {
            string sqlstring = "MySQL string";
            MySqlConnection mysqlcon = new MySqlConnection(sqlstring);
            MySqlCommand mysqlcom;
            MySqlDataReader mdr;

            mysqlcon.Open();

            string selectquery = "SELECT ordernumber, quantity, price, sum(price * quantity) AS Total, sum(pointvalue) AS Total1 FROM cart WHERE ordernumber = '" + SigninPage.strOrderNumber + "' GROUP BY ordernumber;";
            mysqlcom = new MySqlCommand(selectquery, mysqlcon);

            mdr = mysqlcom.ExecuteReader();

            while (mdr.Read())
                if (mdr.FieldCount >= 1)
                {
                    TotalOrder = mdr.GetDecimal(3);
                    Points = mdr.GetDecimal(4);
                }



            if (mdr.HasRows == false)
            {
                strTotalOrder =   Convert.ToString(0);
                strPoints =  Convert.ToString(0);
            }
            else
            {
                strTotalOrder = "R" + Convert.ToString(TotalOrder);
                strPoints = "🐾" + Convert.ToString(Points);

            }
             mysqlcon.Close();
            strTotalOrder = "R" + Convert.ToString(TotalOrder);
            strPoints = "🐾" + Convert.ToString(Points);

        }



        private ICommand _cartCommand;

        public ICommand CartsCommand
        {
            get
            {
                if (_cartCommand == null)
                {
                    _cartCommand = new Command<Cart>(async (cart) =>
                    {
                        await OnCartSelected(cart);
                    });
                }

                return _cartCommand;
            }
        }

        



        public async Task OnCartSelected(Cart cart)
        {


            // Navigate to the ProductDetailsPage and pass the selected product as a parameter
            string constring5 = "MySQL string";
            MySqlConnection con5 = new MySqlConnection(constring5);
            con5.Open();
            MySqlTransaction transaction5 = con5.BeginTransaction();

            using (MySqlCommand cmd5 = new MySqlCommand("DELETE FROM cart WHERE ordernumber = '" + strOrderNumber4 + "'", con5))
            {
                cmd5.Transaction = transaction5;

                cmd5.ExecuteNonQuery();
                transaction5.Commit();
                con5.Close();
            }

           
            LoadTotalsCommand.Execute(null);
            


        }

        public ICommand _payCommand;

        public ICommand PayCommand
        {

            get
            {
                if (_payCommand == null)
                {
                    _payCommand = new Command<Cart>(async (cart) =>
                    {
                        await OnPaySelected(cart);
                    });
                }

                return _payCommand;
            }

        }
        
        // Method that gets executed when a category is selected
        private async Task OnPaySelected(Cart cart)
        {
            string constring = "MySQL string";
            MySqlConnection con = new MySqlConnection(constring);
            con.Open();
            MySqlTransaction transaction = con.BeginTransaction();

            using (MySqlCommand cmd = new MySqlCommand("INSERT INTO cartpaid (name, image, price, email, ordernumber, pointvalue, quantity, total ) SELECT name, image, price, email, ordernumber, pointvalue, quantity, total FROM cart WHERE ordernumber = '" + SigninPage.strOrderNumber  + "' ", con))
            {
                
                
                cmd.Transaction = transaction;
                cmd.ExecuteNonQuery();

                transaction.Commit();
                con.Close();
            }
            string constring3 = "MySQL string";
            MySqlConnection con3 = new MySqlConnection(constring3);
            con3.Open();
            MySqlTransaction transaction3 = con3.BeginTransaction();

            using (MySqlCommand cmd3 = new MySqlCommand("UPDATE cartpaid SET orderstatus = 'Yes', paymentmethod = 'EFT', paymentdate = '" + DateTime.Today.ToString("yyyy/MM/dd") + "', paymenttime = '" + DateTime.Today.ToString("yyyy/MM/dd") + "', collected = 'No', void = 'No', location = '" + LocationsViewModel.strLocation + "', datecollected = 'N/A', notification = 'No'  ", con3))
            {


                cmd3.Transaction = transaction3;
                cmd3.ExecuteNonQuery();

                transaction3.Commit();
                con3.Close();
            }


            string constring1 = "MySQL string";
            MySqlConnection con1 = new MySqlConnection(constring1);
            con1.Open();
            MySqlTransaction transaction1 = con1.BeginTransaction();

            using (MySqlCommand cmd1 = new MySqlCommand("DELETE FROM cart WHERE ordernumber = '" + SigninPage.strOrderNumber + "' ", con1))
            {
                cmd1.Transaction = transaction1;
                cmd1.ExecuteNonQuery();
                transaction1.Commit();
                con1.Close();
            }

            string sqlstring = "MySQL string";
            MySqlConnection mysqlcon = new MySqlConnection(sqlstring);
            MySqlCommand mysqlcom;
            MySqlDataReader mdr;

            mysqlcon.Open();

            string selectquery = "SELECT pointvalue FROM registration WHERE emailaddress = '" + SigninPage.strEmail + "' ";
            mysqlcom = new MySqlCommand(selectquery, mysqlcon);

            mdr = mysqlcom.ExecuteReader();

            while (mdr.Read())
                if (mdr.FieldCount >= 1)
                {
                    Points1 = mdr.GetString(0);
                }

            mysqlcon.Close();


            strNewPoints = (Convert.ToInt32(strOldPoints) + Convert.ToInt32(Points1)).ToString();

            string constring2 = "MySQL string";
            MySqlConnection con2 = new MySqlConnection(constring2);
            con2.Open();
            MySqlTransaction transaction2 = con2.BeginTransaction();

            using (MySqlCommand cmd2 = new MySqlCommand("UPDATE registration SET pointvalue = '" + strNewPoints + "' WHERE emailaddress = '" + SigninPage.strEmail + "' ", con2))
            {


                cmd2.Transaction = transaction2;
                cmd2.ExecuteNonQuery();

                transaction2.Commit();
                con2.Close();
            }


            string signature = "<p>Best Regards,<br/>The Woof Wishlist Team</p>";
            string imageSrc = "Signuture Link"; // Replace with the URL of your signature image


            var message = new MimeMessage();
            message.From.Add(new MailboxAddress()); // Replace with your sender address
            message.To.Add(new MailboxAddress(SigninPage.strEmail, SigninPage.strEmail)); // Use the email address from the entry field
            message.Subject = "Order Confirmation";
            message.Body = new TextPart("html")
            {
                Text = $"<html><body><p>Hi {SigninPage.strEmail},</p><p>Your order with order number {SigninPage.strOrderNumber}, has been recieved. We are working on getting your pets Wishlist Fulfilled.</p><p>Please give us 5 to 7 days to get your order together, in the meantime give your pet some cuddles from the Woof Wishlist team.</p><p></p><p></p>{signature}<img src=\"{imageSrc}\" alt=\"Signature\"></body></html>"
            };


            using (var client = new SmtpClient())
            {
                client.Connect();
                client.Authenticate();
                client.Send(message);
                client.Disconnect(true);
            }


            string sqlstring1 = "MySQL string";
            MySqlConnection mysqlcon1 = new MySqlConnection(sqlstring1);
            MySqlCommand mysqlcom1;
            MySqlDataReader mdr1;

            mysqlcon1.Open();

            string selectquery1 = "SELECT ordernumber FROM ordernumber";
            mysqlcom1 = new MySqlCommand(selectquery1, mysqlcon1);

            mdr1 = mysqlcom1.ExecuteReader();

            while (mdr1.Read())
                if (mdr1.FieldCount >= 1)
                {
                    OrderNumber = mdr1.GetString(0);
                }

            mysqlcon1.Close();



            SigninPage.strOrderNumber = (Convert.ToInt32(OrderNumber) + Convert.ToInt32(1)).ToString();


            string constring4 = "MySQL string";
            MySqlConnection con4 = new MySqlConnection(constring4);
            con4.Open();
            MySqlTransaction transaction4 = con4.BeginTransaction();

            using (MySqlCommand cmd4 = new MySqlCommand("INSERT INTO ordernumber (ordernumber) VALUES(@strOrderNumber)", con4))
            {

                cmd4.Parameters.AddWithValue("@strOrderNumber", SigninPage.strOrderNumber);
                cmd4.Transaction = transaction4;
                cmd4.ExecuteNonQuery();

                transaction4.Commit();
                con4.Close();
            }

            await ShowAlert("Successful", "Payment was successful (u≧ᴥ≦)", "OK");
            await Application.Current.MainPage.Navigation.PushAsync(new TabbedPage1());




        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T backingStore, T value, [CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            OnPropertyChanged(propertyName);

            return true;
        }

        // Method for showing alerts
        protected async Task ShowAlert(string title, string message, string buttonText)
        {
            await Application.Current.MainPage.DisplayAlert(title, message, buttonText);
        }
    }
}
