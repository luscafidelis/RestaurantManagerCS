using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Xml.Linq;

namespace RestaurantManager.Model {
    public class Order : INotifyPropertyChanged, IOrder {
        private string customer;
        private ObservableCollection<IItem> items;
        private double total;
        private int table;
        private int id;

        public event PropertyChangedEventHandler PropertyChanged;

        public Order() {
            customer = string.Empty;
            items = new ObservableCollection<IItem>();
            total = 0;
        }

        //Getters & Setters
        public string Customer {
            get { return customer; }
            set { customer = value; Notify("Customer"); }
        }
        public ObservableCollection<IItem> Items {
            get { return items; }
            set { items = value; Notify("IItems"); }
        }
        public double Total {
            get { return total; }
            set { total = value; Notify("Total"); }
        }
        public int Table {
            get { return table; }
            set { table = value; Notify("Table"); }
        }
        public int Id {
            get { return id; }
            set { id = value; Notify("Id"); }
        }

        //Functions
        public void AddItem(IItem newItem) {
            IItem TargetIItem = Find(newItem.Name);
            if (TargetIItem == null)
                items.Add(newItem.ShallowCopy());
            else
                TargetIItem.Quantity = TargetIItem.Quantity + 1;

            this.total += newItem.Price * newItem.Quantity;
            Notify("IItems");
        }

        public void RemoveItem(IItem DeletedIItem) {
            if (DeletedIItem.Quantity == 1)
                items.Remove(DeletedIItem);
            else
                DeletedIItem.Quantity = DeletedIItem.Quantity - 1;

            this.total -= DeletedIItem.Price;
            Notify("IItems");
        }

        public IOrder ShallowCopy() {
            return (Order)this.MemberwiseClone();
        }
        public void Update(IOrder NewOrder) {
            this.Total = NewOrder.Total;
            this.Table = NewOrder.Table;
            this.Customer = NewOrder.Customer;
            this.Items = NewOrder.Items;
        }

        public IItem Find(string Query) {
            try {
                IItem TargetIItem = Items.Where(x => x.Name.Equals(Query)).First();
                return TargetIItem;
            } catch (Exception) { return null; }
        }

        private void Notify(string name) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
