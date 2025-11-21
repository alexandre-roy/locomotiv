using Locomotiv.Model.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Locomotiv.Model.DAL
{
    public class BlockDAL : IBlockDAL
    {
        private readonly ApplicationDbContext _context;
        /// <summary>
        /// 
        /// </summary>  
        /// <param name="c"></param>
        public BlockDAL(ApplicationDbContext c)
        {
            _context = c;
        }


        public IList<Block> GetAll()
        {
            return _context.Blocks
                .Include(b => b.Points)
                .Include(b => b.CurrentTrain)
                .ToList();
        }

        /// <summary>
        /// Retrieves all trains that are currently positioned on blocks.
        /// </summary>
        /// <returns>A list of trains currently on blocks. Returns an empty list if no trains are on blocks.</returns>
        public IList<Train> GetTrainsCurrentlyOnBlocks()
        {
            return _context.Blocks
                .Where(b => b.CurrentTrain != null)
                .Select(b => b.CurrentTrain)
                .ToList();
        }
    }
}
