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
        public ObservableCollection<IItem> ItemList { get; private set; }
        private IItem selectedItem;
        private IDatabase database;

        //Commands
        public ICommand CreateItem { get; private set; }
        public ICommand RemoveItem { get; private set; }
        public ICommand UpdateItem { get; private set; }

        //Constructor
        public ItemManagementVM(IDatabase database) {
            this.database = database;

            this.ItemList = this.database.ListItems();
            InitCommands();
        }

        public IItem SelectedItem {
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
                try {
                    if (ItemForm.ShowDialog().Equals(true)) {
                        database.CreateItem(Item);
                        UpdateItemList();
                    }
                } catch (Exception) { MessageBox.Show(Message.DatabaseError()); }
            });

            //Delete an existing Item
            this.RemoveItem = new RelayCommand((object param) => {
                if(SelectedItem != null) {

                    try {

                        database.DeleteItem(SelectedItem);
                        UpdateItemList();

                    } catch (Exception) { MessageBox.Show(Message.DatabaseError()); }
                    
                }               
            });

            /* (object _) => SelectedItem != null */

            //Update
            this.UpdateItem = new RelayCommand((object param) => {
                if (SelectedItem != null) {
                    ItemForm updateForm = new ItemForm();
                    IItem EditableCopy = SelectedItem.ShallowCopy();

                    updateForm.DataContext = EditableCopy;

                    if (updateForm.ShowDialog().Equals(true)) {

                        try {
                            SelectedItem.Update(EditableCopy);
                            database.UpdateItem(EditableCopy);
                        } catch (Exception) { MessageBox.Show(Message.DatabaseError()); }

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
