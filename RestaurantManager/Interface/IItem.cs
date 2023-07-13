using System.ComponentModel;

namespace RestaurantManager.Model {
    public interface IItem {
        string Category { get; set; }
        string Description { get; set; }
        int Id { get; set; }
        string Name { get; set; }
        double Price { get; set; }
        int Quantity { get; set; }

        event PropertyChangedEventHandler PropertyChanged;

        IItem ShallowCopy();
        void Update(IItem NewItem);
    }
}