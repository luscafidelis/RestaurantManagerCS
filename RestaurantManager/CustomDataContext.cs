using RestaurantManager.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RestaurantManager {
    //Custom data contexts
    public class CreateOrderDataContext {
        public Order Order { get; set; }
        public ObservableCollection<Item> AvaiableItemList { get; set; }
        public ICommand AddItem { get; set; }
        public ICommand RemoveItem { get; set; }
        public Item ItemFromMenu { get; set; }
        public Item CanceledItem { get; set; }

        public CreateOrderDataContext(Order order, ObservableCollection<Item> ItemList) {
            Order = order;
            AvaiableItemList = ItemList;

            AddItem = new RelayCommand((object param) => {
                if(ItemFromMenu != null) {
                    order.AddItem(ItemFromMenu);
                }
            });

            RemoveItem = new RelayCommand((object param) => {
                if (CanceledItem != null) {
                    order.RemoveItem(CanceledItem);
                }
            });
        }

    }
}
