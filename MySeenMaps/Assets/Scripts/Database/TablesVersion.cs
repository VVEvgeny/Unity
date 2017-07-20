using SQLite;

namespace Assets.Scripts.Database
{
    public class TablesVersion
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string TableName { get; set; }
        public int Version { get; set; }
    }
}