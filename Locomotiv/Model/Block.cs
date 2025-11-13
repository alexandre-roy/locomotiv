using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Block
{
    public int Id { get; set; }

    public List<BlockPoint> Points { get; set; } = new List<BlockPoint>();
    public Train? CurrentTrain { get; set; }
}
