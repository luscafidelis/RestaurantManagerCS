using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantManager.Model {
    public class Item : INotifyPropertyChanged, IItem {
        private string name;
        private string description;
        private double price;
        private int quantity;
        private int id;

        public event PropertyChangedEventHandler PropertyChanged;

        //Costructors
        public Item() { }

        //Getters & Setters
        public string Name {
            get { return name; }
            set { name = value; Notify("Name"); }
        }
        public string Description {
            get { return description; }
            set { description = value; Notify("Description"); }
        }
        public double Price {
            get { return price*quantity; }
            set { price = value; Notify("Price"); }
        }
        public int Quantity {
            get { return quantity; }
            set { quantity = value; Notify("Quantity"); }
        }
        public int Id {
            get { return id; }
            set { id = value; Notify("Id"); }
        }

        //Functions
        public IItem ShallowCopy() {
            return (Item) this.MemberwiseClone();
        }
        public void Update(IItem NewItem) {
            this.Description = NewItem.Description;
            this.Price = NewItem.Price;
            this.Name = NewItem.Name;
            this.Quantity = NewItem.Quantity;
            this.Id = NewItem.Id;
        }

        private void Notify(string name) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
