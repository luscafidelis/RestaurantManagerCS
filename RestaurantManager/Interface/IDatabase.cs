using RestaurantManager.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantManager.Interface {
    public interface IDatabase {

        //Order requests
        ObservableCollection<Order> ListOrders();        
        void CreateOrder(Order order);
        Order ReadOrder(Order order);
        void UpdateOrder(Order order);
        void DeleteOrder(Order order);

        //Item requests
        ObservableCollection<Item> ListItems();
        void CreateItem(Item item);
        void UpdateItem(Item item);
        void DeleteItem(Item item);

    }
}
