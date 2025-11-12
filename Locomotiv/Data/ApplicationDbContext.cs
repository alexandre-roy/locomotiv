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
    public DbSet<Locomotive> Locomotives { get; set; }
    public DbSet<Wagon> Wagons { get; set; }
    public DbSet<Train> Trains { get; set; }
    public DbSet<Station> Stations { get; set; }
    public DbSet<Block> Blocks { get; set; }

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

        if (!Locomotives.Any())
        {
            Locomotives.AddRange(
                new Locomotive { Code = "Loco-001" },
                new Locomotive { Code = "Loco-002" },
                new Locomotive { Code = "Loco-003" },
                new Locomotive { Code = "Loco-004" },
                new Locomotive { Code = "Loco-005" },
                new Locomotive { Code = "Loco-006" },
                new Locomotive { Code = "Loco-007" },
                new Locomotive { Code = "Loco-008" },
                new Locomotive { Code = "Loco-009" },
                new Locomotive { Code = "Loco-010" }
            );
            SaveChanges();
        }

        if (!Wagons.Any())
        {
            Wagons.AddRange(
                new Wagon { Code = "Wagon-001" },
                new Wagon { Code = "Wagon-002" },
                new Wagon { Code = "Wagon-003" },
                new Wagon { Code = "Wagon-004" },
                new Wagon { Code = "Wagon-005" },
                new Wagon { Code = "Wagon-006" },
                new Wagon { Code = "Wagon-007" },
                new Wagon { Code = "Wagon-008" },
                new Wagon { Code = "Wagon-009" },
                new Wagon { Code = "Wagon-010" },
                new Wagon { Code = "Wagon-011" },
                new Wagon { Code = "Wagon-012" },
                new Wagon { Code = "Wagon-013" },
                new Wagon { Code = "Wagon-014" },
                new Wagon { Code = "Wagon-015" },
                new Wagon { Code = "Wagon-016" },
                new Wagon { Code = "Wagon-017" },
                new Wagon { Code = "Wagon-018" },
                new Wagon { Code = "Wagon-019" },
                new Wagon { Code = "Wagon-020" },
                new Wagon { Code = "Wagon-021" },
                new Wagon { Code = "Wagon-022" },
                new Wagon { Code = "Wagon-023" },
                new Wagon { Code = "Wagon-024" },
                new Wagon { Code = "Wagon-025" },
                new Wagon { Code = "Wagon-026" },
                new Wagon { Code = "Wagon-027" },
                new Wagon { Code = "Wagon-028" },
                new Wagon { Code = "Wagon-029" },
                new Wagon { Code = "Wagon-030" },
                new Wagon { Code = "Wagon-031" },
                new Wagon { Code = "Wagon-032" },
                new Wagon { Code = "Wagon-033" },
                new Wagon { Code = "Wagon-034" },
                new Wagon { Code = "Wagon-035" },
                new Wagon { Code = "Wagon-036" },
                new Wagon { Code = "Wagon-037" },
                new Wagon { Code = "Wagon-038" },
                new Wagon { Code = "Wagon-039" },
                new Wagon { Code = "Wagon-040" }
            );
            SaveChanges();
        }

        if (!Trains.Any())
        {
            Trains.AddRange(
                new Train
                {
                    TypeOfTrain = TrainType.Maintenance,
                    PriotityLevel = PriorityLevel.Low,
                    State = TrainState.Idle,
                    Wagons = new List<Wagon>(),
                    Locomotives = new List<Locomotive>()
                },
                new Train
                {
                    TypeOfTrain = TrainType.Merchandise,
                    PriotityLevel = PriorityLevel.Medium,
                    State = TrainState.Idle,
                    Wagons = new List<Wagon>(),
                    Locomotives = new List<Locomotive>()
                },
                new Train
                {
                    TypeOfTrain = TrainType.Passenger,
                    PriotityLevel = PriorityLevel.High,
                    State = TrainState.Idle,
                    Wagons = new List<Wagon>(),
                    Locomotives = new List<Locomotive>()
                },
                new Train
                {
                    TypeOfTrain = TrainType.Express,
                    PriotityLevel = PriorityLevel.Critical,
                    State = TrainState.Idle,
                    Wagons = new List<Wagon>(),
                    Locomotives = new List<Locomotive>()
                }
            );
        }

        if (!Stations.Any())
        {
            Stations.AddRange(
                new Station
                {
                    Name = "Baie de Beauport",
                    Longitude = -71.204475,
                    Latitude = 46.833728,
                    Capacity = 5,
                    Trains = new List<Train>(),
                    TrainsInStation = new List<Train>(),
                    RalwayLines = new List<RailwayLine>(),
                    Employees = new List<User>(),
                    Type = StationType.Point
                },
                new Station
                {
                    Name = "Port de Québec",
                    Longitude = -71.197774,
                    Latitude = 46.823961,
                    Capacity = 2,
                    Trains = new List<Train>(),
                    TrainsInStation = new List<Train>(),
                    RalwayLines = new List<RailwayLine>(),
                    Employees = new List<User>(),
                    Type = StationType.Point
                },
                new Station
                {
                    Name = "Centre de distribution",
                    Longitude = -71.225958,
                    Latitude = 46.793968,
                    Capacity = 3,
                    Trains = new List<Train>(),
                    TrainsInStation = new List<Train>(),
                    RalwayLines = new List<RailwayLine>(),
                    Employees = new List<User>(),
                    Type = StationType.Point
                },
                new Station
                {
                    Name = "Vers Charlevoix",
                    Longitude = -71.207817,
                    Latitude = 46.845779,
                    Capacity = 4,
                    Trains = new List<Train>(),
                    TrainsInStation = new List<Train>(),
                    RalwayLines = new List<RailwayLine>(),
                    Employees = new List<User>(),
                    Type = StationType.Point
                },
                new Station
                {
                    Name = "Vers la Rive-Sud",
                    Longitude = -71.290278,
                    Latitude = 46.748911,
                    Capacity = 1,
                    Trains = new List<Train>(),
                    TrainsInStation = new List<Train>(),
                    RalwayLines = new List<RailwayLine>(),
                    Employees = new List<User>(),
                    Type = StationType.Point
                },
                new Station
                {
                    Name = "Vers Gatineau",
                    Longitude = -71.428302,
                    Latitude = 46.770591,
                    Capacity = 2,
                    Trains = new List<Train>(),
                    TrainsInStation = new List<Train>(),
                    RalwayLines = new List<RailwayLine>(),
                    Employees = new List<User>(),
                    Type = StationType.Point
                },
                new Station
                {
                    Name = "Vers le Nord",
                    Longitude = -71.429804,
                    Latitude = 46.764212,
                    Capacity = 3,
                    Trains = new List<Train>(),
                    TrainsInStation = new List<Train>(),
                    RalwayLines = new List<RailwayLine>(),
                    Employees = new List<User>(),
                    Type = StationType.Point
                },
                new Station
                {
                    Name = "Gare du Palais",
                    Longitude = -71.2139,
                    Latitude = 46.8174,
                    Capacity = 10,
                    Trains = new List<Train>(),
                    TrainsInStation = new List<Train>(),
                    RalwayLines = new List<RailwayLine>(),
                    Employees = new List<User>(),
                    Type = StationType.Station
                },
                new Station
                {
                    Name = "Gare Québec-Gatineau",
                    Longitude = -71.332752,
                    Latitude = 46.795569,
                    Capacity = 8,
                    Trains = new List<Train>(),
                    TrainsInStation = new List<Train>(),
                    RalwayLines = new List<RailwayLine>(),
                    Employees = new List<User>(),
                    Type = StationType.Station
                },
                new Station
                {
                    Name = "Gare CN",
                    Longitude = -71.303381,
                    Latitude = 46.753156,
                    Capacity = 6,
                    Trains = new List<Train>(),
                    TrainsInStation = new List<Train>(),
                    RalwayLines = new List<RailwayLine>(),
                    Employees = new List<User>(),
                    Type = StationType.Station
                }

            );
            SaveChanges();
        }
        if (!Blocks.Any())
        {
            Blocks.AddRange(

                new Block
                {
                    Id = 1,
                    Longitude = -71.204255,
                    Latitude = 46.842256,
                },
                new Block
                {
                    Id = 2,
                    Longitude = -71.334879,
                    Latitude = 46.747842,
                },
                new Block
                {
                    Id = 3,
                    Longitude = -71.337711,
                    Latitude = 46.749053,
                },
                new Block
                {
                    Id = 4,
                    Longitude = -71.296210,
                    Latitude = 46.754409,
                },
                new Block
                {
                    Id = 5,
                    Longitude = -71.235611,
                    Latitude = 46.786403,
                },
                new Block
                {
                    Id = 6,
                    Longitude = -71.213773,
                    Latitude = 46.820117,
                },
                new Block
                {
                    Id = 7,
                    Longitude = -71.216567,
                    Latitude = 46.822044,
                },
                new Block
                {
                    Id = 8,
                    Longitude = -71.289502,
                    Latitude = 46.797511,
                },
                new Block
                {
                    Id = 9,
                    Longitude = -71.287356,
                    Latitude = 46.800243,
                },
                new Block
                {
                    Id = 10,
                    Longitude = -71.287764,
                    Latitude = 46.800625,
                },
                new Block
                {
                    Id = 11,
                    Longitude = -71.232142,
                    Latitude = 46.790461,
                },
                new Block
                {
                    Id = 12,
                    Longitude = -71.294171,
                    Latitude = 46.799669,
                },
                new Block
                {
                    Id = 13,
                    Longitude = -71.213549,
                    Latitude = 46.830895,
                },
                new Block
                {
                    Id = 14,
                    Longitude = -71.210308,
                    Latitude = 46.830998
                },
                new Block
                {
                    Id = 15,
                    Longitude = -71.223526,
                    Latitude = 46.828532
                },
                new Block
                {
                    Id = 16,
                    Longitude = -71.218849,
                    Latitude = 46.826829
                },
                new Block
                {
                    Id = 17,
                    Longitude = -71.339884,
                    Latitude = 46.747778
                },
                new Block
                {
                    Id = 18,
                    Longitude = -71.218491,
                    Latitude = 46.824097
                }
            );
            SaveChanges();
        }
    }
}
