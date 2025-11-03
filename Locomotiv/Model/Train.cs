using System;
using System.Security.Cryptography;
public class Train
{
    public int Id { get; set; }

    public TrainType TypeOfTrain { get; set; }

    public decimal Speed { get; set; }

    public PriorityLevel PriotityLevel { get; set; }

    public int Capacity { get; set; }

    public TrainState State { get; set; }
}