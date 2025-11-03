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
    public DbSet<Station> Stations { get; set; }

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
                    Type = EmployeeType.Mecanicien,
                },
                new User
                {
                    Prenom = "Conducteur",
                    Nom = "Standard",
                    Username = "employe2",
                    Password = "employe",
                    Type = EmployeeType.Conducteur,
                },
                new User
                {
                    Prenom = "PersonnelAdminstratif",
                    Nom = "Standard",
                    Username = "employe3",
                    Password = "employe",
                    Type = EmployeeType.PersonnelAdminstratif,
                },
                new User
                {
                    Prenom = "ControleurDeTrafic",
                    Nom = "Standard",
                    Username = "employe4",
                    Password = "employe",
                    Type = EmployeeType.ControleurDeTrafic,
                }
            );
            SaveChanges();
        }

        if (!Trains.Any())
        {
            Trains.AddRange(
                
                new Train
                {
                    TypeDeTrain = TrainType.Maintenance,
                    Vitesse = 80,
                    NiveauDePriorite = PriorityLevel.Haute,
                    Capacite = 50,
                    Etat = TrainState.EnGare
                },
                new Train
                {
                    TypeDeTrain = TrainType.Marchandise,
                    Vitesse = 80,
                    NiveauDePriorite = PriorityLevel.Moyenne,
                    Capacite = 50,
                    Etat = TrainState.Programme
                },
                new Train
                {
                    TypeDeTrain = TrainType.Passager,
                    Vitesse = 80,
                    NiveauDePriorite = PriorityLevel.Faible,
                    Capacite = 50,
                    Etat = TrainState.EnTransit
                },
                new Train
                {
                    TypeDeTrain = TrainType.Express,
                    Vitesse = 80,
                    NiveauDePriorite = PriorityLevel.Critique,
                    Capacite = 50,
                    Etat = TrainState.EnAttente
                }
            );
            SaveChanges();
        }

    }
}
