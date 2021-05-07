using PRBD_Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace prbd_1819_g20
{
    public enum Role { Admin, Manager, Member }
    public class User : EntityBase<Model>
    {
        [Key]
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public DateTime? BirthDate { get; set; }
        public Role Role { get; set; }
        public virtual ICollection<RentalItem> ActiveRentalItems { get; } = new HashSet<RentalItem>();
        public virtual ICollection<Rental> Rentals { get; set; } = new HashSet<Rental>();
        public int Age
        {
            get
            {
                return BirthDate == null
                    ? 0 : DateTime.Now.Year - ((DateTime)BirthDate).Year - DateTime.Now.Month < BirthDate.Value.Month
                    ? 1 : (DateTime.Now.Month == BirthDate.Value.Month && DateTime.Now.Day < BirthDate.Value.Day)
                    ? 1 : 0;
            }
        }
        public Rental Basket {
            get {
                return CreateBasket();
            }
        }



        public User() : base() { }

        public Rental CreateBasket()
        {
            Rental basket = (from r in Rentals
             where r.RentalDate == null && r.User.UserId == this.UserId
             select r).FirstOrDefault();
            if(basket == null)
            {
                basket = Model.Rentals.Create();
                Rentals.Add(basket);
                basket.User = this;
                this.Rentals.Add(basket);
                Model.SaveChanges();
            }
            return basket;
        }
        public RentalItem AddToBasket(Book book)
        {
            Rental basket = CreateBasket();
            var rentalItem = Model.RentalItems.Create();
            rentalItem.BookCopy = book.GetAvailableCopy();
            basket.Items.Add(rentalItem);
            rentalItem.Rental = basket;
            Model.SaveChanges();

            return rentalItem;
            
        }
        public void RemoveFromBasket(RentalItem item)
        {
            CreateBasket().RemoveItem(item);
            Model.SaveChanges();
        }
        public void ClearBasket()
        {
            CreateBasket().Clear();
            Model.SaveChanges();
        }
        public void ConfirmBasket()
        {
            CreateBasket().RentalDate = DateTime.Now;
            Model.SaveChanges();
        }

        public void Return(BookCopy copy)
        {
            RentalItem rentalItem = (from ri in ActiveRentalItems
                                     where ri.BookCopy.BookCopyId == copy.BookCopyId
                                     select ri).FirstOrDefault();
            if( rentalItem != null)
            {
                rentalItem.DoReturn();
            }
            Model.SaveChanges();
        }

        public bool IsAdmin()
        {
            return Role.ToString().Equals("Admin");
        }

        public bool IsManager()
        {
            return Role.ToString().Equals("Manager");
        }

        public bool IsMember()
        {
            return Role.ToString().Equals("Member");
        }
    }
}
