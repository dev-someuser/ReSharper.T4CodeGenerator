using DatabaseExampleCore;

namespace DatabaseExampleProject
{
    [Table("Airports")]
    public class Airport
    {
        [PrimaryKey]
        public int Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }
    }
}