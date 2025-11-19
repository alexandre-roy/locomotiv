using System;
using System.Security.Cryptography;
public class Train
{
    public int Id { get; set; }

    public TrainType TypeOfTrain { get; set; }

    public PriorityLevel PriotityLevel { get; set; }

    public TrainState State { get; set; }

    public ICollection<Wagon> Wagons { get; set; }

    public ICollection<Locomotive> Locomotives { get; set; }
}