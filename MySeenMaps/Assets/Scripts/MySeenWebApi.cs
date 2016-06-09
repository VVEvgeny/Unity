using Newtonsoft.Json;

namespace Assets.Scripts
{
    public class MySeenWebApi
    {
        public const string ApiHost = "http://localhost:44301";
        public const string ApiSync = "/api/Sync/";
        public const string ApiLogin = "/api/Login/";
        public const string ApiUsers = "/api/Users/";
    
        public const int ApiVersion = 2;
        public const string Proxy = "http://217.23.121.11:3128";

        public enum SyncModesApiSync
        {
            GetRoads = 1
        }
        public enum SyncModesApiLogin
        {
            GetKey = 1
        }
        public enum SyncModesApiUsers
        {
            IsUserExists = 1
        }

        public class SyncJsonAnswer
        {
            //[JsonProperty("Value")]
            public Values Value { get; set; }

            //[JsonProperty("Data")]
            public string Data { get; set; }

            public enum Values
            {
                Ok = 1,
                NoData = 2,
                BadRequestMode = 3,
                UserNotExist = 4,
                NewDataRecieved = 5,
                NoLongerSupportedVersion = 6,
                SomeErrorObtained = 7
            }

            public override string ToString()
            {
                return Value.ToString();
            }
        }

        public class SyncJsonData
        {
            [JsonProperty("DataMode")]
            public int DataMode { get; set; } //in DataModes

            [JsonProperty("Name")]
            public string Name { get; set; }

            [JsonProperty("Type")]
            public int Type { get; set; }

            //[JsonProperty("Date")]
            //public DateTime Date { get; set; }

            [JsonProperty("Coordinates")]
            public string Coordinates { get; set; }

            [JsonProperty("Distance")]
            public double Distance { get; set; }
        }
    }
}