using PRBD_Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

namespace prbd_1819_g20.Vue
{
    /// <summary>
    /// Logique d'interaction pour BooksUC.xaml
    /// </summary>
    public partial class BooksUC : UserControlBase
    {
        public String NewBookButtonVisibility { get => App.CurrentUser.IsAdmin() ? "Visible" : "Hidden"; }

        private Category currentCategory;
        public Category CurrentCategory
        {
            get => currentCategory != null ? currentCategory : All;
            set => SetProperty<Category>(ref currentCategory, value, ApplyFilterAction);
        }

        private Book selectedBook;
        public Book SelectedBook { get => selectedBook; set => SetProperty(ref selectedBook, value); }
        public Category All { get; set; }

        private string filter;
        public string Filter
        {
            get => filter;
            set => SetProperty<string>(ref filter, value, ApplyFilterAction);
        }

        public ICommand FilterCommand { get; set; }
        public ICommand AddToBasketCommand { get; set; }
        public ICommand ClearFilter { get; set; }
        public ICommand NewBook { get; set; }
        public ICommand EditBook { get; set; }

        private ObservableCollection<Book> books;
        public ObservableCollection<Book> Books
        {
            get => books;
            set => SetProperty<ObservableCollection<Book>>(ref books, App.Sort(value));
        }

        private ObservableCollection<Category> categories;
        public ObservableCollection<Category> Categories
        {
            get => categories;
            set => SetProperty<ObservableCollection<Category>>(ref categories, value);
        }
        public BooksUC()
        {
            if (DesignerProperties.GetIsInDesignMode(this)) return;
            DataContext = this;

            All = App.Model.Categories.Create();
            All.Name = "All";
            Books = new ObservableCollection<Book>(App.Model.Books);
            Categories = CategoriesWithAll();
            ClearFilter = new RelayCommand(() => { Filter = ""; CurrentCategory = All; });
            NewBook = new RelayCommand(() => { App.NotifyColleagues(App.PROPERTY_CHANGE_ADD_EDIT_BOOK.NEW_BOOK); });
            FilterCommand = new RelayCommand<Category>(FilterAction);
            AddToBasketCommand = new RelayCommand<Book>(AddToBasketAction, CheckAddToBasketAvailable);
            EditBook = new RelayCommand(EditBookAction);
            currentCategory = All;
            InitializeComponent();

            App.Register(this, App.PROPERTY_CHANGE_TABCONTENT.CATEGORIES_CHANGE, () => { var id = CurrentCategory.CategoryId; Categories = CategoriesWithAll(); CurrentCategory = App.Model.Categories.Find(id); });
            App.Register(this, App.PROPERTY_CHANGE_TABCONTENT.BASKET_CONFIRMED, () => { ApplyFilterAction(); });
            App.Register(this, App.PROPERTY_CHANGE_TABCONTENT.RENTALS_CHANGED, () => { ApplyFilterAction(); });
            App.Register(this, App.PROPERTY_CHANGE_ADD_EDIT_BOOK.SAVE_OR_DELETE, () => { ApplyFilterAction(); });
            App.Register(this, App.PROPERTY_CHANGE_TABCONTENT.BASKET_UPDATE, () => { ApplyFilterAction(); });
            App.Register(this, App.PROPERTY_CHANGE_ADD_EDIT_BOOK.SAVE_NEWBOOK, () => { ApplyFilterAction(); });
        }

        private void FilterAction(Category category)
        {
            CurrentCategory = category;
        }

        private void ApplyFilterAction()
        {
            if (CurrentCategory.Equals(All))
            {
                if (Filter == null || Filter == "")
                {
                    Books = new ObservableCollection<Book>(App.Model.Books);
                }
                else
                {
                    var query = from m in App.Model.Books
                                where
                                    m.Title.Contains(Filter) || m.Author.Contains(Filter)
                                select m;

                    Books = new ObservableCollection<Book>(query);
                }
            }
            else if (Filter == null || Filter == "")
            {
                var query = from m in App.Model.Books
                            where
                            (from c in m.Categories where c.CategoryId == currentCategory.CategoryId select c).Count() > 0
                            select m;

                Books = new ObservableCollection<Book>(query);
            }
            else
            {
                var query = from m in App.Model.Books
                            where
                            (m.Title.Contains(Filter) || m.Author.Contains(Filter)) &&
                            (from c in m.Categories where c.CategoryId == currentCategory.CategoryId select c).Count() > 0
                            select m;

                Books = new ObservableCollection<Book>(query);
            }


        }

        private void AddToBasketAction(Book book)
        {
            var copies = book.NumAvailableCopies;
            App.CurrentUser.AddToBasket(book);
            App.NotifyColleagues(App.PROPERTY_CHANGE_TABCONTENT.BASKET_UPDATE);
        }

        private bool CheckAddToBasketAvailable(Book book)
        {
            return book != null && book.NumAvailableCopies > 0;
        }

        private void EditBookAction()
        {
            if(selectedBook != null)
                App.NotifyColleagues(App.PROPERTY_CHANGE_ADD_EDIT_BOOK.EDIT_BOOK, selectedBook);
        }

        private ObservableCollection<Category> CategoriesWithAll()
        {
            return new ObservableCollection<Category>((new List<Category>() { All }).Concat(App.Model.Categories));
        }
    }
}
