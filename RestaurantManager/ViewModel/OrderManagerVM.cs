using Npgsql;
using RestaurantManager.Datasource;
using RestaurantManager.Interface;
using RestaurantManager.Model;
using RestaurantManager.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace RestaurantManager {

    public class OrderManagerVM {

        public ObservableCollection<IOrder> OrderList { get; private set; }

        private IOrder selectedOrder;

        private IDatabase database;

        private DbConnection connection = new NpgsqlConnection("Host = localhost; Username=postgres;Password=root;Database=restaurantmanager;Port=5455;");

        //Commands
        public ICommand CreateOrder { get; private set; }
        public ICommand RemoveOrder { get; private set; }
        public ICommand UpdateOrder { get; private set; }
        public ICommand OpenItemManager { get; private set; }
        public ICommand AddItem { get; private set; }

        //Constructor
        public OrderManagerVM() {

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

            this.database = new PgSqlDatabase((NpgsqlConnection)connection);
            try {
                this.OrderList = this.database.ListOrders();
            } catch (Exception ex) {
                MessageBox.Show(ex.Message);
            }

            InitCommands();
        }

        public OrderManagerVM(IDatabase database) {
            this.database = database;
            try {
                this.OrderList = this.database.ListOrders();
            } catch (Exception ex) {
                MessageBox.Show(ex.Message);
            }

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

                try {
                    orderForm.DataContext = new CreateOrderDataContext(order, database.ListItems());

                    if (orderForm.ShowDialog().Equals(true)) {
                        database.CreateOrder(order);
                        UpdateOrderList();
                    }
                } catch (Exception ex) { MessageBox.Show(ex.Message); }
            });

            //Delete an existing order
            this.RemoveOrder = new RelayCommand((object param) => {
                try {
                    database.DeleteOrder(SelectedOrder);
                    UpdateOrderList();
                } catch (Exception) { MessageBox.Show(Message.DatabaseError()); }
            });

            //Update
            this.UpdateOrder = new RelayCommand((object param) => {
                OrderForm updateForm = new OrderForm();
                if (SelectedOrder != null) {
                    IOrder EditableCopy = SelectedOrder.ShallowCopy();

                    try { 
                        updateForm.DataContext = new CreateOrderDataContext(database.ReadOrder(EditableCopy), database.ListItems()); ;

                        if (updateForm.ShowDialog().Equals(true)) {
                            SelectedOrder.Update(EditableCopy);
                            database.UpdateOrder(EditableCopy);
                        }
                    } catch (Exception) { MessageBox.Show(Message.DatabaseError()); }
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