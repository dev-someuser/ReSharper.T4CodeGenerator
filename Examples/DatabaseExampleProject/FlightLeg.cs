using System;

using DatabaseExampleCore;

namespace DatabaseExampleProject
{
    [Table("FlightLegs")]
    public class FlightLeg
    {
        [PrimaryKey]
        public int Id { get; set; }

        [TableReference(typeof(Flight))]
        public int FlightId { get; set; }

        [TableReference(typeof(Airport))]
        public int DepartureAirportId { get; set; }

        public TimeSpan DepartureTime { get; set; }

        public int DepartureDayVariance { get; set; }

        [TableReference(typeof(Airport))]
        public int ArrivalAirportId { get; set; }

        public TimeSpan ArrivalTime { get; set; }

        public int ArrivalDayVariance { get; set; }
    }
}