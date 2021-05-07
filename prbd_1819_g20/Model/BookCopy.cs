using PRBD_Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prbd_1819_g20
{
    public class BookCopy : EntityBase<Model>
    {
        [Key]
        public int BookCopyId { get; set; }
        public DateTime AcquisitionDate { get; set; }
        public virtual ICollection<RentalItem> RentalItems { get; set; } = new HashSet<RentalItem>();
        [Required]
        public Book Book { get; set; }
        public User RentedBy
        {
            get { return (from ri in RentalItems
                         where ri.BookCopy.BookCopyId == this.BookCopyId && ri.ReturnDate is null
                          select ri.Rental.User).FirstOrDefault(); }
        }

        public BookCopy() : base()
        {}

        public BookCopy(DateTime acquisitionDate)
        {
            AcquisitionDate = acquisitionDate;
        }
    }
}
