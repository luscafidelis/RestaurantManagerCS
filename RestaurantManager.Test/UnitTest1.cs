using Moq;
using System.Collections.ObjectModel;

namespace RestaurantManager.Test {
    public class Tests {
        
        [Test]
        public void ListItemTest() {

            Mock<IDatabase> Database;
            ObservableCollection<IItem> Items;

            Database = new Mock<IDatabase>();

            Database.Setup(database => database.ListItems()).Returns(
                new ObservableCollection<IItem> {
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
                }
            );

            Items = Database.Object.ListItems();
            Assert.That(Items, Has.Count.EqualTo( 3 ) );
        }

        [Test]
        public void GetOrderTotal() {
            Mock<IDatabase> Database = new Mock<IDatabase>();;
            IOrder order = new Order {Id = 1 };

            object value = Database.Setup(database => database.ReadOrder(order)).Returns(               
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
                }
            );

            Item item = new Item {
                Id = 3,
                Name = "Pizza de calabresa",
                Price = 32,
                Description = "Muita cebola e calabresa",
                Quantity = 4,
            };

            order = Database.Object.ReadOrder(order);
            order.AddItem(item);

            Assert.That(condition: order.Total.Equals(32 + (item.Quantity * item.Price)));
        }
    }
}