using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public abstract class Gare
{
    public int Id { get; set; }

    public string Location { get; set; }

    public List<Train> Trains { get; set; }

    public List<Train> TrainsEnGare { get; set; }

    public List<LigneFerroviaire> LignesFerroviaires { get; set; }

    public List<User> Employes { get; set; }
}
