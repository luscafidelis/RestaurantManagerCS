using RestaurantManager.Interface;
using RestaurantManager.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RestaurantManager.Datasource {
    public class MockDatabase : IDatabase {

        private ObservableCollection<IItem> Items;
        private ObservableCollection<IOrder> Orders;

        private int LastItemId = 0;
        private int LastOrderId = 0;


        public MockDatabase() {
            Items = new ObservableCollection<IItem>();
            Orders = new ObservableCollection<IOrder>();
        }

        public void CreateItem(IItem item) {
            //Updating ID
            LastItemId++;
            item.Id = LastItemId;
            item.Quantity = 1;

            //Adding item
            Items.Add(item);
        }

        public void CreateOrder(IOrder order) {
            LastOrderId++;
            order.Id = LastOrderId;
            Orders.Add(order);
        }

        public void DeleteItem(IItem item) {
            Items.Remove(item);
        }

        public void DeleteOrder(IOrder order) {
            Orders.Remove(order);
        }

        public ObservableCollection<IItem> ListItems() {
            return new ObservableCollection<IItem>(Items);
        }

        public ObservableCollection<IOrder> ListOrders() {
            return new ObservableCollection<IOrder>(Orders);
        }

        public IOrder ReadOrder(IOrder order) {
            return order;
        }

        public void UpdateItem(IItem item) {
            IItem TargetItem = Items.First(x => x.Id.Equals(item.Id));
            TargetItem?.Update(item);
        }

        public void UpdateOrder(IOrder order) {
            IOrder TargetOrder = Orders.First(x => x.Id.Equals(order.Id));
            TargetOrder?.Update(order);
        }
    }
}
