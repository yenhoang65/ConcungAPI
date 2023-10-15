namespace HuongDV.models
{
    public class CartDTO
    {
        public List<CartitemDTO> CartItems { get; set; } = new();
        public decimal SubTotal {  get; set; }
        public decimal ShippingFee {  get; set; }
        public decimal TotalPrice {  get; set; }
    }
}
