using RestaurantManager.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RestaurantManager.ViewModel {
    public class ItemManagementVM {
        public ObservableCollection<Item> ItemList { get; private set; }
        private Item selectedItem;

        //Commands
        public ICommand CreateItem { get; private set; }
        public ICommand RemoveItem { get; private set; }
        public ICommand UpdateItem { get; private set; }

        //Constructor
        public ItemManagementVM(ObservableCollection<Item> ItemList) {
            this.ItemList = ItemList;
            InitCommands();
        }

        public Item SelectedItem {
            get { return selectedItem; }
            set { this.selectedItem = value; }
        }

        //Functions
        private void InitCommands() {

            //Add new Item
            this.CreateItem = new RelayCommand((object param) => {

                Item Item = new Item();

                ItemForm ItemForm = new ItemForm();
                ItemForm.DataContext = Item;

                if (ItemForm.ShowDialog().Equals(true)) {
                    ItemList.Add(Item);
                }
            });

            //Delete an existing Item
            this.RemoveItem = new RelayCommand((object param) => {
                if(SelectedItem != null) {
                   ItemList.Remove(SelectedItem);
                }               
            });

            /* (object _) => SelectedItem != null */

            //Update
            this.UpdateItem = new RelayCommand((object param) => {
                if (SelectedItem != null) {
                    ItemForm updateForm = new ItemForm();
                    Item EditableCopy = SelectedItem.ShallowCopy();

                    updateForm.DataContext = EditableCopy;

                    if (updateForm.ShowDialog().Equals(true)) {
                        SelectedItem.Update(EditableCopy);
                    }
                }
            });
        }
    }
}
