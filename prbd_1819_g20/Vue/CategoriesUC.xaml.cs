using PRBD_Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace prbd_1819_g20.Vue
{
    /// <summary>
    /// Logique d'interaction pour CategoriesUC.xaml
    /// </summary>
    public partial class CategoriesUC : UserControlBase
    {
        public ICommand Add { get; set; }
        public ICommand Update { get; set; }
        public ICommand Cancel { get; set; }
        public ICommand Delete { get; set; }

        private String nameCategorie;
        public String NameCategorie { get => nameCategorie; set => SetProperty<String>(ref nameCategorie, value); }
        
        public String ButtonVisibility { get => App.CurrentUser.IsAdmin() ? "Visible" : "Hidden"; }

        public String NameEnabled { get => App.CurrentUser.IsAdmin() ? "true" : "false"; }

        private ObservableCollection<Category> categories;
        public ObservableCollection<Category> Categories
        {
            get => categories;
            set => SetProperty<ObservableCollection<Category>>(ref categories, App.Sort(value));
        }

        private Category selectedCategory;
        public Category SelectedCategory { get => selectedCategory; set => SetProperty<Category>(ref selectedCategory, value, selectedCategoryAction); }

        private void selectedCategoryAction()
        {
            if (SelectedCategory != null)
                NameCategorie = SelectedCategory.Name;
        }

        public CategoriesUC()
        {

            DataContext = this;
            Categories = new ObservableCollection<Category>(App.Model.Categories);

            Add = new RelayCommand(AddCommand, () => { return SelectedCategory != null ? isNameCategoryEditable() : isNameCategoryCorrect(); });
            Update = new RelayCommand(UpdateCommand , () => { return  isNameCategoryEditable(); });
            Cancel = new RelayCommand(CancelCommand, () => { return isNameCategoryCorrect(); });
            Delete = new RelayCommand(DeleteCommand, () => { return isNameCategoryCorrect(); });

            App.Register(this, App.PROPERTY_CHANGE_TABCONTENT.CATEGORIES_CHANGE, () => { Categories = new ObservableCollection<Category>(App.Model.Categories); });

            InitializeComponent();
                       
        }
        private bool isNameCategoryCorrect()
        {
            foreach(Category cat in Categories)
            {
                if (cat.Name.Equals(NameCategorie) && !cat.Equals(SelectedCategory))
                {
                    return false;
                }
            }
            return NameCategorie != null && NameCategorie != "";
        }
        private bool isNameCategoryEditable()
        {
            return NameCategorie != null && SelectedCategory != null && NameCategorie != "" && SelectedCategory.Name != NameCategorie;
        }


        private void AddCommand()
        {
            App.Model.CreateCategory(NameCategorie);
            Categories = new ObservableCollection<Category>(App.Model.Categories);
            SelectedCategory = null;
            NameCategorie = "";
            App.NotifyColleagues(App.PROPERTY_CHANGE_TABCONTENT.CATEGORIES_CHANGE);

        }

        private void UpdateCommand()
        {
            selectedCategory.Name = NameCategorie;
            App.Model.SaveChanges();
            Categories = new ObservableCollection<Category>(App.Model.Categories);
            App.NotifyColleagues(App.PROPERTY_CHANGE_TABCONTENT.CATEGORIES_CHANGE);
        }

        private void CancelCommand()
        {
            NameCategorie = null;
        }

        private void DeleteCommand()
        {
            
            MessageBoxResult result = System.Windows.MessageBox.Show("Do you really want to delete the category " + SelectedCategory.Name + " ? \n This action cannot be undone !!! ", "Are you sure ?", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No);

            if (result == MessageBoxResult.Yes)
            {
                SelectedCategory.Delete();
                NameCategorie = "";
                Categories = new ObservableCollection<Category>(App.Model.Categories);
                App.NotifyColleagues(App.PROPERTY_CHANGE_TABCONTENT.CATEGORIES_CHANGE);
            }
        }
    }
}
