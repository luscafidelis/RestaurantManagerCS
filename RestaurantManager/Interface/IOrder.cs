using System.Collections.ObjectModel;
using System.ComponentModel;

namespace RestaurantManager.Model {
    public interface IOrder {
        string Customer { get; set; }
        int Id { get; set; }
        ObservableCollection<IItem> Items { get; set; }
        int Table { get; set; }
        double Total { get; set; }

        event PropertyChangedEventHandler PropertyChanged;

        void AddItem(IItem newItem);
        IItem Find(string Query);
        void RemoveItem(IItem DeletedItem);
        IOrder ShallowCopy();
        void Update(IOrder NewOrder);
    }
}