using System;
using System.Security.Cryptography;
public abstract class Train
{
    public int Id { get; set; }

    public TypeTrain Type { get; set; }

    public decimal Vitesse { get; set; }

    public int NiveauDePriorite { get; set; }

    public int Capacite { get; set; }
}