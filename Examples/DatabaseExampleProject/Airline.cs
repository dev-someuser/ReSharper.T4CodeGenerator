using DatabaseExampleCore;

namespace DatabaseExampleProject
{
    [Table("Airlines")]
    public class Airline
    {
        [PrimaryKey]
        public int Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }
    }
}