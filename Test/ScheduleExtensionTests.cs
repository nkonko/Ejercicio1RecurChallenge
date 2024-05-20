using FluentAssertions;
using RecurChallenge.Entities;

namespace RecurChallenge.Tests
{
    public class ScheduleExtensionsTests
    {
        [Fact]
        public void FindOverlaps_NoOverlaps()
        {
            var schedules = new List<Schedule>
            {
                new Schedule { EmployeeId = "1", StartDate = DateTime.Parse("2024-05-20 08:00"), EndDate = DateTime.Parse("2024-05-20 12:00") },
                new Schedule { EmployeeId = "1", StartDate = DateTime.Parse("2024-05-20 13:00"), EndDate = DateTime.Parse("2024-05-20 17:00") }
            };

            var overlaps = schedules.FindOverlaps();

            overlaps.Should().BeEmpty();
        }

        [Fact]
        /// Schule 1    |----------|
        /// Schule 2  |--------------|
        public void FindOverlaps_OverLap_Schedule1_Inside_Schedule2()
        {
            var schedules = new List<Schedule>
            {
                new Schedule { EmployeeId = "1", StartDate = DateTime.Parse("2024-05-20 10:00"), EndDate = DateTime.Parse("2024-05-20 14:00") },
                new Schedule { EmployeeId = "1", StartDate = DateTime.Parse("2024-05-20 08:00"), EndDate = DateTime.Parse("2024-05-20 18:00") },
                new Schedule { EmployeeId = "2", StartDate = DateTime.Parse("2024-05-20 10:00"), EndDate = DateTime.Parse("2024-05-20 14:00") }
            };

            var overlaps = schedules.FindOverlaps();

            overlaps.Should().HaveCount(1);
        }

        [Fact]
        /// Schule 1   |----------|
        /// Schule 2  |--------|
        public void FindOverlaps_OverLap_Schedule1_StartBefore_Finish_Outside()
        {
            var schedules = new List<Schedule>
            {
                new Schedule { EmployeeId = "1", StartDate = DateTime.Parse("2024-05-20 08:00"), EndDate = DateTime.Parse("2024-05-20 12:00") },
                new Schedule { EmployeeId = "1", StartDate = DateTime.Parse("2024-05-20 07:00"), EndDate = DateTime.Parse("2024-05-20 11:00") },
                new Schedule { EmployeeId = "2", StartDate = DateTime.Parse("2024-05-20 07:00"), EndDate = DateTime.Parse("2024-05-20 11:00") },
            };

            var overlaps = schedules.FindOverlaps();

            overlaps.Should().HaveCount(1);
        }

        [Fact]
        /// Schule 1   |----------|
        /// Schule 2    |--------|
        public void FindOverlaps_Overlap_Schedule2_Inside_Schedule1()
        {
            var schedules = new List<Schedule>
            {
                new Schedule { EmployeeId = "1", StartDate = DateTime.Parse("2024-05-20 10:00"), EndDate = DateTime.Parse("2024-05-20 18:00") },
                new Schedule { EmployeeId = "1", StartDate = DateTime.Parse("2024-05-20 12:00"), EndDate = DateTime.Parse("2024-05-20 13:00") },
            };

            var overlaps = schedules.FindOverlaps();

            overlaps.Should().HaveCount(1);
        }

        [Fact]
        /// Schule 1   |----------|
        /// Schule 2        |--------|
        public void FindOverlaps_Overlap_Schedule2_StartBefore_Schedule1()
        {
            var schedules = new List<Schedule>
            {
                new Schedule { EmployeeId = "1", StartDate = DateTime.Parse("2024-05-20 07:00"), EndDate = DateTime.Parse("2024-05-20 12:00") },
                new Schedule { EmployeeId = "1", StartDate = DateTime.Parse("2024-05-20 09:00"), EndDate = DateTime.Parse("2024-05-20 18:00") },
            };

            var overlaps = schedules.FindOverlaps();

            overlaps.Should().HaveCount(1);
        }
    }
}
