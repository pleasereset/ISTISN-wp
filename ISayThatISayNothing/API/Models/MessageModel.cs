
namespace ISayThatISayNothing.Models
{
    public class MessageModel
    {
        public int id { get; set; }
        public string message { get; set; }
		public string translationFr { get; set; }
        public string author { get; set; }
        public string creationdate { get; set; }
        public int nbTop { get; set; }
        public int nbFlop { get; set; }
    }
}
