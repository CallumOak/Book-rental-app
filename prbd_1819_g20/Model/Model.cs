using MySql.Data.EntityFramework;
using System;
using System.Linq;
using System.Data.Entity;
using System.Collections.Generic;

namespace prbd_1819_g20 {
    public enum DbType { MsSQL, MySQL }
    public enum EFDatabaseInitMode { CreateIfNotExists, DropCreateIfChanges, DropCreateAlways }

    [DbConfigurationType(typeof(MySqlEFConfiguration))]
    public class MySqlModel : Model {
        public MySqlModel(EFDatabaseInitMode initMode) : base("name=library-mysql") {
            switch (initMode) {
                case EFDatabaseInitMode.CreateIfNotExists:
                    Database.SetInitializer<MySqlModel>(new CreateDatabaseIfNotExists<MySqlModel>());
                    break;
                case EFDatabaseInitMode.DropCreateIfChanges:
                    Database.SetInitializer<MySqlModel>(new DropCreateDatabaseIfModelChanges<MySqlModel>());
                    break;
                case EFDatabaseInitMode.DropCreateAlways:
                    Database.SetInitializer<MySqlModel>(new DropCreateDatabaseAlways<MySqlModel>());
                    break;
            }
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder) {
            base.OnModelCreating(modelBuilder);

            // see: https://blog.craigtp.co.uk/Post/2017/04/05/Entity_Framework_with_MySQL_-_Booleans,_Bits_and_%22String_was_not_recognized_as_a_valid_boolean%22_errors.
            modelBuilder.Properties<bool>().Configure(c => c.HasColumnType("bit"));
        }

        public override void Reseed(string tableName) {
            Database.ExecuteSqlCommand($"ALTER TABLE {tableName} AUTO_INCREMENT=1");
        }
    }

    public class MsSqlModel : Model {
        public MsSqlModel(EFDatabaseInitMode initMode) : base("name=library-mssql") {
            switch (initMode) {
                case EFDatabaseInitMode.CreateIfNotExists:
                    Database.SetInitializer<MsSqlModel>(new CreateDatabaseIfNotExists<MsSqlModel>());
                    break;
                case EFDatabaseInitMode.DropCreateIfChanges:
                    Database.SetInitializer<MsSqlModel>(new DropCreateDatabaseIfModelChanges<MsSqlModel>());
                    break;
                case EFDatabaseInitMode.DropCreateAlways:
                    Database.SetInitializer<MsSqlModel>(new DropCreateDatabaseAlways<MsSqlModel>());
                    break;
            }
        }

        public override void Reseed(string tableName) {
            Database.ExecuteSqlCommand($"DBCC CHECKIDENT('{tableName}', RESEED, 0)");
        }
    }

    public abstract class Model : DbContext {
        protected Model(string name) : base(name) { }

        public static Model CreateModel(DbType type, EFDatabaseInitMode initMode = EFDatabaseInitMode.DropCreateIfChanges) {
            Console.WriteLine($"Creating model for {type}\n");
            switch (type) {
                case DbType.MsSQL:
                    return new MsSqlModel(initMode);
                case DbType.MySQL:
                    return new MySqlModel(initMode);
                default:
                    throw new ApplicationException("Undefined database type");
            }

        }

        public DbSet<Book> Books { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<BookCopy> BookCopies { get; set; }
        public DbSet<RentalItem> RentalItems { get; set; }
        public DbSet<Rental> Rentals { get; set; }
        public DbSet<User> Users { get; set; }

        public void ClearDatabase() {
            foreach(Book book in Books)
            {
                Books.Remove(book);
            }
            foreach (Category category in Categories)
            {
                Categories.Remove(category);
            }
            foreach (BookCopy bookCopy in BookCopies)
            {
                BookCopies.Remove(bookCopy);
            }
            foreach (RentalItem rentalItem in RentalItems)
            {
                RentalItems.Remove(rentalItem);
            }
            foreach (Rental rental in Rentals)
            {
                Rentals.Remove(rental);
            }
            foreach (User user in Users)
            {
                Users.Remove(user);
            }
            Reseed("users");
            Reseed("books");
            Reseed("categories");
            Reseed("bookcopies");
            Reseed("rentalitems");
            Reseed("rentals");
            SaveChanges();
        }

        public void InitDatabase()
        {
            foreach(Book book in Books)
            {
                Books.Find(book.BookId);
            }
            foreach (Category category in Categories)
            {
                Categories.Find(category.CategoryId);
            }
            foreach (BookCopy bookCopy in BookCopies)
            {
                BookCopies.Find(bookCopy.BookCopyId);
            }
            foreach (RentalItem rentalItem in RentalItems)
            {
                RentalItems.Find(rentalItem.RentalItemId);
            }
            foreach (Rental rental in Rentals)
            {
                Rentals.Find(rental.RentalId);
            }
            foreach (User user in Users)
            {
                Users.Find(user.UserId);
            }
            SaveChanges();
        }

        public abstract void Reseed(string tableName);
        
        public BookCopy CreateBookCopy(DateTime acquisitionDate, Book book)
        {
            var bookCopy = BookCopies.Create();
            bookCopy.AcquisitionDate = acquisitionDate;
            bookCopy.Book = book;
            book.Copies.Add(bookCopy);
            SaveChanges();
            return bookCopy;
        }
        public RentalItem CreateRentalItem(BookCopy bookcopy, Rental rental)
        {
            var rentalItem = RentalItems.Create();
            rentalItem.ReturnDate = null;
            rentalItem.BookCopy = bookcopy;
            rentalItem.Rental = rental;
            SaveChanges();
            return rentalItem;
        }

        public  User CreateUsers(string username, string password , string fullname, string email, DateTime? birthdate,Role role)
        {
            User newUser = Users.Create();
            newUser.UserName = username;
            newUser.Password = password;
            newUser.FullName = fullname;
            newUser.Email = email;
            newUser.BirthDate = birthdate;
            newUser.Role = role;
            Users.Add(newUser);
            SaveChanges();
            return newUser;
        }
        public User CreateUsers(string username, string password, string fullname, string email, Role role)
        {
            User newUser = CreateUsers(username, password, fullname, email, null, role);
            return newUser;

         }
        public User CreateUsers(string username, string password, string fullname, string email)
        {
            User newUser = CreateUsers( username,  password,  fullname,  email, null, Role.Member);
            return newUser;

        }
       

        public Category CreateCategory(string name)
        {
            Category newCategory = Categories.Create();
            newCategory.Name = name;
            Categories.Add(newCategory);
            SaveChanges();
            return newCategory;

        }

        public Book CreateBook(string isbn, string title, string author, string editor, int numCopies)
        {
            Book newBook = Books.Create();
            newBook.Isbn = isbn;
            newBook.Title = title;
            newBook.Author = author;
            newBook.Editor = editor;
            Books.Add(newBook);
            newBook.AddCopies(numCopies, DateTime.Now);
            SaveChanges();
            return newBook;
        }

        public Role Role { get; set; }
        public List<RentalItem> ActiveRentalItems { get; }
        public int Age { get; }

        public List<Book> FindBooksByText(string v)
        {

            return (
                     from b in Books
                     where b.Title.Contains(v) 
                        || b.Author.Contains(v) 
                        || b.Editor.Contains(v) 
                        || b.Isbn.Contains(v)
                     select b
                     
                     ).ToList();

        }

        public List<RentalItem> GetActiveRentalItems()
        {
            return (
                     from ri in RentalItems
                     where ri.ReturnDate == null
                     select ri).ToList();
        }

       
    }
}
