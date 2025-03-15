namespace MovieTicketBooking.Entities
{
    public class Movie
    {
        public int Id { get; set; }
        public string MovieName { get; set; } = "";
        public string Director { get; set; } = "";
        public string Actor { get; set; } = "";
        public int MovieTypeId { get; set; }
        public MovieType MovieType { get; set; } = null!;
    }
}
