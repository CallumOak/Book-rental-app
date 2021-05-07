using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;



using PRBD_Framework;

namespace prbd_1819_g20
{
    public class Rental : EntityBase<Model>
    {
        [Key]
        public int RentalId { get; set; }

        public DateTime? RentalDate { get; set; }

        public int NumOpenItems { get { return getOpenItem(); } }

        public virtual ICollection<RentalItem> Items { get; set; } = new HashSet<RentalItem>();

        [Required] 
        public User User { get; set; }



        public Rental() : base() { }

        public RentalItem RentCopy(BookCopy copy)
        {
            RentalItem newRentalItem =  Model.CreateRentalItem(copy, this);
            Items.Add(newRentalItem);
            Model.SaveChanges();

            return newRentalItem;
        }


        public void RemoveCopy(BookCopy copy)

        {
            
            foreach (RentalItem item in Items)
            {
                if(item.BookCopy.BookCopyId==copy.BookCopyId)

                {
                    Model.RentalItems.Remove(item);
                    Items.Remove(item);
                }
            }
            
           
            Model.SaveChanges();
        }

        public void RemoveItem(RentalItem item)
        {
            Items.Remove(item);
            Model.RentalItems.Remove(item);
            Model.SaveChanges();
        }

        public void Return(RentalItem item)
        {
            foreach (RentalItem myItem in Items)
            {
                if (myItem.RentalItemId==item.RentalItemId)  
                {
                    myItem.DoReturn();
                }

            }
            Model.SaveChanges();
        }

        public void Confirm()
        {
            RentalDate = DateTime.Now;
            Model.SaveChanges();
        }

        public void Clear()
        {
            HashSet<RentalItem> riList = new HashSet<RentalItem>();
            //La copie de la liste des items est faite car la boucle suivante modifie la liste elle même, 
            //or l'enumeration du foreach ne peut pas fonctionner si elle se modifie en cours de route
            foreach (RentalItem ri in this.Items)
            {
                riList.Add(ri);
            }
            foreach(RentalItem ri in riList)
            {
                Model.RentalItems.Remove(ri);
            }
            Model.Rentals.Remove(this);
            Model.SaveChanges();
        }

        private  int getOpenItem()
        {
            int i = 0;
            foreach (RentalItem item in Items)
            {
               i = item.ReturnDate == null ? i+1 : i ;
              
            }

            return i;
        }
    }
}
