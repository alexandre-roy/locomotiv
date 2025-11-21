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
    }
}
