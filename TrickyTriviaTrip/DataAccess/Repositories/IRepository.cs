namespace TrickyTriviaTrip.DataAccess
{
    /// <summary>
    /// Provides basic database operations for <typeparamref name="T"/> - a specific Model class
    /// </summary>
    /// <typeparam name="T">Model class</typeparam>
    public interface IRepository<T> where T : class
    {
        /// <summary>
        /// Gets a record from the database by its id asynchronously
        /// </summary>
        /// <param name="id">Id of the record</param>
        /// <returns>Either the <typeparamref name="T"/> object corresponding to the record, or null, if not found</returns>
        Task<T?> GetByIdAsync(int id);

        /// <summary>
        /// Gets all records from the <typeparamref name="T"/> database table
        /// </summary>
        /// <returns>A collection of <typeparamref name="T"/> objects with all <typeparamref name="T"/> records currently stored in the database</returns>
        Task<IEnumerable<T>> GetAllAsync();

        /// <summary>
        /// Adds a <typeparamref name="T"/> record to the database
        /// </summary>
        /// <param name="entity">Model object that corresponds to the record that should be added</param>
        Task AddAsync(T entity);

        /// <summary>
        /// Updates a <typeparamref name="T"/> record in the database<br/>
        /// </summary>
        /// <param name="entity">Model object that corresponds to the new state of the record being updated.<br/>
        /// The record is found by Id and all columns except Id are updated</param>
        Task UpdateAsync(T entity);

        /// <summary>
        /// Deletes a <typeparamref name="T"/> record in the database 
        /// </summary>
        /// <param name="id">The id of the record that should be deleted</param>
        Task DeleteAsync(int id);
    }
}
