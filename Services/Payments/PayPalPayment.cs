//namespace NewCoreProject.Services.Payments
//{
//    public class PayPalPayment : IPayment
//    {
//        public string Pay(double amount)
//        {
//            // Convert PKR to USD for PayPal (as you already do)
//            double convertedAmount = amount / 282;

//            string paypalUrl = $"https://sandbox.paypal.com/cgi-bin/webscr?cmd=_xclick" +
//                               $"&business=sb-lj439s41015447@personal.example.com" +
//                               $"&item_name=TheWayShopProducts" +
//                               $"&amount={convertedAmount}" +
//                               $"&currency_code=USD" +
//                               $"&return=https://localhost:44369/Home/OrderBooked" +
//                               $"&cancel_return=https://localhost:44369/Home/CancelOrder";

//            return paypalUrl;
//        }
//    }
//}
