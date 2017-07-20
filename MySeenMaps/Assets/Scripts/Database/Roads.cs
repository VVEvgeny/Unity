using SQLite;

namespace Assets.Scripts.Database
{
    public class Roads
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int Type { get; set; }
        public string Name { get; set; }
        public string Coordinates { get; set; }
        public double Distance { get; set; }
    }
}