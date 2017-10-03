namespace ExampleProject
{
    public class HotelRoom
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int MaxGuests { get; set; }

        public bool IsActive { get; set; }

        public HotelRoom(HotelRoom source)
        {
            this.Id = source.Id;
            this.Name = source.Name;
            this.MaxGuests = source.MaxGuests;
            this.IsActive = source.IsActive;
        }

        public HotelRoom()
        {
            
        }
    }
}