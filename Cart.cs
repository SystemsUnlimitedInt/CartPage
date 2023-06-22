using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace App2.Model
{
    public class Cart:INotifyPropertyChanged
    {
        private string _name;
        public string Name 
        {
            set { SetProperty(ref _name, value); }
            get { return _name; }
        }

        private string _imageurl;
        public string ImageUrl
        {
            set { SetProperty(ref _imageurl, value); }
            get { return _imageurl; }
        }

        private byte[] _imagedata;
        public byte[] ImageData 
        {
            set { SetProperty(ref _imagedata, value); }
            get { return _imagedata; }
        }

        private string _price;
        public string Price
        {
            set { SetProperty(ref _price, value); }
            get { return _price; }
        }


        private string _quantity;
        public string Quantity
        {
            set { SetProperty(ref _quantity, value); }
            get { return _quantity; }
        }

        private string _totalprice;
        public string TotalPrice
        {
            set { SetProperty(ref _totalprice, value); }
            get { return _totalprice; }
        }

        

        private string _points;
        public string Points
        {
            set { SetProperty(ref _points, value); }
            get { return _points; }
        }

        private string _points1;
        public string Points1
        {
            set { SetProperty(ref _points1, value); }
            get { return _points1; }
        }

        bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Object.Equals(storage, value))
                return false;
            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public event PropertyChangedEventHandler PropertyChanged;

    }
}
