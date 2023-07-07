using RestaurantManager.Model;
using RestaurantManager.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace RestaurantManager {

    public class OrderManagerVM {

        public ObservableCollection<Order> OrderList { get; private set; }
        public ObservableCollection<Item> ItemList { get; private set; }
        public Item SelectedItem { get; private set; }

        private Order selectedOrder;

        //Commands
        public ICommand CreateOrder { get; private set; }
        public ICommand RemoveOrder { get; private set; }
        public ICommand UpdateOrder { get; private set; }
        public ICommand OpenItemManager { get; private set; }
        public ICommand AddItem { get; private set; }

        //Constructor
        public OrderManagerVM() {
            this.OrderList = new ObservableCollection<Order>();
            this.ItemList = new ObservableCollection<Item>();

            ItemList.Add(new Item { 
                Name = "Pizza de calabresa",
                Description = "Pizza de calabresa",
                Category = "Comida",
                Price = 41.00,
                Quantity = 1,
            });

            ItemList.Add(new Item {
                Name = "Pizza de Frango",
                Description = "Pizza de Frango",
                Category = "Comida",
                Price = 41.00,
                Quantity = 1
            });

            InitCommands();
        }

        public Order SelectedOrder {
            get { return selectedOrder; }
            set { this.selectedOrder = value; }
        }

        //Functions
        private void InitCommands() {

            //Add new order
            this.CreateOrder = new RelayCommand((object param) => {

                Order order = new Order();

                OrderForm orderForm = new OrderForm();
                orderForm.DataContext = new CreateOrderDataContext(order, ItemList);

                if (orderForm.ShowDialog().Equals(true)) {
                    OrderList.Add(order);
                }
            });

            //Delete an existing order
            this.RemoveOrder = new RelayCommand((object param) => {
                OrderList.Remove(SelectedOrder);
            });

            //Update
            this.UpdateOrder = new RelayCommand((object param) => {
                OrderForm updateForm = new OrderForm();
                Order EditableCopy = SelectedOrder.ShallowCopy();

                updateForm.DataContext = new CreateOrderDataContext(EditableCopy, ItemList); ;
                
                if (updateForm.ShowDialog().Equals(true)) {
                    SelectedOrder.Update(EditableCopy);
                }
            });

            /* (object _) => SelectedOrder != null */

            //Open itens menu
            this.OpenItemManager = new RelayCommand((object param) => {
                ItemManagementWindow itemManagementWindow = new ItemManagementWindow();
                itemManagementWindow.DataContext = new ItemManagementVM(ItemList);
                itemManagementWindow.ShowDialog();
            });
        }
    }    
}