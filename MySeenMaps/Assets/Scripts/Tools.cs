using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Newtonsoft.Json;

internal class EncodePolyline
{
    /// <summary>
    ///     Decode google style polyline coordinates.
    /// </summary>
    /// <param name="encodedPoints"></param>
    /// <returns></returns>
    public static IEnumerable<GoogleMapLocation> DecodeCoordinates(string encodedPoints)
    {
        if (string.IsNullOrEmpty(encodedPoints))
            throw new ArgumentNullException("encodedPoints");

        var polylineChars = encodedPoints.ToCharArray();
        var index = 0;

        var currentLat = 0;
        var currentLng = 0;
        int next5bits;
        int sum;
        int shifter;

        while (index < polylineChars.Length)
        {
            // calculate next latitude
            sum = 0;
            shifter = 0;
            do
            {
                next5bits = polylineChars[index++] - 63;
                sum |= (next5bits & 31) << shifter;
                shifter += 5;
            } while (next5bits >= 32 && index < polylineChars.Length);

            if (index >= polylineChars.Length)
                break;

            currentLat += (sum & 1) == 1 ? ~(sum >> 1) : (sum >> 1);

            //calculate next longitude
            sum = 0;
            shifter = 0;
            do
            {
                next5bits = polylineChars[index++] - 63;
                sum |= (next5bits & 31) << shifter;
                shifter += 5;
            } while (next5bits >= 32 && index < polylineChars.Length);

            if (index >= polylineChars.Length && next5bits >= 32)
                break;

            currentLng += (sum & 1) == 1 ? ~(sum >> 1) : (sum >> 1);

            yield return new GoogleMapLocation
            {
                Latitude = float.Parse((Convert.ToDouble(currentLat)/1E5).ToString(CultureInfo.InvariantCulture)),
                Longitude = float.Parse((Convert.ToDouble(currentLng)/1E5).ToString(CultureInfo.InvariantCulture))
            };
        }
    }

    /// <summary>
    ///     Encode it
    /// </summary>
    /// <param name="points"></param>
    /// <returns></returns>
    public static string EncodeCoordinates(IEnumerable<GoogleMapLocation> points)
    {
        var str = new StringBuilder();

        var encodeDiff = (Action<int>) (diff =>
        {
            var shifted = diff << 1;
            if (diff < 0)
                shifted = ~shifted;

            var rem = shifted;

            while (rem >= 0x20)
            {
                str.Append((char) ((0x20 | (rem & 0x1f)) + 63));

                rem >>= 5;
            }

            str.Append((char) (rem + 63));
        });

        var lastLat = 0;
        var lastLng = 0;

        foreach (var point in points)
        {
            var lat = (int) Math.Round(point.Latitude*1E5);
            var lng = (int) Math.Round(point.Longitude*1E5);

            encodeDiff(lat - lastLat);
            encodeDiff(lng - lastLng);

            lastLat = lat;
            lastLng = lng;
        }

        return str.ToString();
    }
}

public class Point
{
    public readonly int x;
    public readonly int y;

    public Point(int _x, int _y)
    {
        x = _x;
        y = _y;
    }
}

public enum GoogleMapColor
{
    black,
    brown,
    green,
    purple,
    yellow,
    blue,
    gray,
    orange,
    red,
    white
}

[Serializable]
public class GoogleMapLocation
{
    public string Address;
    public float Latitude;
    public float Longitude;
}

[Serializable]
public class GoogleMapMarker
{
    public GoogleMapColor Color;
    public string Label;
    public List<GoogleMapLocation> Locations;
    public GoogleMapMarkerSize Size;

    public enum GoogleMapMarkerSize
    {
        Tiny,
        Small,
        Mid
    }
}

[Serializable]
public class GoogleMapPath
{
    public readonly bool fill = false;
    public readonly int weight = 1;
    public GoogleMapColor Color;
    public GoogleMapColor FillColor;
    public List<GoogleMapLocation> Locations;
}

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
