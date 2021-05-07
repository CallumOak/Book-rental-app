using PRBD_Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace prbd_1819_g20.Vue
{
    /// <summary>
    /// Logique d'interaction pour App.xaml
    /// </summary>
    public partial class App : ApplicationBase
    {

        public enum PROPERTY_CHANGE_TABCONTENT { CATEGORIES_CHANGE, RENTALS_CHANGED, BASKET_CONFIRMED, BASKET_UPDATE }
        public enum PROPERTY_CHANGE_ADD_EDIT_BOOK { NEW_BOOK, SAVE_OR_DELETE, CANCEL, TITLE, EDIT_BOOK, ADD_LOCAL_BOOKCOPY,SAVE_NEWBOOK }
        private static Model model;
        public static Model Model { get { return model; } }
        public static User CurrentUser { get; set; }

        public App()
        {
            #if MSSQL
            var type = DbType.MsSQL;
            #else
            var type = DbType.MySQL;
            #endif
            model = Model.CreateModel(type);

            if (Model.Users.Count() == 0)
            {
                CreateTestData();
            }

            Model.InitDatabase();

        }

        private void CreateTestData()
        {

            Model.ClearDatabase();
            Category[] categories = new Category[] { };

            Console.Write("Creating categories data... ");

            Category lit = Model.CreateCategory("Literature");
            Category Etude = Model.CreateCategory("Etude");
            Category Science = Model.CreateCategory("Science Fiction");
            Category Fantastique = Model.CreateCategory("Fantastique");
            Category Horreur = Model.CreateCategory("Horreur");
            Category Jeunesse = Model.CreateCategory("Jeunesse");
            Category Polar = Model.CreateCategory("Polar");
            Category Romance = Model.CreateCategory("Romance");
            Category Mangas = Model.CreateCategory("Mangas");
            Category Bande = Model.CreateCategory("Bande dessinée");
            categories = new Category[] { lit, Etude, Science, Fantastique, Horreur, Jeunesse, Polar, Romance, Mangas, Bande };

            Model.SaveChanges();

            Console.WriteLine("done");
            Console.Write("Creating Books data... ");

            Book ecume = Model.CreateBook("1111111111111", "L'écume des jours", "Boris Vian", "Klett Libri", 20);
            ecume.addPicturePath("lecumedesjours.jpg");
            ecume.AddCategories(new Category[] { categories[0], categories[8] });

            Book Dracula = Model.CreateBook("2222222222222", "Dracula", "Bram Stoker", "NeoBok", 10);
            Dracula.addPicturePath("Dracula.jpg");
            Dracula.AddCategories(new Category[] { categories[0], categories[2], categories[7] });

            Book nuances = Model.CreateBook("3333333333333", "50 nuances de Grey", "James  E L", "Le Livre de Poche", 30);
            nuances.addPicturePath("50nuances.jpg");
            nuances.AddCategories(new Category[] { categories[7], categories[4] });

            Book Potter = Model.CreateBook("4444444444444", "Harry Potter et l'ordre du phénix", "Rowling, Joanne K.", "Gallimard", 10);
            Potter.addPicturePath("harrypotter4.jpg");
            Potter.AddCategories(new Category[] { categories[3], categories[5] });

            Book thanatonaute = Model.CreateBook("5555555555555", "Les Thanatonautes", "Bernard Werber", "Albin Michel", 20);
            thanatonaute.addPicturePath("Thanatonautes.jpg");
            thanatonaute.AddCategories(new Category[] { categories[0], categories[2] });

            Book anges = Model.CreateBook("6666666666666", "L'empire des anges", "Bernard Werber", "Albin Michel", 20);
            anges.addPicturePath("Anges.jpg");
            anges.AddCategories(new Category[] { categories[0], categories[2] });

            Book magicien = Model.CreateBook("7777777777777", "Le Magicien", "Raymond Feist", "Bragelonne", 30);
            magicien.addPicturePath("Magicien.jpg");
            magicien.AddCategories(new Category[] { categories[3], categories[5] });

            Book sylverthorn = Model.CreateBook("8888888888888", "Sylverthorn", "Raymond Feist", "Bragelonne", 30);
            sylverthorn.addPicturePath("Sylverthorn.jpg");
            sylverthorn.AddCategories(new Category[] { categories[3], categories[5] });

            Book sethanon = Model.CreateBook("9999999999999", "Ténebres sur Sethanon", "Raymond Feist", "Bragelonne", 30);
            sethanon.addPicturePath("Sethanon.jpg");
            sethanon.AddCategories(new Category[] { categories[3], categories[5] });


            Model.SaveChanges();

            Console.Write("Creating users data... ");

            var admin = Model.CreateUsers("admin", "admin", "administrator", "administrator@epfc.be", new DateTime(2001, 1, 1), Role.Admin);
            var callum = Model.CreateUsers("callum", "callum", "Callum", "callum@epfc.be", new DateTime(2001, 1, 1), Role.Admin);
            var luis = Model.CreateUsers("luis", "luis", "Luis", "luis@epfc.be", new DateTime(1989, 3, 8), Role.Admin);
            var georges = Model.CreateUsers("georges", "password", "Georges 1er", "georges@epfc.be", new DateTime(2000, 8, 4), Role.Member);
            var sandrine = Model.CreateUsers("sandrine", "password", "Sandrine", "sandrine@epfc.be", new DateTime(2000, 5, 21), Role.Member);
            var felicien = Model.CreateUsers("felicien", "password", "Felicien", "luisfelicien", new DateTime(2000, 8, 6), Role.Member);

            var adminBasket = admin.CreateBasket();
            adminBasket.RentCopy(ecume.GetAvailableCopy());
            adminBasket.RentCopy(Dracula.GetAvailableCopy());
            var adminBasket1 = admin.CreateBasket();
            adminBasket1.RentCopy(ecume.GetAvailableCopy());
            adminBasket1.RentCopy(Dracula.GetAvailableCopy());

            var callumBasket = callum.CreateBasket();
            callumBasket.RentCopy(Potter.GetAvailableCopy());
            callumBasket.RentCopy(Dracula.GetAvailableCopy());
            callumBasket.Confirm();
            var callumBasket1 = callum.CreateBasket();
            callumBasket1.RentCopy(Potter.GetAvailableCopy());
            callumBasket1.RentCopy(Dracula.GetAvailableCopy());

            var luisBasket = luis.CreateBasket();
            luisBasket.RentCopy(nuances.GetAvailableCopy());
            luisBasket.Confirm();
            var luisBasket1 = luis.CreateBasket();
            luisBasket1.RentCopy(nuances.GetAvailableCopy());

            var georgesBasket = georges.CreateBasket();
            georgesBasket.RentCopy(ecume.GetAvailableCopy());
            georgesBasket.RentCopy(Dracula.GetAvailableCopy());
            georgesBasket.RentCopy(Potter.GetAvailableCopy());
            georgesBasket.Confirm();
            var georgesBasket1 = georges.CreateBasket();
            georgesBasket1.RentCopy(ecume.GetAvailableCopy());
            georgesBasket1.RentCopy(Dracula.GetAvailableCopy());
            georgesBasket1.RentCopy(Potter.GetAvailableCopy());

            var sandrineBasket = sandrine.CreateBasket();
            sandrineBasket.RentCopy(ecume.GetAvailableCopy());
            sandrineBasket.RentCopy(nuances.GetAvailableCopy());
            sandrineBasket.Confirm();
            var sandrineBasket1 = sandrine.CreateBasket();
            sandrineBasket1.RentCopy(ecume.GetAvailableCopy());
            sandrineBasket1.RentCopy(nuances.GetAvailableCopy());

            var felicienBasket = felicien.CreateBasket();
            felicienBasket.RentCopy(Potter.GetAvailableCopy());
            felicienBasket.RentCopy(nuances.GetAvailableCopy());
            felicienBasket.Confirm();
            var felicienBasket1 = felicien.CreateBasket();
            felicienBasket1.RentCopy(Potter.GetAvailableCopy());
            felicienBasket1.RentCopy(nuances.GetAvailableCopy());

            Model.SaveChanges();

            Console.WriteLine("done");

        }



        public static ObservableCollection<T> Sort<T>(ObservableCollection<T> observable)
        {
            List<T> sorted = observable.OrderBy(x => x).ToList();

            int ptr = 0;
            while (ptr < sorted.Count)
            {
                if (!observable[ptr].Equals(sorted[ptr]))
                {
                    T t = observable[ptr];
                    observable.RemoveAt(ptr);
                    observable.Insert(sorted.IndexOf(t), t);
                }
                else
                {
                    ptr++;
                }
            }
            return observable;

        }

    }
}
