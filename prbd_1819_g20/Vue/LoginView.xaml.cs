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
using System.Windows.Shapes;


namespace prbd_1819_g20.Vue
{
    /// <summary>
    /// Logique d'interaction pour LoginView.xaml
    /// </summary>
    public partial class LoginView : WindowBase
    {

        private string pseudo;
        public string Pseudo { get => pseudo; set => SetProperty<string>(ref pseudo, value, () => Validate()); }

        private string password;
        public string Password { get => password; set => SetProperty<string>(ref password, value, () => Validate()); }

        public ICommand Login { get; set; }
        public ICommand Cancel { get; set; }

        private bool LoginError = false;

        public LoginView()
        {
            InitializeComponent();
            DataContext = this;
            
            Login = new RelayCommand(LoginAction, () => { return pseudo != null && password != null && !HasErrors; });
            Cancel = new RelayCommand(() => { Password = null; Pseudo = null; });


        }

        private void LoginAction()
        {

            var user = (from u in App.Model.Users where u.UserName == Pseudo && u.Password == Password select u).FirstOrDefault(); // on recherche le membre 

            if (user != null )
            { // si aucune erreurs
                LoginError = false;
                App.CurrentUser = user; // le membre connecté devient le membre courant
                ShowMainView(); // ouverture de la fenêtre principale
                Close(); // fermeture de la fenêtre de login
            }
            else
            {
                LoginError = true;
                Password = null;
                
           }
        }

       private static void ShowMainView()
        {
            var mainWindow = new MainView();
              mainWindow.Show();
           // Application.Current.MainWindow = mainView;
        }

        public override bool Validate()
        {
            ClearErrors();
            
            if (!string.IsNullOrEmpty(Pseudo))
                if (Pseudo.Length < 3)
                {
                    AddError("Pseudo", "length must be >= 3");
                }
                
            
            if (!string.IsNullOrEmpty(Password))
                if (Password.Length < 3)
                {
                    AddError("Password", "	length must be >= 3");
                }

            if (LoginError)
            {
               AddError("Password", "connection error , check login or password !");
            }

            RaiseErrors();
            LoginError = false;
            return !HasErrors;
        }
    }
}
