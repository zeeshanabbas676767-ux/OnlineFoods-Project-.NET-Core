//namespace NewCoreProject.Services.Payments
//{
//    public static class PaymentFactory
//    {
//        public static IPayment GetPaymentMethod(string method)
//        {
//            switch (method)
//            {
//                case "PayPal":
//                    return new PayPalPayment();
//                case "CashOnDelivery":
//                    return new CashOnDeliveryPayment();
//                // You can easily add more here:
//                // case "Stripe": return new StripePayment();
//                default:
//                    throw new ArgumentException("Invalid payment method");
//            }
//        }
//    }
//}
