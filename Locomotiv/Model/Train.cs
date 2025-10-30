using System;
using System.Security.Cryptography;
public abstract class Train
{
    private int _id;
    public int Id { get; set; }

    private enum _type;
    public TypeTrain Type { get; set; }

    private decimal _vitesse;
    public decimal Vitesse { get; set; }

    public int _niveauDePriorite;
    public int NiveauDePriorite { get; set; }

    public int _capacite;
    public int Capacite { get; set; }

    protected Train()
    {
    }

    protected Train(int id, TypeTrain type, decimal vitesse, int niveauDePriorite, int capacite)
    {
        Id = id;
        Type = type;
        Vitesse = vitesse;
        NiveauDePriorite = niveauDePriorite;
        Capacite = capacite;
    }
}