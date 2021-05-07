using PRBD_Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prbd_1819_g20
{
    public class RentalItem : EntityBase<Model>
    {
        public int RentalItemId { get; set; }
        public DateTime? ReturnDate {get; set; }
        public BookCopy BookCopy { get; set; }
        public virtual Rental Rental { get; set; }

        public RentalItem() : base() { }

        public void DoReturn() {
            ReturnDate = DateTime.Now;
            Model.SaveChanges();
        }

        public void CancelReturn() {
            ReturnDate = null;
            Model.SaveChanges();
        }
    }
}
