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
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Station>()
            .HasMany(s => s.Trains)
            .WithMany();

        modelBuilder.Entity<Station>()
            .HasMany(s => s.TrainsInStation)
            .WithMany();

        modelBuilder.Entity<Station>()
            .HasMany(s => s.RalwayLines)
            .WithMany();
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Train> Trains { get; set; }
    public DbSet<Station> Stations { get; set; }

    public DbSet<RailwayLine> RailwayLines { get; set; }

    IConfiguration config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", optional: false).Build();

    public void SeedData()
    {
        if (!RailwayLines.Any())
        {
            RailwayLines.AddRange(
                new RailwayLine
                {
                    Name = "Ligne Principale",
                    Color = "black"
                },
                new RailwayLine
                {
                    Name = "Ligne de Marchandises",
                    Color = "brown"
                },
                new RailwayLine
                {
                    Name = "Ligne Express",
                    Color = "red"
                },
                new RailwayLine
                {
                    Name = "Ligne de Maintenance",
                    Color = "blue"
                }
                );
            SaveChanges();
        }
        if (!Trains.Any() || !Stations.Any())
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

            List<Train> savedTrains = Trains.ToList();
            List<Train> trains = new List<Train>();
            trains.Add(savedTrains[0]);
            trains.Add(savedTrains[1]);
            List<Train> trainsInStation = new List<Train>();
            trainsInStation.Add(savedTrains[2]);
            trainsInStation.Add(savedTrains[3]);

            List<RailwayLine> savedRailwayLines = RailwayLines.ToList();


            Stations.AddRange(

               new Station
               {
                   Name = "Baie de Beauport",
                   Longitude = -71.204255,
                   Latitude = 46.842256,
                   Trains = trains,
                   TrainsInStation = trainsInStation,
                   RalwayLines = savedRailwayLines,
               },
               new Station
               {
                   Name = "Port de Québec",
                   Longitude = -71.197774,
                   Latitude = 46.823961,
                   Trains = trains,
                   TrainsInStation = trainsInStation,
                   RalwayLines = savedRailwayLines,
               },
               new Station
               {
                   Name = "Centre de distribution",
                   Longitude = -71.23208,
                   Latitude = 46.789962,
                   Trains = trains,
                   TrainsInStation = trainsInStation,
                   RalwayLines = savedRailwayLines,
               },
               new Station
               {
                   Name = "Vers Charlevoix",
                   Longitude = -71.207817,
                   Latitude = 46.845779,
                   Trains = trains,
                   TrainsInStation = trainsInStation,
                   RalwayLines = savedRailwayLines,
               },
               new Station
               {
                   Name = "Vers la Rive-Sud",
                   Longitude = -71.290278,
                   Latitude = 46.748911,
                   Trains = trains,
                   TrainsInStation = trainsInStation,
                   RalwayLines = savedRailwayLines,
               },
               new Station
               {
                   Name = "Vers Gatineau",
                   Longitude = -71.428372,
                   Latitude = 46.771591,
                   Trains = trains,
                   TrainsInStation = trainsInStation,
                   RalwayLines = savedRailwayLines,
               },
               new Station
               {
                   Name = "Vers le Nord",
                   Longitude = -71.432235,
                   Latitude = 46.765369,
                   Trains = trains,
                   TrainsInStation = trainsInStation,
                   RalwayLines = savedRailwayLines,
               },
               new Station
               {
                   Name = "Gare du Palais",
                   Longitude = -71.2139,
                   Latitude = 46.8174,
                   Trains = trains,
                   TrainsInStation = trainsInStation,
                   RalwayLines = savedRailwayLines,
               },
               new Station
               {
                   Name = "Gare Québec-Gatineau",
                   Longitude = -71.332752,
                   Latitude = 46.795569,
                   Trains = trains,
                   TrainsInStation = trainsInStation,
                   RalwayLines = savedRailwayLines,
               },
               new Station
               {
                   Name = "Gare CN",
                   Longitude = -71.303381,
                   Latitude = 46.753156,
                   Trains = trains,
                   TrainsInStation = trainsInStation,
                   RalwayLines = savedRailwayLines,
               }
           );
            SaveChanges();
        }

        IConfigurationSection sectionAdmin = config.GetSection("DefaultAdmin");

        if (!Users.Any())
        {
            List<Station> savedStations = Stations.ToList();

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
                    Station = savedStations[0],
                },
                new User
                {
                    Prenom = "Conducteur",
                    Nom = "Standard",
                    Username = "employe2",
                    Password = "employe",
                    Type = EmployeeType.Conductor,
                    Station = savedStations[1],
                },
                new User
                {
                    Prenom = "PersonnelAdminstratif",
                    Nom = "Standard",
                    Username = "employe3",
                    Password = "employe",
                    Type = EmployeeType.AdministrativeStaff,
                    Station = savedStations[2],
                },
                new User
                {
                    Prenom = "ControleurDeTrafic",
                    Nom = "Standard",
                    Username = "employe4",
                    Password = "employe",
                    Type = EmployeeType.TrafficController,
                    Station = savedStations[3],
                }
            );
            SaveChanges();
        }


       
    }
}
