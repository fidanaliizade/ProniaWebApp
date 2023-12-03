using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ProniaWebApp.Models
{
	public class Slider:BaseEntity
	{
		 
		[Required, StringLength(25, ErrorMessage = "The maximum length can be 25")]
		public string Title { get; set; }
		public string SubTitle { get; set; }
		public string Description { get; set; }
		[StringLength(maximumLength: 100)]
		public string? ImgUrl { get; set; }
		[NotMapped]
		public IFormFile ImageFile { get; set; }
	}
}
