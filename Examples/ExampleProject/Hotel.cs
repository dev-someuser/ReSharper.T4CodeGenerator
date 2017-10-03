using System;
using System.Collections.Generic;
using System.Linq;

namespace ExampleProject
{
    public class Hotel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int Category { get; set; }

        public List<HotelRoom> Rooms { get; set; }

        public string GetActiveRoomNameById(string name)
        {
            string result = this.Rooms
                .Where(x => x.IsActive)
                .Select(x => x.Name)
                .First(x => string.Equals(name, x, StringComparison.OrdinalIgnoreCase));

            return result;
        }
    }
}