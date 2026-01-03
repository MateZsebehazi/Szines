namespace szines.API.Models
{
    public class ColorEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string HexValue { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
