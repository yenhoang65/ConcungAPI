namespace HuongDV.services
{
    public class Orderhelper
    {
        public static decimal ShippingFee { get; set; } = 5;

        public static Dictionary<string, string> PTThanhToan { get; } = new()
        {
            {"Tiền mặt","Tiền mặt khi giao hàng" },
            {"Paypal","paypal" },
            {"Thẻ tín dụng","Thẻ tín dụng" }
        };

        public static List<string> TinhTrangTT { get; } = new() 
        {
            "Đang chờ xử lý", "Đã chấp nhận", "Đã hủy"
        };

        public static List<string> TTDatHang { get; } = new()
        {
            "Đã tạo","Được chấp nhận","Đã vận chuyển","Đã giao hàng","Trả lại"
        };

        public static Dictionary<int, int> GetProductDictionary(string productidentifiers)
        {
            var productDictionary = new Dictionary<int, int>();

            if(productidentifiers.Length > 0)
            {
                string[] productIdArray = productidentifiers.Split('-');
                foreach(var productId in productIdArray)
                {
                    try
                    {
                        int id = int.Parse(productId);

                        if(productDictionary.ContainsKey(id))
                        {
                            productDictionary[id] += 1;
                        }
                        else
                        {
                            productDictionary.Add(id, 1);
                        }
                    }
                    catch (Exception) { }
                }
            }
            return productDictionary;
        }
    }
}
