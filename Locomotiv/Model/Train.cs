using System;
using System.Security.Cryptography;
public class Train
{
    public int Id { get; set; }

    public TypeTrain TypeDeTrain { get; set; }

    public decimal Vitesse { get; set; }

    public NiveauDePriorite NiveauDePriorite { get; set; }

    public int Capacite { get; set; }

    public EtatTrain Etat { get; set; }
}