



using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prbd_1819_g20
{
    public class TestDatas
    {
        private readonly DbType dbType;

        private Model model;

        private User admin, ben, bruno;
        private List<User> users = new List<User>();

        private Category catInformatique, catScienceFiction, catRoman, catLitterature, catEssai;
        private List<Category> categories = new List<Category>();

        private Book book1, book2, book3;
        private List<Book> books = new List<Book>();

        public TestDatas(DbType dbType)
        {
            this.dbType = dbType;
        }

        public void Run()
        {
            using (model = Model.CreateModel(dbType))
            {
                //model.Database.Log = Console.Write;
                model.ClearDatabase(); // vide la DB et Reseed les champs auto-incrémentés
                CreateEntities(model);
                testBooks();
                testCategory();
                testBasket();
                testModel();
            }
        }

        private void CreateEntities(Model model)
        {
            Console.WriteLine("Creating test data... ");
            CreateUsers();
            CreateCategories();
            CreateBooks();
        }

        private void CreateUsers()
        {
            admin = model.CreateUsers("admin", "admin", "Administrator", "admin@test.com", role: Role.Admin);
            ben = model.CreateUsers("ben", "ben", "Benoît Penelle", "ben@test.com", new DateTime(1968, 10, 1), role: Role.Manager);
            bruno = model.CreateUsers("bruno", "bruno", "Bruno Lacroix", "bruno@test.com");
            users.AddRange(new User[] { admin, ben, bruno });
        }

        private void CreateCategories()
        {
            catInformatique = model.CreateCategory("Informatique");
            catScienceFiction = model.CreateCategory("Science Fiction");
            catRoman = model.CreateCategory("Roman");
            catLitterature = model.CreateCategory("Littérature");
            catEssai = model.CreateCategory("Essai");
            categories.AddRange(new Category[] { catInformatique, catScienceFiction, catRoman, catLitterature, catEssai });
        }

        private void CreateBooks()
        {
            book1 = model.CreateBook(
                            isbn: "123",
                            title: "Java for Dummies",
                            author: "Duchmol",
                            editor: "EPFC",
                            numCopies: 1);
            book1.PicturePath = "123.jpg";
            book2 = model.CreateBook(
                isbn: "456",
                title: "Le Seigneur des Anneaux",
                author: "Tolkien",
                editor: "Bourgeois",
                numCopies: 1);
            book2.PicturePath = "456.jpg";
            book3 = model.CreateBook(
                isbn: "789",
                title: "Les misérables",
                author: "Victor Hugo",
                editor: "XO",
                numCopies: 1);
            book3.PicturePath = "789.jpg";
            books.AddRange(new Book[] { book1, book2, book3 });
        }

        private void testBooks()
        {
            runTest("Test livres", () => {
                book1.AddCategories(new Category[] { catInformatique });
                book2.AddCategories(new Category[] { catRoman, catScienceFiction });
                book3.AddCategories(new Category[] { catRoman, catLitterature });
                printList("Books", books);
                Console.WriteLine($"book1.RemoveCategory(catInformatique) : suppression de {catInformatique}");
                book1.RemoveCategory(catInformatique);
                Debug.Assert(book1.Categories.Count == 0);
                Console.WriteLine($"book2.RemoveCategory(catEssai) : suppression de {catEssai} (inexistante dans ce livre)");
                book2.RemoveCategory(catEssai);
                printList("Books", books);
                testBookCopies();
            });
        }

        private void testBookCopies()
        {
            Console.WriteLine($"Ajout de 3 copies à book3");
            book3.AddCopies(3, new DateTime(2018, 12, 31, 17, 30, 0));
            printList<BookCopy>("book3.Copies", book3.Copies);
            Debug.Assert(book3.NumAvailableCopies == 4);
            Console.WriteLine("obtention d'une copie du book3 - BookCopy bookCopy = book3.GetAvailableCopy()");
            explicationGetAvailableCopy();
            BookCopy bookCopy = book3.GetAvailableCopy();
            Console.WriteLine($"bookCopy : {bookCopy}");
            Console.WriteLine($"suppression de bookCopy - book3.DeleteCopy(bookCopy)");
            book3.DeleteCopy(bookCopy);
            Debug.Assert(book3.NumAvailableCopies == 3);
            printList<BookCopy>("book3.Copies", book3.Copies);
        }

        private void explicationGetAvailableCopy()
        {
            Console.WriteLine("\nLa méthode book.GetAvailableCopy() retourne une copie de book qui n'est pas référencé par un RentalItem avec une date de retour à null\n");
        }

        private void testCategory()
        {
            runTest("Test Category", () => {
                printList("catEssai.Books", catEssai.Books);
                Console.WriteLine("catEssai.HasBook(book1) : " + catEssai.HasBook(book1));
                Console.WriteLine("catEssai.AddBook(book1)");
                catEssai.AddBook(book1);
                printList("catEssai.Books", catEssai.Books);
                Console.WriteLine("catEssai.RemoveBook(book1)");
                catEssai.RemoveBook(book1);
                printList("catEssai.Books", catEssai.Books);
            });
        }

        private void testBasket()
        {
            runTest("Test Basket", () =>
            {
                Console.WriteLine("Création d'un panier pour ben contenant des copies de book1, book2, book3");
                Console.WriteLine("Appels : ben.AddToBasket(book1); ben.AddToBasket(book2)");
                explicationAddToBasket();
                ben.AddToBasket(book1);
                ben.AddToBasket(book2);
                Console.WriteLine("Appel RentalItem rentalItemBook3 = ben.AddToBasket(book3); On récupère le rentalItem créé");
                RentalItem rentalItemBook3 = ben.AddToBasket(book3);
                Console.WriteLine(ben.Basket);
                printList("Rental Items du panier de ben", ben.Basket.Items);
                Console.WriteLine("Suppression d'un élément du panier de ben - ben.RemoveFromBasket(rentalItemBook3)");
                explicationRemoveFromBasket();
                ben.RemoveFromBasket(rentalItemBook3);
                printList("Rental Items du panier de ben", ben.Basket.Items);
                Console.WriteLine("Confirmation du panier de ben - basket.Confirm()");
                explicationConfirm();
                ben.Basket.Confirm();
                Console.WriteLine(ben.Basket);
                Console.WriteLine("Re-Création du panier de ben essayant d'ajouter des copies de book1, book2, book3");
                Console.WriteLine("");
                Console.WriteLine("On constate que ce ne sont pas les mêmes copies (puisque les précédentes sont déjà louées)");
                ben.AddToBasket(book1);
                ben.AddToBasket(book2);
                ben.AddToBasket(book3);
                Console.WriteLine(ben.Basket);
                printList("Rental Items du panier de ben", ben.Basket.Items);
                Console.WriteLine("Vidage du panier de ben - ben.ClearBasket()");
                ben.ClearBasket();
                Console.WriteLine(ben.Basket);
            });
        }

        private void explicationAddToBasket()
        {
            Console.WriteLine("\nLa méthode user.AddToBasket(Book book) doit :");
            Console.WriteLine("\t- obtenir le basket courant de user (ou le créer si il n'existe pas)");
            Console.WriteLine("\t- obtenir une copie disponible de book : bookCopy");
            Console.WriteLine("\t- si une copie est disponible, appeler la méthode rental.RentCopy(bookCopy)");
            explicationRentCopy();
            Console.WriteLine("\t- retourne le RentalItem créé\n");
        }

        private void explicationRentCopy()
        {
            Console.WriteLine("\n\t(La méthode rental.RentCopy(BookCopy bookCopy) crée un nouvel RentalItem et lui associe bookCopy)\n");
        }

        private void explicationRemoveFromBasket()
        {
            Console.WriteLine("\nLa méthode user.RemoveFromBasket(RentalItem rentalItem) retire rentalItem du panier (qui est un Rental) courant");
            Console.WriteLine("\tAttention, il faut que le panier existe");
            Console.WriteLine("\tFait appel à Rental.RemoveItem(rentalItem) qui retire l'item de la liste des items du Rental\n");
        }

        private void explicationConfirm()
        {
            Console.WriteLine("\nLa méthode Rental.Confirm() :");
            Console.WriteLine("\t- donne la date courante comme RentalDate au rental");
            Console.WriteLine("\t- sauvegarde le panier");
            Console.WriteLine("\t- Attention : après l'appel à cette méthode sur un panier courant, celui-ci n'existe plus (puisqu'il a été sauvegardé)");
        }

        private void explicationClearBasket()
        {
            Console.WriteLine("\nLa méthode user.ClearBasket() vide le panier courant (s'il existe)");
            Console.WriteLine("\tAttention, il faut que le panier existe");
            Console.WriteLine("\tFait appel à Rental.Clear() qui vide la liste des items du Rental\n");
        }

        private void testModel()
        {
            runTest("Test Model", () => {
                List<Book> search = model.FindBooksByText("Tolkien");
                printList("model.FindBooksByText(\"Tolkien\")", search);
                explicationFindBooksByText();
                printList("model.FindRentalItemsActive()", model.GetActiveRentalItems());
                explicationFindRentalItemsActive();
            });

        }

        private void explicationFindBooksByText()
        {
            Console.WriteLine("\nLa méthode model.FindBooksByText(str) retourne une liste de livres contenant le String 'str' dans :");
            Console.WriteLine("\tISBN, Author, Editor, Title\n");
        }

        private void explicationFindRentalItemsActive()
        {
            Console.WriteLine("\nLa méthode model.FindRentalItemsActive() retourne la liste d'items actifs, c'est à dire ceux dont ReturnDate est à null\n");
        }

        private void runTest(String title, Action action)
        {
            Console.WriteLine($"\n{ title }");
            action.Invoke();
            Console.WriteLine("--------------------------------------------------------------------------------------------------------");
        }

        private void printList<T>(String title, ICollection<T> list)
        {
            Console.WriteLine($"\n{title} :");
            String s = String.Join("\n", list);
            Console.WriteLine(s + "\n");
        }
    }
}
