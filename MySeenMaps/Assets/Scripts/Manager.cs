using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HTTP;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Manager : MonoBehaviour
{
    private readonly string url = "http://maps.googleapis.com/maps/api/staticmap";

    private Texture2D baseTexture;

    private readonly GoogleMapLocation centerLocation = new GoogleMapLocation {Address = "Minsk"};
    private readonly bool doubleResolution = false;

    public InputField EmailField;
    public GameObject LoginPanel;
    public GameObject MainPanel;
    public GameObject MapPanel;
    private readonly MapType mapType = MapType.RoadMap;
    private readonly List<GoogleMapMarker> markers = new List<GoogleMapMarker>();
    public InputField PasswordField;
    public Text RoadsCountText;
    private readonly int size = 512;
    private readonly int zoom = 11;

    public enum MapType
    {
        RoadMap,
        Satellite,
        Terrain,
        Hybrid
    }

    private void Start()
    {
        StartCoroutine(Starting());
    }

    private IEnumerator Starting()
    {
        var worked = false;
        if (!string.IsNullOrEmpty(UserPref.Instance.UserKey))
        {
            StartCoroutine(Logining(true, x => worked = x));
            while (!worked) yield return null;

            worked = false;
            StartCoroutine(Get(x => worked = x));
            while (!worked) yield return null;

            worked = false;
            StartCoroutine(Process(null, x => worked = x));
            while (!worked) yield return null;
        }
    }

    public void ShowOnMap()
    {
        StartCoroutine(ShowOnMapCoroutine());
    }

    private IEnumerator ShowOnMapCoroutine()
    {
        var worked = false;
        //StartCoroutine(ShowOnMapProcess(x => worked = x));
        StartCoroutine(Process(UserPref.Instance.Roads.Select(road => road.Coordinates).ToList(), x => worked = x));
        while (!worked) yield return null;
    }

    private GoogleMapPath GetPathFromString(string coords)
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

    private bool HaveEmpty(IEnumerable<Texture2D> pathTextures)
    {
        return pathTextures.Any(p => p == null);
    }
    private IEnumerator Process(IList<string> paths, Action<bool> worked)
    {
        var threadId = 0;

        if (baseTexture == null)
        {
            StartCoroutine(GetImage(threadId++, null, 0, x => baseTexture = x.Texture));
            while (baseTexture == null) yield return null;
        }

        var outTexture = baseTexture;
        var points = new List<Point>();

        if (paths != null)
        {
            var pathTextures = new Texture2D[paths.Count];
            for (var i = 0; i < paths.Count; i++) pathTextures[i] = null;

            for (var i = 0; i < paths.Count; i++)
            {
                StartCoroutine(GetImage(threadId++, GetPathFromString(paths[i]), i,
                    x => { pathTextures[x.Index] = x.Texture; }));
                if (i%9 == 0) yield return new WaitForSeconds(1f); //не более 9 потоков за сек
            }
            while (HaveEmpty(pathTextures)) yield return null;

            //Debug.Log("all data recieved");

            for (var i = 0; i < paths.Count; i++)
            {
                for (var w = 0; w <= size; w++)
                {
                    for (var h = 0; h <= size; h++)
                    {
                        if (baseTexture.GetPixel(w, h) != pathTextures[i].GetPixel(w, h))
                        {
                            //if (points.Count < 10000)
                            if (!points.Contains(new Point(w, h))) points.Add(new Point(w, h));
                        }
                    }
                }
            }
        }
        //Debug.Log("points=" + points.Count);
        foreach (var p in points) outTexture.SetPixel(p.x, p.y, Color.red);
        outTexture.Apply();

        var spr = Sprite.Create(outTexture, new Rect(0, 0, size, size), new Vector2(size, size));
        MapPanel.GetComponent<Image>().sprite = spr;

        worked(true);
        //Debug.Log("process END");
    }

    private IEnumerator GetImage(int threadId, GoogleMapPath path, int index, Action<ActionResults> action)
    {
        //Debug.Log("act=" + threadId + " BEGIN");

        var image = new Texture2D(size, size);
        var qs = "";
        //if (!autoLocateCenter)
        {
            if (!string.IsNullOrEmpty(centerLocation.Address)) qs += "center=" + URL.Encode(centerLocation.Address);
            else
                qs += "center=" +
                      URL.Encode(string.Format("{0},{1}", centerLocation.Latitude, centerLocation.Longitude));
            qs += "&zoom=" + zoom;
        }
        qs += "&size=" + URL.Encode(string.Format("{0}x{0}", size));
        qs += "&scale=" + (doubleResolution ? "2" : "1");
        qs += "&maptype=" + mapType.ToString().ToLower();
        qs += "&format=png32"; //только его не размывает

        //var usingSensor = false;
        //qs += "&sensor=" + (usingSensor ? "true" : "false");

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

        if (path != null)
        {
            qs += "&path=" + string.Format("weight:{0}|color:{1}", path.weight, "0xff0000ff");
            if (path.fill) qs += "|fillcolor:" + path.FillColor;
            qs += "|enc:" + EncodePolyline.EncodeCoordinates(path.Locations);
        }
        qs += "&key=AIzaSyAzduON1ycPY7318RfjwIjI3vtnWN8xb_s";

        //Debug.Log("before get =" + url + "?" + qs);
        //Debug.Log("url len=" + (url + "?" + qs).Length);

        Request.proxy = new Uri(MySeenWebApi.Proxy);
        var req = new Request("GET", url + "?" + qs, true);
        req.Send();

        while (!req.isDone) yield return null;

        //Debug.Log("after get=" + req.response.Bytes.Length);
        try
        {
            //Debug.Log("act=" + threadId + " after get data=" + req.response.message);
        }
        catch
        {
            // ignored
        }

        if (req.exception == null) image.LoadImage(req.response.Bytes);
        //Debug.Log("act=" + threadId + " END");
        action(new ActionResults(image, index));
    }

    private IEnumerator Get(Action<bool> endWork)
    {
        var worked = false;
        var isOkAction = false;
        StartCoroutine(GetRoads(x => worked = x, x => isOkAction = x));
        while (!worked) yield return null;

        //Debug.Log("after get isOk=" + isOkAction);
        endWork(true);
    }

    private IEnumerator GetRoads(Action<bool> worked, Action<bool> isOkAction)
    {
        //Debug.Log("Начинаю запрос всех путей");

        var url = MySeenWebApi.ApiHost + MySeenWebApi.ApiSync + UserPref.Instance.UserKey + "/" +
                  (int)MySeenWebApi.SyncModesApiSync.GetRoads + "/" + MySeenWebApi.ApiVersion;

        //Debug.Log("url=" + url);

        Request.proxy = null;
        var req = new Request("GET", url, true);
        req.Send();
        while (!req.isDone) yield return null;

        if (req.exception == null)
        {
            //Debug.Log("responce=" + req.response.Text);

            try
            {
                var answer = JsonConvert.DeserializeObject<List<MySeenWebApi.SyncJsonData>>(req.response.Text);
                //Debug.LogWarning("count=" + answer.Count);
                RoadsCountText.text = answer.Count.ToString();
                UserPref.Instance.Roads = answer;
                isOkAction(true);
            }
            catch (Exception e)
            {
                Debug.LogError("ex Deserialize=" + e.Message);
            }
        }
        else
        {
            Debug.Log("after get ex=" + req.exception.Message);
        }

        worked(true);
    }

    public void Login()
    {
        StartCoroutine(Login(false));
    }

    private IEnumerator Login(bool skipGet)
    {
        var worked = false;
        StartCoroutine(Logining(skipGet, x => worked = x));
        while (!worked) yield return null;

        StartCoroutine(Starting());
    }

    private IEnumerator Logining(bool skipGet, Action<bool> worked)
    {
        var work = false;
        if (!skipGet)
        {
            StartCoroutine(GetUserKey(EmailField.text, PasswordField.text, x => work = x));

            while (!work) yield return null;
            //Debug.Log("Ключ получили, чекаем ="+ UserPref.Instance.UserKey);
            work = false;
        }
        var isKeyOk = false;
        StartCoroutine(CheckUserKey(UserPref.Instance.UserKey, x => work = x, x => isKeyOk = x));
        while (!work) yield return null;
        //Debug.Log("ключ прочекали результат=" + isKeyOk);

        if (isKeyOk)
        {
            MainPanel.SetActive(true);
            LoginPanel.SetActive(false);
        }
        worked(true);
    }

    private IEnumerator CheckUserKey(string userKey, Action<bool> worked, Action<bool> isKeyOk)
    {
        //Debug.Log("CheckUserKey");
        var url = MySeenWebApi.ApiHost + MySeenWebApi.ApiUsers + userKey + "/" +
                  (int)MySeenWebApi.SyncModesApiUsers.IsUserExists + "/" + (int)MySeenWebApi.ApiVersion;
        //Debug.Log(url);

        Request.proxy = null;
        var req = new Request("GET", url, true);
        req.Send();
        while (!req.isDone) yield return null;

        if (req.exception == null)
        {
            //Debug.Log("responce=" + req.response.Text);
            try
            {
                var answer = JsonConvert.DeserializeObject<MySeenWebApi.SyncJsonAnswer>(req.response.Text);
                if (answer.Value == MySeenWebApi.SyncJsonAnswer.Values.Ok) isKeyOk(true);
            }
            catch (Exception)
            {
                // ignored
            }
        }
        else
        {
            Debug.Log("after get ex=" + req.exception.Message);
        }
        worked(true);
    }

    private IEnumerator GetUserKey(string login, string password, Action<bool> worked)
    {
        //Debug.Log("GetUserKey");

        //for local test
        //login = "vvevgeny@gmail.555";
        //password = "vvevgeny@gmail.555A";

        var url = MySeenWebApi.ApiHost + MySeenWebApi.ApiLogin + login + ";" + password + "/" +
                  (int)MySeenWebApi.SyncModesApiLogin.GetKey + "/" + MySeenWebApi.ApiVersion;

        Request.proxy = null;
        var req = new Request("GET", url, true);
        req.Send();
        while (!req.isDone) yield return null;

        if (req.exception == null)
        {
            //Debug.Log("responce=" + req.response.Text);
            var answer = JsonConvert.DeserializeObject<MySeenWebApi.SyncJsonAnswer>(req.response.Text);
            if (answer.Value == MySeenWebApi.SyncJsonAnswer.Values.Ok && !string.IsNullOrEmpty(answer.Data)) UserPref.Instance.UserKey = answer.Data;
        }
        else
        {
            Debug.Log("after get ex=" + req.exception.Message);
        }
        worked(true);
    }

    private class ActionResults
    {
        public readonly int Index;
        public readonly Texture2D Texture;

        public ActionResults(Texture2D texture, int index)
        {
            Texture = texture;
            Index = index;
        }
    }
}