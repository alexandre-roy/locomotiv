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
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Block>()
            .HasMany(b => b.Points)
            .WithMany(bp => bp.Blocks);
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Train> Trains { get; set; }
    public DbSet<Station> Stations { get; set; }
    public DbSet<Block> Blocks { get; set; }
    public DbSet<BlockPoint> BlockPoints { get; set; }

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
                    Longitude = -71.204475,
                    Latitude = 46.833728,
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
                    Trains = new List<Train>(),
                    TrainsInStation = new List<Train>(),
                    RalwayLines = new List<RailwayLine>(),
                    Employees = new List<User>(),
                    Type = StationType.Station
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
        }

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
