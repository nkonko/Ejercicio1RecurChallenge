using RecurChallenge.Entities;
using System;
using System.Collections.Generic;

namespace RecurChallenge
{
    internal class Program
    {
        static void Main(string[] args)
        {

            var schedules = new List<Schedule>
            {
                new Schedule { EmployeeId = "001", StartDate = new DateTime(2024, 5, 1, 11, 0, 0), EndDate = new DateTime(2024, 5, 1, 13, 0, 0) },
                new Schedule { EmployeeId = "002", StartDate = new DateTime(2024, 5, 1, 11, 0, 0), EndDate = new DateTime(2024, 5, 1, 13, 0, 0) },
                new Schedule { EmployeeId = "003", StartDate = new DateTime(2024, 5, 1, 11, 0, 0), EndDate = new DateTime(2024, 5, 1, 13, 0, 0) },

                new Schedule { EmployeeId = "001", StartDate = new DateTime(2024, 5, 1, 12, 0, 0), EndDate = new DateTime(2024, 6, 1, 13, 0, 0) },

                new Schedule { EmployeeId = "001", StartDate = new DateTime(2024, 7, 1, 9, 0, 0), EndDate = new DateTime(2024, 7, 1, 17, 0, 0) },
            };

            var overlaps = schedules.FindOverlaps();

            Console.WriteLine("Overlapping Schedules:");
            foreach (var overlap in overlaps)
            {
                Console.WriteLine($"Employee {overlap.Item1.EmployeeId}: start: {overlap.Item1.StartDate} - end: {overlap.Item1.EndDate} \n overlaps with start: {overlap.Item2.StartDate} - end:{overlap.Item2.EndDate} \n");
            }
        }
    }
}