using System;
using System.Security.Cryptography;
public class Train
{
    public int Id { get; set; }

    public TrainType TypeOfTrain { get; set; }

    public PriorityLevel PriotityLevel { get; set; }

    public TrainState State { get; set; }

    public List<Wagon> Wagons { get; set; }

    public List<Locomotive> Locomotives { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}