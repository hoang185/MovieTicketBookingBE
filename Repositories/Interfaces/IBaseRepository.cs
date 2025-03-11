namespace MovieTicketBooking.Repositories.Interfaces
{
    public interface IBaseRepository<T> where T : class
    {
        Task CreateAsync(T entity);
    }
}
