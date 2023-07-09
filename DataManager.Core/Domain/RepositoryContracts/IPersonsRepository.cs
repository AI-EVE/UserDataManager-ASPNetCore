using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Entities;

namespace RepositoryContracts
{
    /// <summary>
    /// Represents business logic for manipulating Person entity
    /// </summary>
    public interface IPersonsRepository
    {
        /// <summary>
        /// Addds a new person into the DB
        /// </summary>
        /// <param name="person">Person to add</param>
        /// <returns>Returns the person after adding it to the DB</returns>
        Task<Person> AddPerson(Person? person);


        /// <summary>
        /// Returns all persons from the DB
        /// </summary>
        /// <returns>Returns a list of objects of Person type</returns>
        Task<List<Person>> GetAllPersons();

        /// <summary>
        /// Returns the person object based on the given person id
        /// </summary>
        /// <param name="personID">Person id to search</param>
        /// <returns>Returns matching person object</returns>
        Task<Person?> GetPersonByPersonID(Guid? personID);

        /// <summary>
        /// searches the DB for the wanted object based on the given predicate
        /// </summary>
        /// <param name="Predicate">The condition</param>
        /// <returns>a list of persons that match with the condition</returns>
        Task<List<Person>> GetFilteredPersons(Expression<Func<Person, bool>> Predicate);

        /// <summary>
        /// Updates the specified person details based on the given person ID
        /// </summary>
        /// <param name="person">Person details to update, including person id</param>
        /// <returns>Returns the person object after updation</returns>
        Task<Person> UpdatePerson(Person person);


        /// <summary>
        /// Deletes a person based on the given person id
        /// </summary>
        /// <param name="PersonID">PersonID to delete</param>
        /// <returns>Returns true, if the deletion is successful; otherwise false</returns>
        Task<bool> DeletePerson(Guid? personID);
    }
}
