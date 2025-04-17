namespace TrickyTriviaTrip.DataAccess
{
    /// <summary>
    /// Provides basic database operations for T - a specific Model class
    /// </summary>
    /// <typeparam name="T">Model class</typeparam>
    public interface IRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(int id);
    }
}
