using Locomotiv.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Locomotiv.Model.DAL
{
    public class BlockPointDAL : IBlockPointDAL
    {
        private readonly ApplicationDbContext _context;

        public BlockPointDAL(ApplicationDbContext c)
        {
            _context = c;
        }

        /// <summary>
        /// Retrieves all block points from the data source.
        /// </summary>
        /// <returns>A list of <see cref="BlockPoint"/> objects representing all block points in the data source. The list will be empty
        /// if no block points are found.</returns>
        public IList<BlockPoint> GetAll()
        {
            return _context.BlockPoints.ToList();
        }
    }
}
