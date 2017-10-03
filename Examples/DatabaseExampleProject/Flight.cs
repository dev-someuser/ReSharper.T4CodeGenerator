using System;
using System.Collections.Generic;

using DatabaseExampleCore;

namespace DatabaseExampleProject
{
    [Table("Flights")]
    public class Flight
    {
        [PrimaryKey]
        public int Id { get; set; }

        [TableReference(typeof(Airline))]
        public int AirlineId { get; set; }

        public string Number { get; set; }

        public DateTime DepartureDate { get; set; }

        public List<FlightLeg> Legs { get; set; }
    }
}