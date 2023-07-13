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
        public IOrder Order { get; set; }
        public ObservableCollection<IItem> AvaiableItemList { get; set; }
        public ICommand AddItem { get; set; }
        public ICommand RemoveItem { get; set; }
        public IItem ItemFromMenu { get; set; }
        public IItem CanceledItem { get; set; }

        public CreateOrderDataContext(IOrder order, ObservableCollection<IItem> ItemList) {
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
