using RestaurantManager.Datasource;
using RestaurantManager.Interface;
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

        public ObservableCollection<IOrder> OrderList { get; private set; }

        private IOrder selectedOrder;

        private IDatabase database;

        //Commands
        public ICommand CreateOrder { get; private set; }
        public ICommand RemoveOrder { get; private set; }
        public ICommand UpdateOrder { get; private set; }
        public ICommand OpenItemManager { get; private set; }
        public ICommand AddItem { get; private set; }

        //Constructor
        public OrderManagerVM(IDatabase database) {

            /**************************************** 
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
            **************************************/

            this.database = database;
            this.OrderList = this.database.ListOrders();

            InitCommands();
        }

        public IOrder SelectedOrder {
            get { return selectedOrder; }
            set { this.selectedOrder = value; }
        }

        //Functions
        private void InitCommands() {

            //Add new order
            this.CreateOrder = new RelayCommand((object param) => {

                Order order = new Order();

                OrderForm orderForm = new OrderForm();
                orderForm.DataContext = new CreateOrderDataContext(order, database.ListItems());

                if (orderForm.ShowDialog().Equals(true)) {
                    database.CreateOrder(order);

                    UpdateOrderList();
                }
            });

            //Delete an existing order
            this.RemoveOrder = new RelayCommand((object param) => {
                database.DeleteOrder(SelectedOrder);
                UpdateOrderList();
            });

            //Update
            this.UpdateOrder = new RelayCommand((object param) => {
                OrderForm updateForm = new OrderForm();
                IOrder EditableCopy = SelectedOrder.ShallowCopy();

                updateForm.DataContext = new CreateOrderDataContext(database.ReadOrder(EditableCopy), database.ListItems()); ;
                
                if (updateForm.ShowDialog().Equals(true)) {
                    database.UpdateOrder(EditableCopy);
                    SelectedOrder.Update(EditableCopy);
                }
            });

            /* (object _) => SelectedOrder != null */

            //Open itens menu
            this.OpenItemManager = new RelayCommand((object param) => {
                ItemManagementWindow itemManagementWindow = new ItemManagementWindow();
                itemManagementWindow.DataContext = new ItemManagementVM(database);
                itemManagementWindow.ShowDialog();
            });
        }

        private void UpdateOrderList() {
            OrderList.Clear();
            
            foreach (IOrder order in database.ListOrders()) {
                OrderList.Add(order);
            }
        }
    }    
}