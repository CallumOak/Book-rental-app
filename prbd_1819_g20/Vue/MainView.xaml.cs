using PRBD_Framework;
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

namespace prbd_1819_g20.Vue
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainView : WindowBase
    {
        public ICommand Logout { get; set; }
        private TabItem BookDetailTab { get; set; }

    public MainView()
        {
            Logout = new RelayCommand(LogoutAction);
            InitializeComponent();

            App.Register(this, App.PROPERTY_CHANGE_ADD_EDIT_BOOK.NEW_BOOK, () => {
               
                var book = App.Model.Books.Create();
                AddTab(book, true);
            });

            App.Register(this, App.PROPERTY_CHANGE_ADD_EDIT_BOOK.SAVE_OR_DELETE, () => {

                RemoveTab(BookDetailTab);               
            });
            App.Register(this, App.PROPERTY_CHANGE_ADD_EDIT_BOOK.SAVE_NEWBOOK, () => {

                RemoveTab(BookDetailTab);
            });
            App.Register(this, App.PROPERTY_CHANGE_ADD_EDIT_BOOK.CANCEL, () => {

                RemoveTab(BookDetailTab);
            });
            App.Register<Book>(this, App.PROPERTY_CHANGE_ADD_EDIT_BOOK.EDIT_BOOK, (Book) => {
                               
                AddTab(Book, false);
            });

            App.Register<string>(this, App.PROPERTY_CHANGE_ADD_EDIT_BOOK.TITLE, (s) => {
                if(tabControl.SelectedIndex != 0)
                    (tabControl.SelectedItem as TabItem).Header = s;

            });
        }

        private void RemoveTab(TabItem tab)
        {
            tabControl.Items.Remove(tab);
            Dispatcher.InvokeAsync(() => tabItemBooks.Focus());
            BookDetailTab = null;
        }

        private void AddTab(Book book, bool isNew)
        {
            var ctl = new BookDetailView(book, isNew);
            BookDetailTab = new TabItem();
            
            BookDetailTab.Header = isNew ? "<new book>" : book.Title;
            BookDetailTab.Content = ctl; 
            
            // ajoute ce onglet à la liste des onglets existant du TabControl
            tabControl.Items.Add(BookDetailTab);
            // exécute la méthode Focus() de l'onglet pour lui donner le focus (càd l'activer)
            Dispatcher.InvokeAsync(() => BookDetailTab.Focus());
        }

        private void LogoutAction()
        {
            App.CurrentUser = null;
            var loginView = new LoginView();
            loginView.Show();
            Close();
        }

    }
}
