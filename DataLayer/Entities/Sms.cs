namespace DataLayer.Entities
{
    public class Sms
    {
        public int Id { get; set; }
        public string Meesage { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime SendTime { get; set; }

    }
}
