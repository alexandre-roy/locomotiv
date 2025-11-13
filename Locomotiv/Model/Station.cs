using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Station
{
    public int Id { get; set; }

    public string Name { get; set; }

    public double Latitude { get; set; }

    public double Longitude { get; set; }

    public int Capacity { get; set; }

    public List<Train> Trains { get; set; }

    public List<Train> TrainsInStation { get; set; }

    public List<RailwayLine> RalwayLines { get; set; }

    public List<User> Employees { get; set; }

    public StationType Type { get; set; }
}
