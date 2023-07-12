using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using MySqlConnector;
using RestaurantManager.Interface;
using RestaurantManager.Model;

namespace RestaurantManager {
    public class MySqlDatabase: IDatabase {

        private readonly DbConnection connection;
        
        public MySqlDatabase() {
            
            //string connectionString = "Host=localhost;Username=postgres;Password=root;Database=restaurantmanager;Port=5455;";
            string connectionString = "Host=localhost;Username=root;Password=root;Database=restaurantmanager;Port=52804;";
            
            connection = new MySqlConnection(connectionString);
        }

        //Orders requests
        public ObservableCollection<Order> ListOrders() {

            ObservableCollection <Order> orders = new ObservableCollection<Order>();

            string query = "SELECT * FROM Orders";
            DbCommand command = new MySqlCommand(query, (MySqlConnection) connection);

            try {
                connection.Open();
                DbDataReader DataReader = command.ExecuteReader();

                while (DataReader.Read()) {

                    Order order = new Order {
                        Id = DataReader.GetInt32(0),
                        Customer = DataReader.GetString(1),
                        Total = double.Parse(DataReader.GetValue(2).ToString(), NumberStyles.AllowCurrencySymbol | NumberStyles.Currency),
                        Table = DataReader.GetInt32(3),
                    };

                    orders.Add(order);
                }

                connection.Close();
            } catch (Exception) { MessageBox.Show(Message.DatabaseError()); }

            return  orders;
        }

        public Order ReadOrder(Order order) {

            StringBuilder query = new StringBuilder();

            query.Append("SELECT Item.id, Item.name , Item.description, ");
            query.Append("Item.price, OrderItem.quantity ");
            query.Append("FROM Item JOIN OrderItem ON ");
            query.Append("Item.id = OrderItem.item_id ");
            query.Append("WHERE OrderItem.order_id = " +order.Id+";");

            MySqlCommand command = new MySqlCommand(query.ToString(), (MySqlConnection) connection);

            order.Items.Clear();
            order.Total = 0;

            try {
                connection.Open();
                DbDataReader DataReader = command.ExecuteReader();

                while (DataReader.Read()) {
                    Item item = new Item {
                        Id = DataReader.GetInt32(0),
                        Name = DataReader.GetString(1),
                        Description = DataReader.GetString(2),
                        Price = double.Parse(DataReader.GetValue(3).ToString(), NumberStyles.AllowCurrencySymbol | NumberStyles.Currency),
                        Quantity = DataReader.GetInt32(4)
                    };

                    order.AddItem(item);
                }
                connection.Close();
            } catch (Exception) { MessageBox.Show(Message.DatabaseError()); }

            return order;
        }

        public void DeleteOrder(Order order) {

            string query = "DELETE FROM Orders WHERE id = " + order.Id + ";";
            MySqlCommand command = new MySqlCommand(query, (MySqlConnection) connection);

            try {
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            } catch (Exception) { MessageBox.Show(Message.DatabaseError()); }
        }

        public void CreateOrder(Order order) {

            StringBuilder query = new StringBuilder();

            query.Append("INSERT INTO Orders ");
            query.Append("(customer, total, table_order, closed) ");
            query.Append("VALUES (@Customer, @Total,@Table, @Closed );");

            MySqlCommand command = new MySqlCommand(query.ToString(), (MySqlConnection) connection);

            AppendOrderParameters(command, order);

            try {
                connection.Open();
                command.ExecuteNonQuery();
                int newId = (int)command.LastInsertedId;
                order.Id = newId;
                connection.Close();
            } catch (Exception) { MessageBox.Show(Message.DatabaseError()); }

            CreateOrderItem(order);
        }

        public void UpdateOrder(Order order) {

            StringBuilder query = new StringBuilder();

            query.Append("UPDATE Orders SET ");
            query.Append("customer = @Customer, ");
            query.Append("total = @Total, ");
            query.Append("table_order = @Table ");
            query.Append("WHERE id = @Id;");

            DbCommand command = new MySqlCommand(query.ToString(), (MySqlConnection) connection);

            AppendOrderParameters((MySqlCommand)command, order);

            try {
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            } catch (Exception) { MessageBox.Show(Message.DatabaseError()); }

            CreateOrderItem(order);
        }

        private void CreateOrderItem (Order order) {
            
            StringBuilder query = new StringBuilder();

            DeleteOrderItem(order.Id);

            try {
                connection.Open();
                foreach (Item item in order.Items) {
                    query.Clear();

                    query.Append("INSERT INTO OrderItem ");
                    query.Append("(item_id, order_id, quantity) ");
                    query.Append("values (@Item, @Order, @Quantity) ");

                    MySqlCommand command = new MySqlCommand(query.ToString(), (MySqlConnection)connection);

                    command.Parameters.AddWithValue("@Item", item.Id);
                    command.Parameters.AddWithValue("@Order", order.Id);
                    command.Parameters.AddWithValue("@Quantity", item.Quantity);

                    command.ExecuteNonQuery();
                }
                connection.Close();
            } catch (Exception) { MessageBox.Show(Message.DatabaseError()); }
        }

        private void DeleteOrderItem (int OrderID) {
            
            string query = "DELETE FROM OrderItem WHERE order_id = " + OrderID + ";";
            DbCommand command = new MySqlCommand(query, (MySqlConnection)connection);

            try {
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            } catch (Exception) { MessageBox.Show(Message.DatabaseError()); }
        }

        private void AppendOrderParameters(MySqlCommand command, Order order) {
            command.Parameters.AddWithValue("@Id", order.Id);
            command.Parameters.AddWithValue("@Customer", order.Customer);
            command.Parameters.AddWithValue("@Table", order.Table);
            command.Parameters.AddWithValue("@Total", order.Total);
            command.Parameters.AddWithValue("@Closed", false);
        }

        //Itens requests
        public ObservableCollection<Item> ListItems() {

            string query = "SELECT * FROM Item";
            DbCommand command = new MySqlCommand(query, (MySqlConnection) connection);

            ObservableCollection<Item> AvaiableItems = new ObservableCollection<Item>();

            try {
                connection.Open();
                DbDataReader DataReader = command.ExecuteReader();

                while (DataReader.Read()) {

                    Item item = new Item {
                        Id = DataReader.GetInt32(0),
                        Name = DataReader.GetString(1),
                        Description = DataReader.GetString(2),
                        Price = double.Parse(DataReader.GetValue(3).ToString(), NumberStyles.AllowCurrencySymbol | NumberStyles.Currency),
                        Quantity = 1
                    };

                    AvaiableItems.Add(item);
                }

                connection.Close();

            } catch (Exception) { MessageBox.Show(Message.DatabaseError()); }

            return AvaiableItems;
        }

        public void CreateItem(Item item) {

            StringBuilder query = new StringBuilder();

            query.Append("INSERT INTO Item ");
            query.Append("(name, description, price) ");
            query.Append("VALUES(@Name, @Description, @Price)"); ;
            
            DbCommand command = new MySqlCommand(query.ToString(), (MySqlConnection) connection);

            AppendItemParameters((MySqlCommand)command, item);

            try {
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            } catch (Exception) { MessageBox.Show(Message.DatabaseError()); }
        }

        public void UpdateItem(Item item) {

            StringBuilder query = new StringBuilder();

            query.Append("UPDATE Item SET ");
            query.Append("name = @Name, ");
            query.Append("description = @Description, ");
            query.Append("price = @Price ");
            query.Append("WHERE id = @Id;");

            DbCommand command = new MySqlCommand(query.ToString(), (MySqlConnection) connection);

            AppendItemParameters((MySqlCommand) command, item);

            try {
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            } catch (Exception) { MessageBox.Show(Message.DatabaseError()); }
        }

        public void DeleteItem(Item item) {
            
            string query = "DELETE FROM Item WHERE id = " + item.Id + ";";
            DbCommand command = new MySqlCommand(query, (MySqlConnection)connection);


            try {
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            } catch (Exception) { MessageBox.Show(Message.DatabaseError()); }
        }

        private void AppendItemParameters(MySqlCommand command, Item item) {
            command.Parameters.AddWithValue("@Id", item.Id);
            command.Parameters.AddWithValue("@Name", item.Name);
            command.Parameters.AddWithValue("@Description", item.Description);
            command.Parameters.AddWithValue("@Price", item.Price);
        }
    }
}
