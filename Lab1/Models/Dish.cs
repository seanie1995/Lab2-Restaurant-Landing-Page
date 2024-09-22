using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Lab1.Models
{
    public class Dish
    {    
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Name cannot be blank")]
        [DisplayName("Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Price cannot be blank")]
        [DisplayName("Price")]
        public int Price { get; set; }

        [Required(ErrorMessage = "Availability cannot be blank")]
        [DisplayName("Availability")]
        public bool Availability { get; set; }
    }
}
