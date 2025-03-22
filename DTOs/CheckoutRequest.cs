namespace MovieTicketBooking.DTOs
{
    public class CheckoutRequest
    {
        public int MovieId { get; set; }
        public int CinemaId { get; set; }
        public string Date { get; set; } = "";
        public string Time { get; set; } = "";
        public List<string> SeatIds { get; set; } = new();
        public string UserId { get; set; } = "";
    }

}
