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
                    Type = EmployeeType.Mechanic,
                },
                new User
                {
                    Prenom = "Conducteur",
                    Nom = "Standard",
                    Username = "employe2",
                    Password = "employe",
                    Type = EmployeeType.Conductor,
                },
                new User
                {
                    Prenom = "PersonnelAdminstratif",
                    Nom = "Standard",
                    Username = "employe3",
                    Password = "employe",
                    Type = EmployeeType.AdministrativeStaff,
                },
                new User
                {
                    Prenom = "ControleurDeTrafic",
                    Nom = "Standard",
                    Username = "employe4",
                    Password = "employe",
                    Type = EmployeeType.TrafficController,
                }
            );
            SaveChanges();
        }

        if (!Trains.Any())
        {
            Trains.AddRange(
                
                new Train
                {
                    TypeOfTrain = TrainType.Maintenance,
                    Speed = 80,
                    PriotityLevel = PriorityLevel.High,
                    Capacity = 50,
                    State = TrainState.InStation
                },
                new Train
                {
                    TypeOfTrain = TrainType.Merchandise,
                    Speed = 80,
                    PriotityLevel = PriorityLevel.Medium,
                    Capacity = 50,
                    State = TrainState.Programmed
                },
                new Train
                {
                    TypeOfTrain = TrainType.Passenger,
                    Speed = 80,
                    PriotityLevel = PriorityLevel.Low,
                    Capacity = 50,
                    State = TrainState.InTransit
                },
                new Train
                {
                    TypeOfTrain = TrainType.Express,
                    Speed = 80,
                    PriotityLevel = PriorityLevel.Critical,
                    Capacity = 50,
                    State = TrainState.Idle
                }
            );
            SaveChanges();
        }

        if (!Stations.Any())
        {
            Stations.AddRange(

                new Station
                {
                    Name = "Baie de Beauport",
                    Longitude = -71.204255,
                    Latitude = 46.842256,
                    Trains = new List<Train>(),
                    TrainsInStation = new List<Train>(),
                    RalwayLines = new List<RailwayLine>(),
                    Employees = new List<User>()
                },
                new Station
                {
                    Name = "Port de Québec",
                    Longitude = -71.197774,
                    Latitude = 46.823961,
                    Trains = new List<Train>(),
                    TrainsInStation = new List<Train>(),
                    RalwayLines = new List<RailwayLine>(),
                    Employees = new List<User>()
                },
                new Station
                {
                    Name = "Centre de distribution",
                    Longitude = -71.23208,
                    Latitude = 46.789962,
                    Trains = new List<Train>(),
                    TrainsInStation = new List<Train>(),
                    RalwayLines = new List<RailwayLine>(),
                    Employees = new List<User>()
                },
                new Station
                {
                    Name = "Vers Charlevoix",
                    Longitude = -71.207817,
                    Latitude = 46.845779,
                    Trains = new List<Train>(),
                    TrainsInStation = new List<Train>(),
                    RalwayLines = new List<RailwayLine>(),
                    Employees = new List<User>()
                },
                new Station
                {
                    Name = "Vers la Rive-Sud",
                    Longitude = -71.290278,
                    Latitude = 46.748911,
                    Trains = new List<Train>(),
                    TrainsInStation = new List<Train>(),
                    RalwayLines = new List<RailwayLine>(),
                    Employees = new List<User>()
                },
                new Station
                {
                    Name = "Vers Gatineau",
                    Longitude = -71.428372,
                    Latitude = 46.771591,
                    Trains = new List<Train>(),
                    TrainsInStation = new List<Train>(),
                    RalwayLines = new List<RailwayLine>(),
                    Employees = new List<User>()
                },
                new Station
                {
                    Name = "Vers le Nord",
                    Longitude = -71.432235,
                    Latitude = 46.765369,
                    Trains = new List<Train>(),
                    TrainsInStation = new List<Train>(),
                    RalwayLines = new List<RailwayLine>(),
                    Employees = new List<User>()
                },
                new Station
                {
                    Name = "Gare du Palais",
                    Longitude = -71.2139,
                    Latitude = 46.8174,
                    Trains = new List<Train>(),
                    TrainsInStation = new List<Train>(),
                    RalwayLines = new List<RailwayLine>(),
                    Employees = new List<User>()
                },
                new Station
                {
                    Name = "Gare Québec-Gatineau",
                    Longitude = -71.332752,
                    Latitude = 46.795569,
                    Trains = new List<Train>(),
                    TrainsInStation = new List<Train>(),
                    RalwayLines = new List<RailwayLine>(),
                    Employees = new List<User>()
                },
                new Station
                {
                    Name = "Gare CN",
                    Longitude = -71.303381,
                    Latitude = 46.753156,
                    Trains = new List<Train>(),
                    TrainsInStation = new List<Train>(),
                    RalwayLines = new List<RailwayLine>(),
                    Employees = new List<User>()
                }
            );
            SaveChanges();
        }
    }
}
