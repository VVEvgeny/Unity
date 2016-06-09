using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using HTTP;
using UnityEngine;

namespace Assets.Scripts
{
    public enum MapType
    {
        RoadMap,
        Satellite,
        Terrain,
        Hybrid
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

    public static class GMapManager
    {
        private const string Url = "http://maps.googleapis.com/maps/api/staticmap";
        private static readonly GoogleMapLocation CenterLocation = new GoogleMapLocation { Address = "Minsk" };
        //public static bool DoubleResolution = false;
        private static readonly MapType MapType = MapType.RoadMap;
        //public static List<GoogleMapMarker> Markers = new List<GoogleMapMarker>();
        public const int Size = 512;
        private const int Zoom = 11;

        public static string GetUrl(GoogleMapPath path)
        {
            var qs = "";
            //if (!autoLocateCenter)
            {
                if (!string.IsNullOrEmpty(CenterLocation.Address)) qs += "center=" + URL.Encode(CenterLocation.Address);
                else qs += "center=" + URL.Encode(string.Format("{0},{1}", CenterLocation.Latitude, CenterLocation.Longitude));
                qs += "&zoom=" + Zoom;
            }
            qs += "&size=" + URL.Encode(string.Format("{0}x{0}", Size));
            //qs += "&scale=" + (doubleResolution ? "2" : "1");
            qs += "&maptype=" + MapType.ToString().ToLower();
            qs += "&format=png32"; //только его не размывает

            //var usingSensor = false;
            //qs += "&sensor=" + (usingSensor ? "true" : "false");
            /*
        foreach (var i in markers)
        {
            qs += "&markers=" +
                  string.Format("size:{0}|color:{1}|label:{2}", i.Size.ToString().ToLower(), i.Color, i.Label);
            foreach (var loc in i.Locations)
            {
                if (loc.Address != "")
                    qs += "|" + URL.Encode(loc.Address);
                else
                    qs += "|" + URL.Encode(string.Format("{0},{1}", loc.Latitude, loc.Longitude));
            }
        }
        */

            if (path != null)
            {
                qs += "&path=" + string.Format("weight:{0}|color:{1}", path.weight, "0xff0000ff");
                if (path.fill) qs += "|fillcolor:" + path.FillColor;
                qs += "|enc:" + EncodePolyline.EncodeCoordinates(path.Locations);
            }
            qs += "&key=AIzaSyAzduON1ycPY7318RfjwIjI3vtnWN8xb_s";
            return Url + "?" + qs;
        }
        public static GoogleMapPath GetPathFromString(string coords)
        {
            var p = new GoogleMapPath();
            try
            {
                coords = coords.Trim();
                if (coords[coords.Length - 1] == ';') coords = coords.Remove(coords.Length - 1);

                p.Color = GoogleMapColor.red;

                var locat = new List<GoogleMapLocation>();
                foreach (var c in coords.Split(';'))
                {
                    var l = new GoogleMapLocation();
                    try
                    {
                        l.Latitude = float.Parse(c.Split(',')[0]);
                        l.Longitude = float.Parse(c.Split(',')[1]);
                        locat.Add(l);
                    }
                    catch (Exception e)
                    {
                        Debug.Log(e.Message + "=" + c);
                        throw;
                    }
                }
                p.Locations = locat;
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
                throw;
            }
            return p;
        }
    }

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

            while (index < polylineChars.Length)
            {
                // calculate next latitude
                var sum = 0;
                var shifter = 0;
                int next5Bits;
                do
                {
                    next5Bits = polylineChars[index++] - 63;
                    sum |= (next5Bits & 31) << shifter;
                    shifter += 5;
                } while (next5Bits >= 32 && index < polylineChars.Length);

                if (index >= polylineChars.Length)
                    break;

                currentLat += (sum & 1) == 1 ? ~(sum >> 1) : (sum >> 1);

                //calculate next longitude
                sum = 0;
                shifter = 0;
                do
                {
                    next5Bits = polylineChars[index++] - 63;
                    sum |= (next5Bits & 31) << shifter;
                    shifter += 5;
                } while (next5Bits >= 32 && index < polylineChars.Length);

                if (index >= polylineChars.Length && next5Bits >= 32)
                    break;

                currentLng += (sum & 1) == 1 ? ~(sum >> 1) : (sum >> 1);

                yield return new GoogleMapLocation
                {
                    Latitude = float.Parse((Convert.ToDouble(currentLat) / 1E5).ToString(CultureInfo.InvariantCulture)),
                    Longitude = float.Parse((Convert.ToDouble(currentLng) / 1E5).ToString(CultureInfo.InvariantCulture))
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

            var encodeDiff = (Action<int>)(diff =>
            {
                var shifted = diff << 1;
                if (diff < 0)
                    shifted = ~shifted;

                var rem = shifted;

                while (rem >= 0x20)
                {
                    str.Append((char)((0x20 | (rem & 0x1f)) + 63));

                    rem >>= 5;
                }

                str.Append((char)(rem + 63));
            });

            var lastLat = 0;
            var lastLng = 0;

            foreach (var point in points)
            {
                var lat = (int)Math.Round(point.Latitude * 1E5);
                var lng = (int)Math.Round(point.Longitude * 1E5);

                encodeDiff(lat - lastLat);
                encodeDiff(lng - lastLng);

                lastLat = lat;
                lastLng = lng;
            }

            return str.ToString();
        }
    }
}