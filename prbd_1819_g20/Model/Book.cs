using PRBD_Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace prbd_1819_g20
{
    public class Book : EntityBase<Model>, IComparable<Book>, IEquatable<Book>
    {
        public readonly static String PICTURE_FOLDER = 
            Path.GetFullPath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/../../images\\");

        public int BookId { get; set; }
        public string Isbn { get; set; }
        public string Author { get; set; }
        public string Title { get; set; }
        public string Editor { get; set; }
        public string PicturePath { get; set; }
        public virtual ICollection<BookCopy> Copies { get; set; } = new HashSet<BookCopy>();
        public virtual ICollection<Category> Categories { get; set; } = new HashSet<Category>();

        public Book() : base() {}

        public int NumAvailableCopies
        {
            get {
                return (from bc in Model.BookCopies
                where bc.Book.BookId == this.BookId
                    && (from ri in Model.RentalItems
                       where ri.BookCopy == bc
                       && ri.ReturnDate == null
                       select ri.Rental.User).FirstOrDefault() == null
                select bc).Count(); }
        }

        public void AddCategory(Category category)
        {
            if (!Categories.Contains(category))
            {
                Model.Categories.Add(category);
                category.AddBook(this);
                Model.SaveChanges();
            }
        }

        public void RemoveCategory(Category category)
        {
            if (Categories.Contains(category))
            {
                Categories.Remove(category);
                category.Books.Remove(this);

            }
            Model.SaveChanges();
        }

        public void AddCopies(int quantity, DateTime date)
        {
            for(int i = 0; i< quantity; i++)
            {
                var bookCopy = Model.CreateBookCopy(date, this);
            }
        }

        public BookCopy GetAvailableCopy()
        {
            return (
                    from c in Model.BookCopies
                    where c.Book.BookId == BookId &&
                    (from i in c.RentalItems where i.ReturnDate == null select i).Count() == 0
                    select c
                ).FirstOrDefault();
        }

        public void DeleteCopy(BookCopy copy)
        {
            Model.BookCopies.Remove(copy);
            Model.SaveChanges();
        }

        public void Delete()
        {
            while( Copies.Count() > 0)
            {
                var copy = Copies.First();
                while(copy.RentalItems.Count() > 0)
                {
                    var ri = copy.RentalItems.First();
                    copy.RentalItems.Remove(ri);
                    ri.BookCopy = null;
                    ri.Rental.Items.Remove(ri);
                    Model.RentalItems.Remove(ri);
                }
                Copies.Remove(copy);
                copy.Book = null;
                Model.BookCopies.Remove(copy);
            }

            Model.Books.Remove(this);
            Model.SaveChanges();
        }

        public void AddCategories(Category[] category)
        {
            foreach (Category cat in category)
            {
                this.Categories.Add(cat);
                cat.AddBook(this);
            }
            
            Model.SaveChanges();
        }

        public override string ToString()
        {
            return this.Title+" "+ this.Author+" "+ this.PicturePath;
        }
        public void addPicturePath(String path)
        {
            PicturePath = PICTURE_FOLDER+path;
            Model.SaveChanges();

        }

        public int CompareTo(Book other)
        {
            return Title.CompareTo(other.Title);
        }

        public bool Equals(Book other)
        {
            return Title.Equals(other.Title);
        }
    }
}
