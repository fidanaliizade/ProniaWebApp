namespace ProniaWebApp.Models
{
    public class Category:BaseEntity
    {
        [StringLength(maximumLength: 10, ErrorMessage ="Uzunluq max 10 ola biler!")]
        public string Name { get; set; }
        public List<Product>? Products { get; set; }
        
    }
}
