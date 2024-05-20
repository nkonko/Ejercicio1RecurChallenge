using System.Collections.Generic;
using System.Linq;

namespace RecurChallenge.Entities
{
    public static class ScheduleExtensions
    {
        public static List<(Schedule, Schedule)> FindOverlaps(this List<Schedule> schedules)
        {
            var overlaps = new List<(Schedule, Schedule)>();

            var groupedSchedules = schedules.GroupBy(s => s.EmployeeId);

            foreach (var group in groupedSchedules)
            {
                var employeeSchedules = group.ToList();

                if (employeeSchedules.Count > 1)
                {
                    for (int i = 0; i < employeeSchedules.Count - 1; i++)
                    {
                        for (int j = i + 1; j < employeeSchedules.Count; j++)
                        {
                            var schedule1 = employeeSchedules[i];
                            var schedule2 = employeeSchedules[j];

                            if (schedule1.StartDate < schedule2.EndDate && schedule2.StartDate < schedule1.EndDate)
                            {
                                overlaps.Add((schedule1, schedule2));
                            }
                        }
                    }
                }
            }

            return overlaps;
        }
    }
}
