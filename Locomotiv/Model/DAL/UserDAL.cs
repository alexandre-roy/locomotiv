using System.Collections.Generic;
using System.Linq;
using Locomotiv.Model.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Locomotiv.Model.DAL
{
    public class UserDAL : IUserDAL
    {
        private readonly ApplicationDbContext _context;

        public UserDAL(ApplicationDbContext c)
        {
            _context = c;
        }
        /// <summary>
        /// Retrieves a user matching the specified username and password, including related station and train
        /// information.
        /// </summary>
        /// <remarks>The returned user includes related entities such as the user's station, trains,
        /// locomotives, and wagons. This method performs a database query and may impact performance if used
        /// frequently.</remarks>
        /// <param name="u">The username to search for. Cannot be null.</param>
        /// <param name="p">The password associated with the username. Cannot be null.</param>
        /// <returns>A <see cref="User"/> object if a matching user is found; otherwise, <see langword="null"/>.</returns>
        public User? FindByUsernameAndPassword(string u, string p)
        {
            return _context.Users
                .Include(user => user.Station)
                    .ThenInclude(s => s.Trains)
                        .ThenInclude(t => t.Locomotives)
                .Include(user => user.Station)
                    .ThenInclude(s => s.Trains)
                        .ThenInclude(t => t.Wagons)
                .Include(user => user.Station)
                    .ThenInclude(s => s.TrainsInStation)
                        .ThenInclude(t => t.Locomotives)
                .Include(user => user.Station)
                    .ThenInclude(s => s.TrainsInStation)
                        .ThenInclude(t => t.Wagons)
                .FirstOrDefault(u2 => u2.Username == u && u2.Password == p);
        }
    }
}
