namespace ProniaWebApp.Areas.Manage.ViewModels.Product
{
	public class UpdateProductVM
	{
        public int Id { get; set; }
        public string Name { get; set; }
		public string ProductCode { get; set; }
		public string Description { get; set; }
		public double Price { get; set; }
		public int? CategoryId { get; set; }
		public List<int>? TagIds { get; set; }
		public IFormFile? MainPhoto { get; set; }
		public IFormFile? HoverPhoto { get; set; }
		public List<IFormFile>? Photos { get; set; }
		public List<ProductImagesVM>? productImages { get; set; }	
		public List<int>? ImageIds { get; set; }	

	}

	public class ProductImagesVM
	{
		public int Id { get; set; }
		public string ImgUrl { get; set; }
        public bool? IsPrime { get; set; }	
    }
}
