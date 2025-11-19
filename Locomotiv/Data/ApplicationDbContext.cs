using Locomotiv.Model;
using Locomotiv.Utils.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Windows.Documents;

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

        modelBuilder.Entity<User>()
            .HasOne(u => u.Station)
            .WithMany();

        modelBuilder.Entity<Block>()
            .HasMany(b => b.Points)
            .WithMany(bp => bp.Blocks);

        modelBuilder.Entity<Train>()
            .HasMany(t => t.Locomotives)
            .WithMany();

        modelBuilder.Entity<Train>()
            .HasMany(t => t.Wagons)
            .WithMany();

    }


    public DbSet<Locomotive> Locomotives { get; set; }
    public DbSet<Wagon> Wagons { get; set; }
    public DbSet<Train> Trains { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Station> Stations { get; set; }
    public DbSet<Block> Blocks { get; set; }
    public DbSet<BlockPoint> BlockPoints { get; set; }

    IConfiguration config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", optional: false).Build();

    public void SeedData()
    {

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
                new Locomotive { Code = "Loco-010" },
                new Locomotive { Code = "Loco-011" },
                new Locomotive { Code = "Loco-012" },
                new Locomotive { Code = "Loco-013" },
                new Locomotive { Code = "Loco-014" },
                new Locomotive { Code = "Loco-015" },
                new Locomotive { Code = "Loco-016" },
                new Locomotive { Code = "Loco-017" },
                new Locomotive { Code = "Loco-018" },
                new Locomotive { Code = "Loco-019" },
                new Locomotive { Code = "Loco-020" },
                new Locomotive { Code = "Loco-021" },
                new Locomotive { Code = "Loco-022" },
                new Locomotive { Code = "Loco-023" },
                new Locomotive { Code = "Loco-024" },
                new Locomotive { Code = "Loco-025" },
                new Locomotive { Code = "Loco-026" },
                new Locomotive { Code = "Loco-027" },
                new Locomotive { Code = "Loco-028" },
                new Locomotive { Code = "Loco-029" },
                new Locomotive { Code = "Loco-030" },
                new Locomotive { Code = "Loco-031" },
                new Locomotive { Code = "Loco-032" },
                new Locomotive { Code = "Loco-033" },
                new Locomotive { Code = "Loco-034" },
                new Locomotive { Code = "Loco-035" },
                new Locomotive { Code = "Loco-036" },
                new Locomotive { Code = "Loco-037" },
                new Locomotive { Code = "Loco-038" },
                new Locomotive { Code = "Loco-039" },
                new Locomotive { Code = "Loco-040" }
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

        List<Locomotive> savedLocomotives = Locomotives.ToList();
        List<Wagon> savedWagons = Wagons.ToList();

        if (!Trains.Any())
        {
            Trains.AddRange(
                new Train { TypeOfTrain = TrainType.Maintenance, PriotityLevel = PriorityLevel.Low, State = TrainState.Idle, Wagons = new List<Wagon> { savedWagons[0] }, Locomotives = new List<Locomotive> { savedLocomotives[0] } },
                new Train { TypeOfTrain = TrainType.Merchandise, PriotityLevel = PriorityLevel.Medium, State = TrainState.Idle, Wagons = new List<Wagon> { savedWagons[1] }, Locomotives = new List<Locomotive> { savedLocomotives[1] } },
                new Train { TypeOfTrain = TrainType.Passenger, PriotityLevel = PriorityLevel.High, State = TrainState.Idle, Wagons = new List<Wagon> { savedWagons[2] }, Locomotives = new List<Locomotive> { savedLocomotives[2] } },
                new Train { TypeOfTrain = TrainType.Express, PriotityLevel = PriorityLevel.Critical, State = TrainState.Idle, Wagons = new List<Wagon> { savedWagons[3] }, Locomotives = new List<Locomotive> { savedLocomotives[3] } },
                new Train { TypeOfTrain = TrainType.Maintenance, PriotityLevel = PriorityLevel.Low, State = TrainState.Idle, Wagons = new List<Wagon> { savedWagons[4] }, Locomotives = new List<Locomotive> { savedLocomotives[4] } },
                new Train { TypeOfTrain = TrainType.Merchandise, PriotityLevel = PriorityLevel.Medium, State = TrainState.Idle, Wagons = new List<Wagon> { savedWagons[5] }, Locomotives = new List<Locomotive> { savedLocomotives[5] } },
                new Train { TypeOfTrain = TrainType.Passenger, PriotityLevel = PriorityLevel.High, State = TrainState.Idle, Wagons = new List<Wagon> { savedWagons[6] }, Locomotives = new List<Locomotive> { savedLocomotives[6] } },
                new Train { TypeOfTrain = TrainType.Express, PriotityLevel = PriorityLevel.Critical, State = TrainState.Idle, Wagons = new List<Wagon> { savedWagons[7] }, Locomotives = new List<Locomotive> { savedLocomotives[7] } },
                new Train { TypeOfTrain = TrainType.Maintenance, PriotityLevel = PriorityLevel.Low, State = TrainState.Idle, Wagons = new List<Wagon> { savedWagons[8] }, Locomotives = new List<Locomotive> { savedLocomotives[8] } },
                new Train { TypeOfTrain = TrainType.Merchandise, PriotityLevel = PriorityLevel.Medium, State = TrainState.Idle, Wagons = new List<Wagon> { savedWagons[9] }, Locomotives = new List<Locomotive> { savedLocomotives[9] } },
                new Train { TypeOfTrain = TrainType.Passenger, PriotityLevel = PriorityLevel.High, State = TrainState.Idle, Wagons = new List<Wagon> { savedWagons[10] }, Locomotives = new List<Locomotive> { savedLocomotives[10] } },
                new Train { TypeOfTrain = TrainType.Express, PriotityLevel = PriorityLevel.Critical, State = TrainState.Idle, Wagons = new List<Wagon> { savedWagons[11] }, Locomotives = new List<Locomotive> { savedLocomotives[11] } },
                new Train { TypeOfTrain = TrainType.Maintenance, PriotityLevel = PriorityLevel.Low, State = TrainState.Idle, Wagons = new List<Wagon> { savedWagons[12] }, Locomotives = new List<Locomotive> { savedLocomotives[12] } },
                new Train { TypeOfTrain = TrainType.Merchandise, PriotityLevel = PriorityLevel.Medium, State = TrainState.Idle, Wagons = new List<Wagon> { savedWagons[13] }, Locomotives = new List<Locomotive> { savedLocomotives[13] } },
                new Train { TypeOfTrain = TrainType.Passenger, PriotityLevel = PriorityLevel.High, State = TrainState.Idle, Wagons = new List<Wagon> { savedWagons[14] }, Locomotives = new List<Locomotive> { savedLocomotives[14] } },
                new Train { TypeOfTrain = TrainType.Express, PriotityLevel = PriorityLevel.Critical, State = TrainState.Idle, Wagons = new List<Wagon> { savedWagons[15] }, Locomotives = new List<Locomotive> { savedLocomotives[15] } },
                new Train { TypeOfTrain = TrainType.Maintenance, PriotityLevel = PriorityLevel.Low, State = TrainState.Idle, Wagons = new List<Wagon> { savedWagons[16] }, Locomotives = new List<Locomotive> { savedLocomotives[16] } },
                new Train { TypeOfTrain = TrainType.Merchandise, PriotityLevel = PriorityLevel.Medium, State = TrainState.Idle, Wagons = new List<Wagon> { savedWagons[17] }, Locomotives = new List<Locomotive> { savedLocomotives[17] } },
                new Train { TypeOfTrain = TrainType.Passenger, PriotityLevel = PriorityLevel.High, State = TrainState.Idle, Wagons = new List<Wagon> { savedWagons[18] }, Locomotives = new List<Locomotive> { savedLocomotives[18] } },
                new Train { TypeOfTrain = TrainType.Express, PriotityLevel = PriorityLevel.Critical, State = TrainState.Idle, Wagons = new List<Wagon> { savedWagons[19] }, Locomotives = new List<Locomotive> { savedLocomotives[19] } },
                new Train { TypeOfTrain = TrainType.Maintenance, PriotityLevel = PriorityLevel.Low, State = TrainState.Idle, Wagons = new List<Wagon> { savedWagons[20] }, Locomotives = new List<Locomotive> { savedLocomotives[20] } },
                new Train { TypeOfTrain = TrainType.Merchandise, PriotityLevel = PriorityLevel.Medium, State = TrainState.Idle, Wagons = new List<Wagon> { savedWagons[21] }, Locomotives = new List<Locomotive> { savedLocomotives[21] } },
                new Train { TypeOfTrain = TrainType.Passenger, PriotityLevel = PriorityLevel.High, State = TrainState.Idle, Wagons = new List<Wagon> { savedWagons[22] }, Locomotives = new List<Locomotive> { savedLocomotives[22] } },
                new Train { TypeOfTrain = TrainType.Express, PriotityLevel = PriorityLevel.Critical, State = TrainState.Idle, Wagons = new List<Wagon> { savedWagons[23] }, Locomotives = new List<Locomotive> { savedLocomotives[23] } },
                new Train { TypeOfTrain = TrainType.Maintenance, PriotityLevel = PriorityLevel.Low, State = TrainState.Idle, Wagons = new List<Wagon> { savedWagons[24] }, Locomotives = new List<Locomotive> { savedLocomotives[24] } },
                new Train { TypeOfTrain = TrainType.Merchandise, PriotityLevel = PriorityLevel.Medium, State = TrainState.Idle, Wagons = new List<Wagon> { savedWagons[25] }, Locomotives = new List<Locomotive> { savedLocomotives[25] } },
                new Train { TypeOfTrain = TrainType.Passenger, PriotityLevel = PriorityLevel.High, State = TrainState.Idle, Wagons = new List<Wagon> { savedWagons[26] }, Locomotives = new List<Locomotive> { savedLocomotives[26] } },
                new Train { TypeOfTrain = TrainType.Express, PriotityLevel = PriorityLevel.Critical, State = TrainState.Idle, Wagons = new List<Wagon> { savedWagons[27] }, Locomotives = new List<Locomotive> { savedLocomotives[27] } },
                new Train { TypeOfTrain = TrainType.Maintenance, PriotityLevel = PriorityLevel.Low, State = TrainState.Idle, Wagons = new List<Wagon> { savedWagons[28] }, Locomotives = new List<Locomotive> { savedLocomotives[28] } },
                new Train { TypeOfTrain = TrainType.Merchandise, PriotityLevel = PriorityLevel.Medium, State = TrainState.Idle, Wagons = new List<Wagon> { savedWagons[29] }, Locomotives = new List<Locomotive> { savedLocomotives[29] } },
                new Train { TypeOfTrain = TrainType.Passenger, PriotityLevel = PriorityLevel.High, State = TrainState.Idle, Wagons = new List<Wagon> { savedWagons[30] }, Locomotives = new List<Locomotive> { savedLocomotives[30] } },
                new Train { TypeOfTrain = TrainType.Express, PriotityLevel = PriorityLevel.Critical, State = TrainState.Idle, Wagons = new List<Wagon> { savedWagons[31] }, Locomotives = new List<Locomotive> { savedLocomotives[31] } },
                new Train { TypeOfTrain = TrainType.Maintenance, PriotityLevel = PriorityLevel.Low, State = TrainState.Idle, Wagons = new List<Wagon> { savedWagons[32] }, Locomotives = new List<Locomotive> { savedLocomotives[32] } },
                new Train { TypeOfTrain = TrainType.Merchandise, PriotityLevel = PriorityLevel.Medium, State = TrainState.Idle, Wagons = new List<Wagon> { savedWagons[33] }, Locomotives = new List<Locomotive> { savedLocomotives[33] } },
                new Train { TypeOfTrain = TrainType.Passenger, PriotityLevel = PriorityLevel.High, State = TrainState.Idle, Wagons = new List<Wagon> { savedWagons[34] }, Locomotives = new List<Locomotive> { savedLocomotives[34] } },
                new Train { TypeOfTrain = TrainType.Express, PriotityLevel = PriorityLevel.Critical, State = TrainState.Idle, Wagons = new List<Wagon> { savedWagons[35] }, Locomotives = new List<Locomotive> { savedLocomotives[35] } },
                new Train { TypeOfTrain = TrainType.Maintenance, PriotityLevel = PriorityLevel.Low, State = TrainState.Idle, Wagons = new List<Wagon> { savedWagons[36] }, Locomotives = new List<Locomotive> { savedLocomotives[36] } },
                new Train { TypeOfTrain = TrainType.Merchandise, PriotityLevel = PriorityLevel.Medium, State = TrainState.Idle, Wagons = new List<Wagon> { savedWagons[37] }, Locomotives = new List<Locomotive> { savedLocomotives[37] } },
                new Train { TypeOfTrain = TrainType.Passenger, PriotityLevel = PriorityLevel.High, State = TrainState.Idle, Wagons = new List<Wagon> { savedWagons[38] }, Locomotives = new List<Locomotive> { savedLocomotives[38] } },
                new Train { TypeOfTrain = TrainType.Express, PriotityLevel = PriorityLevel.Critical, State = TrainState.Idle, Wagons = new List<Wagon> { savedWagons[39] }, Locomotives = new List<Locomotive> { savedLocomotives[39] } }
            );

            SaveChanges();

        }
        List<Train> savedTrains = Trains.ToList();

        if (!Stations.Any())
        {
            Stations.AddRange(

            new Station
            {
                Name = "Baie de Beauport",
                Longitude = -71.204255,
                Latitude = 46.842256,
                Trains = new List<Train> { savedTrains[0], savedTrains[1] },
                TrainsInStation = new List<Train> { savedTrains[2], savedTrains[3] },
            },
            new Station
            {
                Name = "Port de Québec",
                Longitude = -71.197774,
                Latitude = 46.823961,
                Trains = new List<Train> { savedTrains[4], savedTrains[5] },
                TrainsInStation = new List<Train> { savedTrains[6], savedTrains[7] },
            },
            new Station
            {
                Name = "Centre de distribution",
                Longitude = -71.23208,
                Latitude = 46.789962,
                Trains = new List<Train> { savedTrains[8], savedTrains[9] },
                TrainsInStation = new List<Train> { savedTrains[10], savedTrains[11] },
            },
            new Station
            {
                Name = "Vers Charlevoix",
                Longitude = -71.207817,
                Latitude = 46.845779,
                Trains = new List<Train> { savedTrains[12], savedTrains[13] },
                TrainsInStation = new List<Train> { savedTrains[14], savedTrains[15] },
            },
            new Station
            {
                Name = "Vers la Rive-Sud",
                Longitude = -71.290278,
                Latitude = 46.748911,
                Trains = new List<Train> { savedTrains[16], savedTrains[17] },
                TrainsInStation = new List<Train> { savedTrains[18], savedTrains[19] },
            },
            new Station
            {
                Name = "Vers Gatineau",
                Longitude = -71.428372,
                Latitude = 46.771591,
                Trains = new List<Train> { savedTrains[20], savedTrains[21] },
                TrainsInStation = new List<Train> { savedTrains[22], savedTrains[23] },
            },
            new Station
            {
                Name = "Vers le Nord",
                Longitude = -71.432235,
                Latitude = 46.765369,
                Trains = new List<Train> { savedTrains[24], savedTrains[25] },
                TrainsInStation = new List<Train> { savedTrains[26], savedTrains[27] },
            },
            new Station
            {
                Name = "Gare du Palais",
                Longitude = -71.2139,
                Latitude = 46.8174,
                Trains = new List<Train> { savedTrains[28], savedTrains[29] },
                TrainsInStation = new List<Train> { savedTrains[30], savedTrains[31] },
            },
            new Station
            {
                Name = "Gare Québec-Gatineau",
                Longitude = -71.332752,
                Latitude = 46.795569,
                Trains = new List<Train> { savedTrains[32], savedTrains[33] },
                TrainsInStation = new List<Train> { savedTrains[34], savedTrains[35] },
            },
            new Station
            {
                Name = "Gare CN",
                Longitude = -71.303381,
                Latitude = 46.753156,
                Trains = new List<Train> { savedTrains[36], savedTrains[37] },
                TrainsInStation = new List<Train> { savedTrains[38], savedTrains[39] },
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

        if (!BlockPoints.Any())
        {
            BlockPoints.AddRange(

                new BlockPoint
                {
                    Id = 1,
                    Longitude = -71.204255,
                    Latitude = 46.842256,
                },
                new BlockPoint
                {
                    Id = 2,
                    Longitude = -71.334879,
                    Latitude = 46.747842,
                },
                new BlockPoint
                {
                    Id = 3,
                    Longitude = -71.337711,
                    Latitude = 46.749053,
                },
                new BlockPoint
                {
                    Id = 4,
                    Longitude = -71.428566,
                    Latitude = 46.76423,
                },
                new BlockPoint
                {
                    Id = 5,
                    Longitude = -71.235611,
                    Latitude = 46.786403,
                },
                new BlockPoint
                {
                    Id = 6,
                    Longitude = -71.213773,
                    Latitude = 46.820117,
                },
                new BlockPoint
                {
                    Id = 7,
                    Longitude = -71.216567,
                    Latitude = 46.822044,
                },
                new BlockPoint
                {
                    Id = 8,
                    Longitude = -71.289502,
                    Latitude = 46.797511,
                },
                new BlockPoint
                {
                    Id = 9,
                    Longitude = -71.287356,
                    Latitude = 46.800243,
                },
                new BlockPoint
                {
                    Id = 10,
                    Longitude = -71.287764,
                    Latitude = 46.800625,
                },
                new BlockPoint
                {
                    Id = 11,
                    Longitude = -71.232142,
                    Latitude = 46.790461,
                },
                new BlockPoint
                {
                    Id = 12,
                    Longitude = -71.294171,
                    Latitude = 46.799669,
                },
                new BlockPoint
                {
                    Id = 13,
                    Longitude = -71.213549,
                    Latitude = 46.830895,
                },
                new BlockPoint
                {
                    Id = 14,
                    Longitude = -71.210308,
                    Latitude = 46.830998
                },
                new BlockPoint
                {
                    Id = 15,
                    Longitude = -71.223526,
                    Latitude = 46.828532
                },
                new BlockPoint
                {
                    Id = 16,
                    Longitude = -71.218849,
                    Latitude = 46.826829
                },
                new BlockPoint
                {
                    Id = 17,
                    Longitude = -71.339884,
                    Latitude = 46.747778
                },
                new BlockPoint
                {
                    Id = 18,
                    Longitude = -71.218491,
                    Latitude = 46.824097
                },
                new BlockPoint
                {
                    Id = 19,
                    Longitude = -71.320005,
                    Latitude = 46.796969
                },
                new BlockPoint
                {
                    Id = 20,
                    Longitude = -71.351264,
                    Latitude = 46.792849
                },
                new BlockPoint
                {
                    Id = 21,
                    Longitude = -71.31513,
                    Latitude = 46.751768
                },
                new BlockPoint
                {
                    Id = 22,
                    Longitude = -71.297888,
                    Latitude = 46.754179
                },
                new BlockPoint
                {
                    Id = 23,
                    Longitude = -71.228659,
                    Latitude = 46.792598
                },
                new BlockPoint
                {
                    Id = 24,
                    Longitude = -71.22396,
                    Latitude = 46.794831
                },
                new BlockPoint
                {
                    Id = 25,
                    Longitude = -71.19766,
                    Latitude = 46.836048
                },
                new BlockPoint
                {
                    Id = 26,
                    Longitude = -71.195107,
                    Latitude = 46.832995
                },
                new BlockPoint
                {
                    Id = 27,
                    Longitude = -71.197474,
                    Latitude = 46.823175
                },
                new BlockPoint
                {
                    Id = 28,
                    Longitude = -71.207624,
                    Latitude = 46.845702
                },
                new BlockPoint
                {
                    Id = 29,
                    Longitude = -71.284019,
                    Latitude = 46.802571
                },
                new BlockPoint
                {
                    Id = 30,
                    Longitude = -71.428995,
                    Latitude = 46.770815
                }
            );
            SaveChanges();


            if (!Blocks.Any())
            {
                var allPoints = BlockPoints.ToList();

                var block1 = new Block
                {
                    Points = new List<BlockPoint>
                    {
                        allPoints.First(bp => bp.Id == 19),
                        allPoints.First(bp => bp.Id == 20)
                    }
                };

                var block2 = new Block
                {
                    Points = new List<BlockPoint>
                {
                    allPoints.First(bp => bp.Id == 19),
                    allPoints.First(bp => bp.Id == 12)
                }
                };

                var block3 = new Block
                {
                    Points = new List<BlockPoint>
                {
                    allPoints.First(bp => bp.Id == 12),
                    allPoints.First(bp => bp.Id == 10)
                }
                };

                var block4 = new Block
                {
                    Points = new List<BlockPoint>
                {
                    allPoints.First(bp => bp.Id == 12),
                    allPoints.First(bp => bp.Id == 8)
                }
                };

                var block5 = new Block
                {
                    Points = new List<BlockPoint>
                {
                    allPoints.First(bp => bp.Id == 8),
                    allPoints.First(bp => bp.Id == 9)
                }
                };

                var block6 = new Block
                {
                    Points = new List<BlockPoint>
                {
                    allPoints.First(bp => bp.Id == 9),
                    allPoints.First(bp => bp.Id == 11)
                }
                };

                var block7 = new Block
                {
                    Points = new List<BlockPoint>
                {
                    allPoints.First(bp => bp.Id == 11),
                    allPoints.First(bp => bp.Id == 5)
                }
                };

                var block8 = new Block
                {
                    Points = new List<BlockPoint>
                {
                    allPoints.First(bp => bp.Id == 11),
                    allPoints.First(bp => bp.Id == 23)
                }
                };

                var block9 = new Block
                {
                    Points = new List<BlockPoint>
                {
                    allPoints.First(bp => bp.Id == 10),
                    allPoints.First(bp => bp.Id == 29)
                }
                };

                var block10 = new Block
                {
                    Points = new List<BlockPoint>
                {
                    allPoints.First(bp => bp.Id == 9),
                    allPoints.First(bp => bp.Id == 29)
                }
                };

                var block11 = new Block
                {
                    Points = new List<BlockPoint>
                {
                    allPoints.First(bp => bp.Id == 29),
                    allPoints.First(bp => bp.Id == 15)
                }
                };

                var block12 = new Block
                {
                    Points = new List<BlockPoint>
                {
                    allPoints.First(bp => bp.Id == 15),
                    allPoints.First(bp => bp.Id == 16)
                }
                };

                var block13 = new Block
                {
                    Points = new List<BlockPoint>
                {
                    allPoints.First(bp => bp.Id == 15),
                    allPoints.First(bp => bp.Id == 18)
                }
                };
                var block14 = new Block
                {
                    Points = new List<BlockPoint>
                {
                    allPoints.First(bp => bp.Id == 18),
                    allPoints.First(bp => bp.Id == 16)
                }
                };
                var block15 = new Block
                {
                    Points = new List<BlockPoint>
                {
                    allPoints.First(bp => bp.Id == 18),
                    allPoints.First(bp => bp.Id == 7)
                }
                };
                var block16 = new Block
                {
                    Points = new List<BlockPoint>
                {
                    allPoints.First(bp => bp.Id == 7),
                    allPoints.First(bp => bp.Id == 6)
                }
                };
                var block17 = new Block
                {
                    Points = new List<BlockPoint>
                {
                    allPoints.First(bp => bp.Id == 6),
                    allPoints.First(bp => bp.Id == 27)
                }
                };
                var block18 = new Block
                {
                    Points = new List<BlockPoint>
                {
                    allPoints.First(bp => bp.Id == 16),
                    allPoints.First(bp => bp.Id == 13)
                }
                };
                var block19 = new Block
                {
                    Points = new List<BlockPoint>
                {
                    allPoints.First(bp => bp.Id == 13),
                    allPoints.First(bp => bp.Id == 14)
                }
                };
                var block20 = new Block
                {
                    Points = new List<BlockPoint>
                {
                    allPoints.First(bp => bp.Id == 14),
                    allPoints.First(bp => bp.Id == 25)
                }
                };
                var block21 = new Block
                {
                    Points = new List<BlockPoint>
                {
                    allPoints.First(bp => bp.Id == 14),
                    allPoints.First(bp => bp.Id == 26)
                }
                };
                var block22 = new Block
                {
                    Points = new List<BlockPoint>
                {
                    allPoints.First(bp => bp.Id == 13),
                    allPoints.First(bp => bp.Id == 1)
                }
                };
                var block23 = new Block
                {
                    Points = new List<BlockPoint>
                {
                    allPoints.First(bp => bp.Id == 16),
                    allPoints.First(bp => bp.Id == 28)
                }
                };
                var block24 = new Block
                {
                    Points = new List<BlockPoint>
                {
                    allPoints.First(bp => bp.Id == 22),
                    allPoints.First(bp => bp.Id == 5)
                }
                };
                var block25 = new Block
                {
                    Points = new List<BlockPoint>
                {
                    allPoints.First(bp => bp.Id == 21),
                    allPoints.First(bp => bp.Id == 2)
                }
                };
                var block26 = new Block
                {
                    Points = new List<BlockPoint>
                {
                    allPoints.First(bp => bp.Id == 2),
                    allPoints.First(bp => bp.Id == 17)
                }
                };
                var block27 = new Block
                {
                    Points = new List<BlockPoint>
                {
                    allPoints.First(bp => bp.Id == 2),
                    allPoints.First(bp => bp.Id == 3)
                }
                };
                var block28 = new Block
                {
                    Points = new List<BlockPoint>
                {
                    allPoints.First(bp => bp.Id == 17),
                    allPoints.First(bp => bp.Id == 3)
                }
                };
                var block29 = new Block
                {
                    Points = new List<BlockPoint>
                {
                    allPoints.First(bp => bp.Id == 3),
                    allPoints.First(bp => bp.Id == 8)
                }
                };
                var block30 = new Block
                {
                    Points = new List<BlockPoint>
                {
                    allPoints.First(bp => bp.Id == 21),
                    allPoints.First(bp => bp.Id == 22)
                }
                };
                var block31 = new Block
                {
                    Points = new List<BlockPoint>
                {
                    allPoints.First(bp => bp.Id == 20),
                    allPoints.First(bp => bp.Id == 30)
                }
                };
                var block32 = new Block
                {
                    Points = new List<BlockPoint>
                {
                    allPoints.First(bp => bp.Id == 17),
                    allPoints.First(bp => bp.Id == 4)
                }
                };
                var block33 = new Block
                {
                    Points = new List<BlockPoint>
                {
                    allPoints.First(bp => bp.Id == 23),
                    allPoints.First(bp => bp.Id == 24)
                }
                };
                Blocks.AddRange(block1, block2, block3, block4, block5, block6, block7, block8, block9, block10, block11, block12, block13, block14, block15, block16, block17, block18, block19, block20, block21, block22, block23, block24, block25, block26, block27, block28, block29, block30, block31, block32);
                SaveChanges();
            }


        }

    }



}
