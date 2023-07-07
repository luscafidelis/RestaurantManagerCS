using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Xml.Linq;

namespace RestaurantManager.Model {
    public class Order : INotifyPropertyChanged {
        private string customer;
        private ObservableCollection<Item> items;
        private double total;
        private int table;

        public event PropertyChangedEventHandler PropertyChanged;

        public Order() {
            customer = string.Empty;
            items = new ObservableCollection<Item>();
            total = 0;
        }
        public Order(string customer, ObservableCollection<Item> items, double total, int table) {
            this.customer = customer;
            this.items = items;
            this.total = total;
            this.table = table;
        }

        //Getters & Setters
        public string Customer{
            get { return customer; }
            set {  customer = value; Notify("Customer"); }
        }
        public ObservableCollection<Item> Items {
            get { return items; }
            set { items = value; Notify("Items"); }  
        }
        public double Total { 
            get { return total; } 
            set { total = value; Notify("Total"); } 
        }
        public int Table {
            get { return table; }
            set { table = value; Notify("Table"); }
        }

        //Functions
        public void AddItem(Item newItem) {
            Item TargetItem = Find(newItem.Name);
            if (TargetItem == null)
                items.Add(newItem.ShallowCopy());
            else
                TargetItem.Quantity = TargetItem.Quantity + 1;

            this.total += newItem.Price;
            Notify("Items");
        }

        public void RemoveItem(Item DeletedItem) {
            if (DeletedItem.Quantity == 1)
                items.Remove(DeletedItem);
            else
                DeletedItem.Quantity = DeletedItem.Quantity - 1;

            this.total -= DeletedItem.Price;
            Notify("Items");
        }

        public Order ShallowCopy() {
            return (Order)this.MemberwiseClone();
        }
        public void Update(Order NewOrder) {
            this.Total = NewOrder.Total;
            this.Table = NewOrder.Table;
            this.Customer = NewOrder.Customer;
            this.Items = NewOrder.Items;
        }

        public Item Find(string Query) {
            try {
                Item TargetItem = Items.Where(x => x.Name.Equals(Query)).First();
                return TargetItem;
            }catch (Exception ex) {
                Console.WriteLine("Opa A");
                return null;
            }
            
        }

        private void Notify(string name) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
