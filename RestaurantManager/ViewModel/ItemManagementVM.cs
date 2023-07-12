using RestaurantManager.Interface;
using RestaurantManager.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace RestaurantManager.ViewModel {
    public class ItemManagementVM {
        public ObservableCollection<Item> ItemList { get; private set; }
        private Item selectedItem;
        private IDatabase database;

        //Commands
        public ICommand CreateItem { get; private set; }
        public ICommand RemoveItem { get; private set; }
        public ICommand UpdateItem { get; private set; }

        //Constructor
        public ItemManagementVM(IDatabase database) {
            this.database = database;
            this.ItemList = database.ListItems();

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
                    database.CreateItem(Item);
                    UpdateItemList();
                }
            });

            //Delete an existing Item
            this.RemoveItem = new RelayCommand((object param) => {
                if(SelectedItem != null) {
                    database.DeleteItem(SelectedItem);
                    UpdateItemList();
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
                        database.UpdateItem(EditableCopy);
                    }
                }
            });
        }

        private void UpdateItemList() {
            ItemList.Clear();

            foreach (Item item in database.ListItems()) {
                ItemList.Add(item);
            }
        }
    }
}
