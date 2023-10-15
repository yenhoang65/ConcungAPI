namespace HuongDV.models
{
    public class BlogDTO
    {
        public string Title { get; set; } = "";

        public string Content { get; set; } = "";

        public IFormFile? ImageFileName { get; set; }

        //public int UserID { get; set; }
    }
}
