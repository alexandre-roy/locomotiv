using Locomotiv.Utils.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Locomotiv.Model;
using System.IO;

public class ApplicationDbContext : DbContext
{
    protected override void OnConfiguring(
       DbContextOptionsBuilder optionsBuilder)
    {
        // Définir le chemin absolu pour la base de données dans le répertoire AppData
        var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Locomotiv", "Locomotiv.db");
        Directory.CreateDirectory(Path.GetDirectoryName(dbPath));
        var connectionString = $"Data Source={dbPath}";

        // Configurer le DbContext pour utiliser la chaîne de connexion
        optionsBuilder.UseSqlite(connectionString);
    }
    public DbSet<User> Users { get; set; }
    public DbSet<Train> Trains { get; set; }

    IConfiguration config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", optional: false).Build();

    public void SeedData()
    {
        IConfigurationSection sectionAdmin = config.GetSection("DefaultAdmin");

        if (!Users.Any())
        {
            Users.AddRange(
                new User
                {
                    Prenom = sectionAdmin["Prenom"],
                    Nom = sectionAdmin["Nom"],
                    Username = sectionAdmin["Username"],
                    Password = sectionAdmin["Password"],
                    IsAdmin = true,
                },
                new User
                {
                    Prenom = "Mecanicien",
                    Nom = "Standard",
                    Username = "employe1",
                    Password = "employe",
                    Type = TypeEmploye.Mecanicien,
                },
                new User
                {
                    Prenom = "Conducteur",
                    Nom = "Standard",
                    Username = "employe2",
                    Password = "employe",
                    Type = TypeEmploye.Conducteur,
                },
                new User
                {
                    Prenom = "PersonnelAdminstratif",
                    Nom = "Standard",
                    Username = "employe3",
                    Password = "employe",
                    Type = TypeEmploye.PersonnelAdminstratif,
                },
                new User
                {
                    Prenom = "ControleurDeTrafic",
                    Nom = "Standard",
                    Username = "employe4",
                    Password = "employe",
                    Type = TypeEmploye.ControleurDeTrafic,
                }
            );
            SaveChanges();
        }

        if (!Trains.Any())
        {
            Trains.AddRange(
                
                new Train
                {
                    Type = TypeTrain.Maintenance,
                    Vitesse = 80,
                    NiveauDePriorite = 1,
                    Capacite = 50
                },
                new Train
                {
                    Type = TypeTrain.Marchandise,
                    Vitesse = 80,
                    NiveauDePriorite = 1,
                    Capacite = 50
                },
                new Train
                {
                    Type = TypeTrain.Passager,
                    Vitesse = 80,
                    NiveauDePriorite = 1,
                    Capacite = 50
                },
                new Train
                {
                    Type = TypeTrain.Express,
                    Vitesse = 80,
                    NiveauDePriorite = 1,
                    Capacite = 50
                }
            );
            SaveChanges();
        }

    }
}
