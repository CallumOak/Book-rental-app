using PRBD_Framework;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace prbd_1819_g20.Vue
{
    /// <summary>
    /// Logique d'interaction pour RentalsUC.xaml
    /// </summary>
    public partial class RentalsUC : UserControlBase
    {

        private ObservableCollection<Rental> rentals;
        public ObservableCollection<Rental> Rentals
        {
            get => rentals;
            set => SetProperty(ref rentals, RentalsWithoutBaskets(value));
        }

        private Rental selectedRental;
        public Rental SelectedRental {
            get => selectedRental;
            set => SetProperty(ref selectedRental, value, SelectRentalAction);
        }

        private ObservableCollection<RentalItem> items;
        public ObservableCollection<RentalItem> Items
        {
            get => items;
            set => SetProperty(ref items, value);
        }

        private RentalItem selectedItem;
        public RentalItem SelectedItem {
            get;
            set;
        }

        public String ButtonVisibility
        {
            get => App.CurrentUser.IsAdmin() ? "Visible" : "Hidden";
        }

        public ICommand Return { get; set; }
        public ICommand Delete { get; set; }

        public RentalsUC()
        {
            DataContext = this;
            Rentals = new ObservableCollection<Rental>(App.Model.Rentals);
            Return = new RelayCommand<RentalItem>(ReturnAction);
            Delete = new RelayCommand<RentalItem>(DeleteAction);
            InitializeComponent();
            App.Register(this, App.PROPERTY_CHANGE_TABCONTENT.BASKET_CONFIRMED, () => { Rentals = new ObservableCollection<Rental>(App.Model.Rentals); });
            App.Register(this, App.PROPERTY_CHANGE_ADD_EDIT_BOOK.SAVE_OR_DELETE, () =>
            {
                Rentals = new ObservableCollection<Rental>(App.Model.Rentals);
                if (SelectedRental != null)
                    Items = new ObservableCollection<RentalItem>(SelectedRental.Items);
            });

        }

        private ObservableCollection<Rental> RentalsWithoutBaskets(ObservableCollection<Rental> value)
        {
            ObservableCollection<Rental> rentedRents = new ObservableCollection<Rental>();
            foreach (Rental rental in value)
            {
                if (rental.RentalDate != null)
                {
                    if(App.CurrentUser.IsAdmin() || App.CurrentUser.UserId.Equals(rental.User.UserId))
                        rentedRents.Add(rental);
                }
            }
            return rentedRents;
        }

        private void SelectRentalAction()
        {
            Items = new ObservableCollection<RentalItem>(SelectedRental.Items);
        }

        public void ReturnAction(RentalItem item)
        {
            if (App.CurrentUser.IsAdmin())
            {
                if (item.ReturnDate == null)
                {
                    item.DoReturn();
                }
                else
                {
                    item.CancelReturn();
                }
                Rentals = new ObservableCollection<Rental>(App.Model.Rentals);
                Items = new ObservableCollection<RentalItem>(SelectedRental.Items);
                App.NotifyColleagues(App.PROPERTY_CHANGE_TABCONTENT.RENTALS_CHANGED);
            }
        }

        public void DeleteAction(RentalItem item)
        {
            if (App.CurrentUser.IsAdmin())
            {
                SelectedRental.RemoveItem(item);
                SelectedItem = null;
                Rentals = new ObservableCollection<Rental>(App.Model.Rentals);
                Items = new ObservableCollection<RentalItem>(SelectedRental.Items);
                App.NotifyColleagues(App.PROPERTY_CHANGE_TABCONTENT.RENTALS_CHANGED);
            }
        }
    }
}
