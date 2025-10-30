using System;
using System.Security.Cryptography;
public abstract class LigneFerroviaire
{
    private int _id;
    public int Id { get; set; }

    private string _nom;
    public string Nom { get; set; }

    private string _couleur;
    public string Couleur { get; set; }

    protected LigneFerroviaire()
    {
    }

    protected LigneFerroviaire(int id, string nom, string couleur)
    {
        Id = id;
        Nom = nom;
        Couleur = couleur;
    }
}