using PRBD_Framework;
using System.Collections.ObjectModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;

namespace prbd_1819_g20.Vue
{
    /// <summary>
    /// Logique d'interaction pour Basket.xaml
    /// </summary>
    public partial class BasketUC : UserControlBase
    {
        public ICommand Delete { get; set; }
        public ICommand Confirm { get; set; }
        public ICommand Clear { get; set; }

        public String ComboboxVisibility
        {
            get => App.CurrentUser.IsAdmin() ? "Visible" : "Hidden";
        }

        public ObservableCollection<User> Users
        {
            get => new ObservableCollection<User>(App.Model.Users);
        }

        private RentalItem selectedItem;
        public RentalItem SelectedItem { get => selectedItem; set => SetProperty(ref selectedItem, value); }

        private User selectedUser;
        public User SelectedUser
        {
            get => selectedUser;
            set => SetProperty(ref selectedUser, value, ChangeSelectedUserAction);
        }

        private ObservableCollection<RentalItem> basket;
        public ObservableCollection<RentalItem> Basket
        {
            get => basket;
            set => SetProperty(ref basket, value);
        }
        public BasketUC()
        {
            if (DesignerProperties.GetIsInDesignMode(this)) return;
            DataContext = this;

            SelectedUser = App.CurrentUser;
            Basket = new ObservableCollection<RentalItem>( SelectedUser.Basket.Items);
            Delete = new RelayCommand(DeleteAction);
            Confirm = new RelayCommand(ConfirmAction);
            Clear = new RelayCommand(ClearAction);
            InitializeComponent();
            App.Register(this, App.PROPERTY_CHANGE_TABCONTENT.BASKET_UPDATE, () => { Basket = new ObservableCollection<RentalItem>(SelectedUser.Basket.Items); });
            App.Register(this, App.PROPERTY_CHANGE_ADD_EDIT_BOOK.SAVE_OR_DELETE, () => { Basket = new ObservableCollection<RentalItem>(SelectedUser.Basket.Items); });
            App.Register(this, App.PROPERTY_CHANGE_ADD_EDIT_BOOK.SAVE_NEWBOOK, () => { Basket = new ObservableCollection<RentalItem>(SelectedUser.Basket.Items); });

        }

        private void ChangeSelectedUserAction()
        {
            Basket = new ObservableCollection<RentalItem>(SelectedUser.Basket.Items);
        }

        private void DeleteAction()
        {
            if (SelectedItem != null)
                SelectedUser.Basket.RemoveItem(SelectedItem);
            SelectedItem = null;
            Basket = new ObservableCollection<RentalItem>(SelectedUser.Basket.Items);
            App.NotifyColleagues(App.PROPERTY_CHANGE_TABCONTENT.BASKET_UPDATE);
        }

        private void ConfirmAction()
        {
            SelectedUser.Basket.Confirm();
            Basket = new ObservableCollection<RentalItem>(SelectedUser.Basket.Items);
            App.NotifyColleagues(App.PROPERTY_CHANGE_TABCONTENT.BASKET_CONFIRMED);
        }

        private void ClearAction()
        {
            SelectedUser.Basket.Clear();
            Basket = new ObservableCollection<RentalItem>(SelectedUser.Basket.Items);
            App.NotifyColleagues(App.PROPERTY_CHANGE_TABCONTENT.BASKET_UPDATE);
        }
    }
}
