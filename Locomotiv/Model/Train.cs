using System;
using System.Security.Cryptography;
public abstract class Train
{
    public int Id { get; set; }

    public TypeTrain Type { get; set; }

    public decimal Vitesse { get; set; }

    public NveauDePriorite NveauDePriorite { get; set; }

    public int Capacite { get; set; }

    public EtatTrain Etat { get; set; }
}