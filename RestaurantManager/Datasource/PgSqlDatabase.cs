using Npgsql;
using RestaurantManager.Interface;
using RestaurantManager.Model;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RestaurantManager.Datasource {
    public class PgSqlDatabase : IDatabase {

        private readonly DbConnection connection;

        public PgSqlDatabase() {

            string connectionString = "Host=localhost;Username=postgres;Password=root;Database=restaurantmanager;Port=5455;";
            connection = new NpgsqlConnection(connectionString);

        }

        //Orders requests
        public ObservableCollection<IOrder> ListOrders() {

            ObservableCollection<IOrder> orders = new ObservableCollection<IOrder>();

            string query = "SELECT * FROM Orders";
            DbCommand command = new NpgsqlCommand(query, (NpgsqlConnection)connection);

            connection.Open();
            DbDataReader DataReader = command.ExecuteReader();

            while (DataReader.Read()) {

                IOrder order = new Order {
                    Id = DataReader.GetInt32(0),
                    Customer = DataReader.GetString(1),
                    Total = double.Parse(DataReader.GetValue(2).ToString()),
                    Table = DataReader.GetInt32(3),
                };

                orders.Add(order);
            }
            connection.Close();            

            return orders;
        }

        public void CreateOrder(IOrder order) {

            StringBuilder query = new StringBuilder();

            query.Append("INSERT INTO \"public\".Orders ");
            query.Append("(customer, total, table_order) ");
            query.Append("VALUES (@Customer,@Total,@Table ) RETURNING id;");

            NpgsqlCommand command = new NpgsqlCommand(query.ToString(), (NpgsqlConnection)connection);
            AppendParameters(command, order);

            

            connection.Open();
            int newId = (int )command.ExecuteScalar();
            connection.Close();

            order.Id = newId;

            

            CreateOrderItem(order);
        }

        public IOrder ReadOrder(IOrder order) {

            StringBuilder query = new StringBuilder();

            query.Append("SELECT Item.id, Item.name , Item.description, ");
            query.Append("Item.price, OrderItem.quantity ");
            query.Append("FROM \"public\".Item JOIN \"public\".OrderItem ON ");
            query.Append("Item.id = OrderItem.item_id ");
            query.Append("WHERE OrderItem.order_id = " + order.Id + ";");

            NpgsqlCommand command = new NpgsqlCommand(query.ToString(), (NpgsqlConnection)connection);

            order.Items.Clear();
            order.Total = 0;

            
            connection.Open();
            DbDataReader DataReader = command.ExecuteReader();

            while (DataReader.Read()) {
                IItem item = new Item {
                    Id = DataReader.GetInt32(0),
                    Name = DataReader.GetString(1),
                    Description = DataReader.GetString(2),
                    Price = double.Parse(DataReader.GetValue(3).ToString()),
                    Quantity = DataReader.GetInt32(4)
                };

                order.AddItem(item);
            }
            connection.Close();
            

            return order;
        }

        public void UpdateOrder(IOrder order) {

            StringBuilder query = new StringBuilder();

            query.Append("UPDATE Orders SET ");
            query.Append("customer = @Customer, ");
            query.Append("total = @Total, ");
            query.Append("table_order = @Table ");
            query.Append("WHERE id = @Id;");

            DbCommand command = new NpgsqlCommand(query.ToString(), (NpgsqlConnection)connection);
            AppendParameters((NpgsqlCommand)command, order);

            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();
            
            CreateOrderItem(order);
        }

        public void DeleteOrder(IOrder order) {

            string query = "DELETE FROM Orders WHERE id = " + order.Id + ";";
            NpgsqlCommand command = new NpgsqlCommand(query, (NpgsqlConnection)connection);

            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();
            
        }

        private void AppendOrderParameters(NpgsqlCommand command, IOrder order) {
            command.Parameters.AddWithValue("@Id", order.Id);
            command.Parameters.AddWithValue("@Customer", order.Customer);
            command.Parameters.AddWithValue("@Table", order.Table);
            command.Parameters.AddWithValue("@Total", order.Total);
        }

        //IOrder item requests
        private void CreateOrderItem(IOrder order) {

            StringBuilder query = new StringBuilder();

            DeleteOrderItem(order.Id);
            
            connection.Open();
            foreach (IItem item in order.Items) {
                query.Clear();

                query.Append("INSERT INTO OrderItem ");
                query.Append("(item_id, order_id, quantity) ");
                query.Append("values (@Item, @IOrder, @Quantity) ");

                NpgsqlCommand command = new NpgsqlCommand(query.ToString(), (NpgsqlConnection)connection);

                command.Parameters.AddWithValue("@Item", item.Id);
                command.Parameters.AddWithValue("@IOrder", order.Id);
                command.Parameters.AddWithValue("@Quantity", item.Quantity);

                command.ExecuteNonQuery();
            }
            connection.Close();
            
        }

        private void DeleteOrderItem(int OrderID) {

            string query = "DELETE FROM OrderItem WHERE order_id = " + OrderID + ";";
            DbCommand command = new NpgsqlCommand(query, (NpgsqlConnection)connection);

            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();
            
        }

        //Itens requests
        public ObservableCollection<IItem> ListItems() {

            string query = "SELECT * FROM Item";
            DbCommand command = new NpgsqlCommand(query, (NpgsqlConnection)connection);

            ObservableCollection<IItem> AvaiableItems = new ObservableCollection<IItem>();

            IItem item;

            connection.Open();
            DbDataReader DataReader = command.ExecuteReader();

            while (DataReader.Read()) {

                //Create item object
                item = new Item {
                    Id = DataReader.GetInt32(0),
                    Name = DataReader.GetString(1),
                    Description = DataReader.GetString(2),
                    Price = double.Parse(DataReader.GetValue(3).ToString()),
                    Quantity = DataReader.GetInt32(4)
                };

                //Add item to item list
                AvaiableItems.Add(item);
            }

            connection.Close();
            

            return AvaiableItems;
        }

        public void CreateItem(IItem item) {

            StringBuilder query = new StringBuilder();

            query.Append("INSERT INTO Item ");
            query.Append("(name, description, price) ");
            query.Append("VALUES(@Name, @Description, @Price)"); ;

            DbCommand command = new NpgsqlCommand(query.ToString(), (NpgsqlConnection)connection);

            AppendParameters((NpgsqlCommand)command, item);

            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();

        }

        public void UpdateItem(IItem item) {

            StringBuilder query = new StringBuilder();

            query.Append("UPDATE Item SET ");
            query.Append("name = @Name, ");
            query.Append("description = @Description, ");
            query.Append("price = @Price ");
            query.Append("WHERE id = @Id;");

            DbCommand command = new NpgsqlCommand(query.ToString(), (NpgsqlConnection)connection);

            AppendParameters((NpgsqlCommand)command, item);

            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();

        }

        public void DeleteItem(IItem item) {

            string query = "DELETE FROM Item WHERE id = " + item.Id + ";";
            DbCommand command = new NpgsqlCommand(query, (NpgsqlConnection)connection);

            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();
            
        }

        private void AppendParameters(NpgsqlCommand command, object obj) {

            foreach (PropertyInfo prop in obj.GetType().GetProperties()) {
                if (!prop.Name.Equals("Items")) {
                    command.Parameters.AddWithValue($"@{prop.Name}", prop.GetValue(obj, null));
                }
            }

            /*************************************************************************
             * 
             * foreach (PropertyInfo prop in obj.GetType().GetProperties()) {
             *      MessageBox.Show($@"{prop.Name}: {prop.GetValue(obj, null)}");
             * }
             * 
             * command.Parameters.AddWithValue("@Id", item.Id);
             * command.Parameters.AddWithValue("@Name", item.Name);
             * command.Parameters.AddWithValue("@Description", item.Description);
             * command.Parameters.AddWithValue("@Price", item.Price);
             *************************************************************************/
        }
    }
}
