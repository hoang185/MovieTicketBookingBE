namespace MovieTicketBooking.DTOs
{
    public class MovieDetailDTO
    {
        public int Id { get; set; }
        public string MovieName { get; set; } = "";
        public string ImageUrl { get; set; } = "";
        public string Rating { get; set; } = "";
        public string Director { get; set; } = "";
        public string Actor { get; set; } = "";
        public string MovieType { get; set; } = "";
    }
}
