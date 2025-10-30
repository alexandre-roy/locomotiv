using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public abstract class Gare
{
    private int _id;
    public int Id { get; set; }

    private string _location;
    public string Location { get; set; }

    private List<Train> _trains;
    public List<Train> Trains { get; set; }

    private List<LigneFerroviaire> _lignesFerroviaires;
    public List<LigneFerroviaire> LignesFerroviaires { get; set; }
}
