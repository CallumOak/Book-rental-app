using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PRBD_Framework;

namespace prbd_1819_g20
{
    public class Category: EntityBase<Model>, IComparable<Category>, IEquatable<Category>
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public virtual ICollection<Book>Books { get; set; } = new HashSet<Book>();

        public Category() : base() { }

        public Category(string name)
        {
            Name = name;
        }

        public bool HasBook(Book book)
        {
            return Books.Contains(book);
        }
        public void AddBook(Book book)
        {
            book.Categories.Add(this);
            Books.Add(book);
            Model.SaveChanges();
        }
        public void RemoveBook(Book book)
        {
            if (Books.Contains(book))
            {
                Books.Remove(book);
                book.Categories.Remove(this);
            }
            Model.SaveChanges();
        }
        public void Delete()
        {
            while (Books.Count > 0)
            {
                Books.First().RemoveCategory(this);
            }
            
           Model.Categories.Remove(this);

           Model.SaveChanges();

        }

        public override string ToString()
        {
            return this.Name;
        }

        public int CompareTo(Category other)
        {
            return this.Name.CompareTo(other.Name);
        }

        public bool Equals(Category other)
        {
            return this.Name.Equals(other.Name);
        }
    }
}
