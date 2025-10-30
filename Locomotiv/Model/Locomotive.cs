using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public abstract class Locomotive
{
    private int _id;
    public int Id { get; set; }

    private int _taille;
    public int Taille { get; set; }

    protected Locomotive()
    {
    }

    protected Locomotive(int id, int taille)
    {
        Id = id;
        Taille = taille;
    }
}
