using System;

namespace Serverless.Openhack
{
    public class Rating
    {
        public string id { get; set; }
        public string userId { get; set; }
        public string productId { get; set; }
        public string locationName { get; set; }
        public int rating { get; set; }
        public string userNotes { get; set; }
        public DateTime timeStamp { get; internal set; }
    }

        

    public class Product
    {
        public string productId { get; set; }
        public string productName { get; set; }
        public string productDescription { get; set; }
    }

    public class User
    {
        public string userId { get; set; }
        public string userName { get; set; }
        public string fullName { get; set; }
    }
}