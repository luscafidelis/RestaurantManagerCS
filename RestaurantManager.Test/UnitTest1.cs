using Moq;
using RestaurantManager.Interface;
using RestaurantManager.Model;
using RestaurantManager.ViewModel;
using System.Collections.ObjectModel;

namespace RestaurantManager.Test {
    public class Tests {

        private IItem[] Items = {
            new Item {
                Id = 1,
                Name = "Pizza",
                Price = 32,
                Description = "Moda da casa",
                Quantity = 1,
            },
            new Item {
                Id = 2,
                Name = "Pizza de queijo",
                Price = 32,
                Description = "Muito queijo",
                Quantity = 1,
            },
            new Item {
                Id = 3,
                Name = "Pizza de calabresa",
                Price = 32,
                Description = "Muita cebola e calabresa",
                Quantity = 1,
            }
        };

        private IOrder[] Orders = {
            new Order {
                Id = 1,
                Customer = "Lucas",
                Total = 32,
                Items = new ObservableCollection<IItem> {
                    new Item {
                        Id = 2,
                        Name = "Pizza de queijo",
                        Price = 32,
                        Description = "Muito queijo",
                        Quantity = 1,
                    },
                }
            },
            new Order {
                Id = 12,
                Customer = "Lucas",
                Total = 64,
                Items = new ObservableCollection<IItem> {
                    new Item {
                        Id = 2,
                        Name = "Pizza de queijo",
                        Price = 32,
                        Description = "Muito queijo",
                        Quantity = 2,
                    },
                }
            },
        };

        [Test]
        public void FindItemInOrderTest() {
            string query = "Pizza";
            IOrder order = new Order { Id = 1 };

            Mock<IDatabase> Database = new Mock<IDatabase>();
            Database.Setup(database => database.ReadOrder(order)).Returns(
                 Orders.Where((o) => o.Id.Equals(order.Id)).First());

            order = Database.Object.ReadOrder(order);

            //testing if item is found
            IItem item = order.Find(query);

            Assert.IsNotNull(item);
        }

        [Test]
        public void ItemNotFoundInOrderTest() {
            string query = "NOTFOUND";
            IOrder order = new Order { Id = 1 };

            Mock<IDatabase> Database = new Mock<IDatabase>();
            Database.Setup(database => database.ReadOrder(order)).Returns(
                 Orders.Where((o) => o.Id.Equals(order.Id)).First());

            order = Database.Object.ReadOrder(order);

            //testing if item is found
            IItem item = order.Find(query);

            Assert.IsNull(item);
        }

        [Test]
        public void AddItemToOrderTest() {

            IOrder order = new Order { Id = 1 };
            Mock<IDatabase> Database = new Mock<IDatabase>();
            
            Database.Setup(database => database.ReadOrder(order)).Returns(
                Orders.Where((o) => o.Id.Equals(order.Id)).First());

            order = Database.Object.ReadOrder(order);
            double ExpectedTotal = order.Total;

            foreach ( var item in Items ) {
                //calcula o preço total do pedido
                ExpectedTotal = item.Price * item.Quantity + ExpectedTotal;

                order.AddItem(item);
            }

            Assert.That(condition: order.Total.Equals(ExpectedTotal));
        }

        [Test] 
        public void RemoveItemFromOrderTest() {

            IOrder order = new Order { Id = 1 };
            Mock<IDatabase> Database = new Mock<IDatabase>();

            Database.Setup(database => database.ReadOrder(order)).Returns(
                Orders.Where((o) => o.Id.Equals(order.Id)).First());

            order = Database.Object.ReadOrder(order);
            double ExpectedTotal = order.Total;

            foreach (var item in Items) {
                //calcula o preço total do pedido
                ExpectedTotal = ExpectedTotal - item.Price * item.Quantity;

                order.RemoveItem(item);
            }

            //Testing if total is the expected
            Assert.That(condition: order.Total.Equals(ExpectedTotal));
        }
        
        [Test]
        public void RemoveNonExistingItemFromOrderTest() {

            IOrder order = new Order { Id = 1 };
            Mock<IDatabase> Database = new Mock<IDatabase>();

            Database.Setup(database => database.ReadOrder(order)).Returns(
                Orders.Where((o) => o.Id.Equals(order.Id)).First());

            order = Database.Object.ReadOrder(order);
            double ExpectedTotal = order.Total;

            //Arranging testing item
            IItem item = new Item() {
                Id = 99999,
                Price = 9999,
                Name = "test",
                Quantity = 1,
            };

            order.RemoveItem(item);

            //Testing if total is the expected
            Assert.That(condition: order.Total.Equals(ExpectedTotal));
        }

        /*
            [Test]
            public void UpdateOrderList() {
                Mock<IDatabase> Database = new Mock<IDatabase>();
                OrderManagerVM orderManagerVM;

                Database.Setup(database => database.ListOrders()).Returns(
                    new ObservableCollection<IOrder>(Orders));

                orderManagerVM = new OrderManagerVM(Database.Object);
                orderManagerVM.UpdateOrderList();

                Assert.That(condition: orderManagerVM.OrderList.SequenceEqual(Database.Object.ListOrders()));
            }
        
            [Test]
            public void UpdateItemList() {
                Mock<IDatabase> Database = new Mock<IDatabase>();
                ItemManagementVM itemManagementVM;

                Database.Setup(database => database.ListItems()).Returns(
                    new ObservableCollection<IItem>(Items));

                itemManagementVM = new ItemManagementVM(Database.Object);
                itemManagementVM.UpdateItemList();

                Assert.That(condition: itemManagementVM.ItemList.SequenceEqual(Database.Object.ListItems()));
            }
        */

    }
}