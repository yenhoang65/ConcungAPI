namespace HuongDV.services
{
    public class Orderhelper
    {
        public static decimal ShippingFee { get; set; } = 5;

        public static Dictionary<string, string> PaymentMethds { get; } = new()
        {
            {"Cash","Cash on Delivery" },
            {"Paypal","paupal" },
            {"Credit Card","Credit Card" }
        };

        public static List<string> PaymentStatuses { get; } = new() 
        {
            "Pending", "Accepted", "Canceled"
        };

        public static List<string> OrderStatus { get; } = new()
        {
            "Created","Accepted"
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
