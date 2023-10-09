namespace HuongDV.models
{
    public class Product
    {
        public int Id { get; set; }

        public string Name { get; set; } = "";

        public string Brand { get; set; } = "";

        public string DanhMuc { get; set; } = "";

        public string Gia { get; set; }

        public string MoTa { get; set; } = "";

        public string AnhSP { get; set; } = "";

        public DateTime CreatedAt { get; set; } = DateTime.Now;

    }
}
