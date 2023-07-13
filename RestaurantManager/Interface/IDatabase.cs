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
        ObservableCollection<IOrder> ListOrders();        
        void CreateOrder(IOrder order);
        IOrder ReadOrder(IOrder order);
        void UpdateOrder(IOrder order);
        void DeleteOrder(IOrder order);

        //Item requests
        ObservableCollection<IItem> ListItems();
        void CreateItem(IItem item);
        void UpdateItem(IItem item);
        void DeleteItem(IItem item);

    }
}
