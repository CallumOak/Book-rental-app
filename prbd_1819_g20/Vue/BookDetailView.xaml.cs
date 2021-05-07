using Microsoft.Win32;
using prbd_1819_g20.VueModel;
using PRBD_Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
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
using Xceed.Wpf.Toolkit;

namespace prbd_1819_g20.Vue
{

    public partial class BookDetailView : UserControlBase
    {
        public ICommand Save { get; set; }
        public ICommand Cancel { get; set; }
        public ICommand Delete { get; set; }
        public ICommand LoadImage { get; set; }
        public ICommand ClearImage { get; set; }
        public ICommand AddBookCopy { get; set; }
        public ICommand AddNumCopies { get; set; }
        public ICommand SubtractNumCopies { get; set; }


        private ObservableCollection<CategoryVM> categories;
        public ObservableCollection<CategoryVM> Categories
        {
            get => categories;
            set => SetProperty<ObservableCollection<CategoryVM>>(ref categories, value);
        }

        private ObservableCollection<BookCopy> bookCopies;
        public ObservableCollection<BookCopy> BookCopies
        {
            get => bookCopies;
            set => SetProperty<ObservableCollection<BookCopy>>(ref bookCopies, value, () => { TotalCopies = BookCopies; });
        }
        private ObservableCollection<BookCopy> tempCopies;
        public ObservableCollection<BookCopy> TempCopies
        {
            get => tempCopies;
            set => SetProperty<ObservableCollection<BookCopy>>(ref tempCopies, value, UpdateTotalCopies);
        }

        private ObservableCollection<BookCopy> totalCopies;
        public ObservableCollection<BookCopy> TotalCopies
        {
            get => totalCopies;
            set => SetProperty<ObservableCollection<BookCopy>>(ref totalCopies, value);
        }

        public Book Book { get; set; }
        private ImageHelper imageHelper;
        private bool isNew;
        public bool IsNew
        {
            get { return isNew; }
            set
            {
                isNew = value;
                RaisePropertyChanged(nameof(IsNew));
            }
        }

        public string AdminView
        {
            get => App.CurrentUser.IsAdmin() ? "Visible" : "Hidden";
        }
        public bool AdminEdit
        {
            get => App.CurrentUser.IsAdmin();
        }

        private string isbn;
        public string Isbn
        {
            get => isbn;
            set => SetProperty<string>(ref isbn, value, () => { Book.Isbn = value; Validate(); });
        }

        private string author;
        public string Author
        {
            get => author;
            set => SetProperty(ref author, value, () => { Book.Author = value; Validate(); });
        }

        private string editor;
        public string Editor
        {
            get => editor;
            set => SetProperty(ref editor, value, () => { Book.Editor = value; Validate(); });
        }

        private string title;
        public string Title
        {
            get => title;
            set => SetProperty(ref title, value, () =>
            {
                Book.Title = value;
                App.NotifyColleagues(App.PROPERTY_CHANGE_ADD_EDIT_BOOK.TITLE, string.IsNullOrEmpty(value) ? "<new book>" : value);
                Validate();
            });
        }
        private string picturePath;
        public string PicturePath
        {
            get => picturePath != null ? picturePath : Book.PICTURE_FOLDER + "UnknownBook.jpg";
            set => SetProperty(ref picturePath, value, () => { Book.PicturePath = value; });
        }

        private int numberBookCopy;
        public int NumberBookCopy
        {
            get => numberBookCopy;
            set => SetProperty(ref numberBookCopy, value, () =>
            {
                if (NumberBookCopy < 0)
                {
                    NumberBookCopy = 0;
                }
            });
        }

        private DateTime aquisitionNewDate;
        public DateTime AcquisitionNewDate
        {
            get => aquisitionNewDate;
            set => SetProperty(ref aquisitionNewDate, value);
        }
        private bool bookCopiesHasChange = false;
        public BookDetailView(Book book, bool isNew)
        {
            InitializeComponent();
            DataContext = this;
            Book = book;


            Isbn = book.Isbn;
            Title = book.Title;
            Author = book.Author;
            Editor = book.Editor;
            PicturePath = Book.PicturePath;
            NumberBookCopy = 1;
            AcquisitionNewDate = DateTime.Now;

            tempCopies = new ObservableCollection<BookCopy>();
            BookCopies = new ObservableCollection<BookCopy>(book.Copies);
            IsNew = isNew;
            imageHelper = new ImageHelper(Book.PICTURE_FOLDER, Book.PicturePath);
            var catvm = new ObservableCollection<CategoryVM>();
            var cats = new ObservableCollection<Category>(App.Model.Categories);
            cats = App.Sort(cats);
            foreach (Category cat in cats)
            {
                var categoryvm = new CategoryVM(cat);
                catvm.Add(categoryvm);
            }
            foreach (CategoryVM cat in catvm)
            {
                bool hasB = cat.Category.HasBook(book);
                cat.HasBookSaved = hasB;
            }
            Categories = catvm;
            Save = new RelayCommand(SaveAction, CanSave);
            Cancel = new RelayCommand(CancelAction);
            Delete = new RelayCommand(DeleteAction, () => !IsNew);
            LoadImage = new RelayCommand(LoadImageAction);
            AddBookCopy = new RelayCommand(AddBookCopiesAction, () => NumberBookCopy > 0);
            ClearImage = new RelayCommand(ClearImageAction, () => PicturePath != null);
            AddNumCopies = new RelayCommand(AddNumCopiesAction);
            SubtractNumCopies = new RelayCommand(SubtractNumCopiesAction);

            App.Register(this, App.PROPERTY_CHANGE_TABCONTENT.BASKET_CONFIRMED, () => { BookCopies = new ObservableCollection<BookCopy>(book.Copies); });
            App.Register(this, App.PROPERTY_CHANGE_TABCONTENT.RENTALS_CHANGED, () => { BookCopies = new ObservableCollection<BookCopy>(book.Copies); });
            App.Register(this, App.PROPERTY_CHANGE_TABCONTENT.BASKET_UPDATE, () => { BookCopies = new ObservableCollection<BookCopy>(book.Copies); });

            CanSave();
            Validate();
        }

        private void AddBookCopiesAction()
        {
            var tCopies = new ObservableCollection<BookCopy>();
            for (int i = 0; i < NumberBookCopy; i++)
            {
                BookCopy bc = App.Model.BookCopies.Create();
                bc.AcquisitionDate = AcquisitionNewDate;
                tCopies.Add(bc);
            }
            foreach (BookCopy bc in TempCopies)
            {
                tCopies.Add(bc);
            }
            TempCopies = tCopies;
        }
        private void DeleteAction()
        {
            MessageBoxResult result = System.Windows.MessageBox.Show("This will remove book copies and rental items associated to "+Book.Title +" \n This action cannot be undone !!!", "Are you sure ?", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No);

            if (result == MessageBoxResult.Yes)
            {
                Book.Delete();
                App.NotifyColleagues(App.PROPERTY_CHANGE_ADD_EDIT_BOOK.SAVE_OR_DELETE);
                App.NotifyColleagues(App.PROPERTY_CHANGE_TABCONTENT.CATEGORIES_CHANGE);
            }
                

        }
        private void CancelAction()
        {
            App.NotifyColleagues(App.PROPERTY_CHANGE_ADD_EDIT_BOOK.CANCEL);

        }
        private void SaveAction()
        {
            if (IsNew)
            {
                String pictureTitle = Title.Replace(' ', '_');
                PicturePath = Book.PICTURE_FOLDER + pictureTitle + ".jpg";
                App.Model.Books.Add(Book);
                imageHelper.Confirm(pictureTitle);
            }
            foreach (CategoryVM cat in Categories)
            {
                if (cat.HasBookSaved != cat.Category.HasBook(Book))
                {
                    if (cat.HasBookSaved)
                        cat.Category.AddBook(Book);
                    else
                        cat.Category.RemoveBook(Book);
                }
            }
            while (tempCopies.Count() > 0)
            {
                var bc = tempCopies.First();
                tempCopies.Remove(bc);
                Book.AddCopies(1, bc.AcquisitionDate);
            }
            App.Model.SaveChanges();
            if (IsNew)
            {
                App.NotifyColleagues(App.PROPERTY_CHANGE_ADD_EDIT_BOOK.SAVE_NEWBOOK);
            }
            else
            {
                App.NotifyColleagues(App.PROPERTY_CHANGE_ADD_EDIT_BOOK.SAVE_OR_DELETE);
            }

            App.NotifyColleagues(App.PROPERTY_CHANGE_TABCONTENT.CATEGORIES_CHANGE);
        }
        private bool CanSave()
        {
            if (HasErrors)
            {
                return false;
            }
            if (tempCopies.Count() > 0)
            {
                return true;
            }
            if (IsNew)
            {
                return !string.IsNullOrEmpty(Title) &&
                       !string.IsNullOrEmpty(Isbn) &&
                       !string.IsNullOrEmpty(Author) &&
                       !string.IsNullOrEmpty(Editor) &&
                       !string.IsNullOrEmpty(PicturePath) &&
                       bookCopiesHasChange &&
                       !PicturePath.Equals(Book.PICTURE_FOLDER + "UnknownBook.jpg") && !HasErrors;
            }
            foreach (CategoryVM cat in Categories)
            {
                if (cat.HasBookSaved != cat.Category.HasBook(Book))
                {
                    return true;
                }
            }
            var change = (from c in App.Model.ChangeTracker.Entries<Book>()
                          where c.Entity == Book
                          select c).FirstOrDefault();

            return change != null && change.State != EntityState.Unchanged;
        }
        private void LoadImageAction()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Image Files | *.jpg";
            if (dialog.ShowDialog() == true)
            {
                imageHelper.Load(dialog.FileName);
                PicturePath = Book.PICTURE_FOLDER + imageHelper.CurrentFile;
            }
        }
        private void ClearImageAction()
        {
            imageHelper.Clear();
            PicturePath = null;
        }
        private void UpdateTotalCopies()
        {
            var tCopies = new ObservableCollection<BookCopy>(Book.Copies);
            foreach (BookCopy bc in TempCopies)
            {
                tCopies.Add(bc);
            }
            TotalCopies = tCopies;
        }
        private void AddNumCopiesAction()
        {
            NumberBookCopy++;
        }
        private void SubtractNumCopiesAction()
        {
            NumberBookCopy--;
        }
        public override bool Validate()
        {

            ClearErrors();

            if (string.IsNullOrEmpty(Isbn))
            {
                AddError("Isbn", "Isbn required !");
            }
            else if (!IsIsbnUnique())
            {
                AddError("Isbn", "Isbn must be unique !");
            }
            else if (Isbn.Length != 13)
            {
                AddError("Isbn", "Isbn must be 13 numbers, you have " + Isbn.Length + " characters !");
            }
            else if (!Int64.TryParse(isbn, out Int64 num))
            {
                AddError("Isbn", "Isbn must be numbers !");
            }

            if (string.IsNullOrEmpty(Title))
            {
                AddError("Title", "Title required !");
            }
            else if (Title.Length < 3)
            {
                AddError("Title", "Title must be minimum 3 characters");
            }

            if (string.IsNullOrEmpty(Author))
            {
                AddError("Author", "Author required !");
            }
            else if (Author.Length < 3)
            {
                AddError("Author", "Author must be minimum 3 characters");
            }

            if (string.IsNullOrEmpty(Editor))
            {
                AddError("Editor", "Editor required !");
            }
            else if (Editor.Length < 3)
            {
                AddError("Editor", "Editor must be minimum 3 characters");
            }
            RaiseErrors();

            return !HasErrors;

        }

        private bool IsIsbnUnique()
        {
            var books = (from b in App.Model.Books where b.Isbn == Isbn select b);
            foreach(Book book in books)
            {
                if(book.BookId != Book.BookId)
                {
                    return false;
                }
            }
            return true;
        }

    }
}
