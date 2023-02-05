using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Rocky_Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("Category Name")]
        [Required] // Проверка на заполнение поля
        public string CategoryName { get; set; }

        [DisplayName("Display Order")]
        [Range(1,int.MaxValue,ErrorMessage ="Значение должно быть больше 0")]
        public string DisplayOrder { get; set; }
    }
}
