using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts;
using Assets.Scripts.Database;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;
// ReSharper disable UnusedMember.Global

// ReSharper disable once CheckNamespace
// ReSharper disable once UnusedMember.Global
public class Manager : MonoBehaviour
{
    // ReSharper disable once MemberCanBePrivate.Global
    // ReSharper disable once UnassignedField.Global
    public InputField EmailField;
    // ReSharper disable once MemberCanBePrivate.Global
    // ReSharper disable once UnassignedField.Global
    public GameObject LoginPanel;
    // ReSharper disable once MemberCanBePrivate.Global
    // ReSharper disable once UnassignedField.Global
    public GameObject MainPanel;
    // ReSharper disable once MemberCanBePrivate.Global
    // ReSharper disable once UnassignedField.Global
    public GameObject MapPanel;
    // ReSharper disable once MemberCanBePrivate.Global
    // ReSharper disable once UnassignedField.Global
    public InputField PasswordField;
    // ReSharper disable once UnassignedField.Global
    // ReSharper disable once MemberCanBePrivate.Global
    public Text RoadsCountText;
    // ReSharper disable once UnassignedField.Global
    // ReSharper disable once MemberCanBePrivate.Global
    public Dropdown RoadTypeDropdown;

    private RemoteManager _remoteManager;

    // ReSharper disable once UnusedMember.Local
    private void Start()
    {
        StartCoroutine(Starting());

        Database.Get.ToString();
    }

    // ReSharper disable once UnusedMember.Local
    private void Awake()
    {
        _remoteManager = FindObjectOfType<RemoteManager>();
    }

    private IEnumerator Starting()
    {
        var worked = false;
        if (!string.IsNullOrEmpty(UserPref.UserKey))
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

    // ReSharper disable once UnusedMember.Global
    public void ShowOnMap()
    {
        StartCoroutine(ShowOnMapCoroutine());
    }

    // ReSharper disable once UnusedMember.Global
    public void ChangeRoadType()
    {
        RoadsCountText.text = UserPref.Roads.Count(r => r.Type == RoadTypeDropdown.value).ToString();
    }

    private IEnumerator ShowOnMapCoroutine()
    {
        if (UserPref.Roads.Count(r => r.Type == RoadTypeDropdown.value || RoadTypeDropdown.value == 0) > 0)
        {
            var worked = false;
            StartCoroutine(
                Process(
                    UserPref.Roads.Where(r => r.Type == RoadTypeDropdown.value || RoadTypeDropdown.value == 0)
                        .Select(r => r.Coordinates)
                        .ToList(), x => worked = x));
            while (!worked) yield return null;
        }
    }

    private IEnumerator Process(IList<string> paths, Action<bool> worked)
    {
        if (GMapManager.BaseTexture == null)
        {
            StartCoroutine(GetImage(null, 0, x => GMapManager.BaseTexture = x.Texture));
            while (GMapManager.BaseTexture == null) yield return null;
        }

        var outTexture = Instantiate(GMapManager.BaseTexture);
        var points = new List<Point>();

        if (paths != null)
        {
            var pathTextures = new Texture2D[paths.Count];
            //for (var i = 0; i < paths.Count; i++) pathTextures[i] = null;

            for (var i = 0; i < paths.Count; i++)
            {
                StartCoroutine(GetImage(GMapManager.GetPathFromString(paths[i]), i, x => { pathTextures[x.Index] = x.Texture; }));
                if (i%9 == 0) yield return new WaitForSeconds(1f); //не более 9 потоков за сек
            }
            while (pathTextures.Any(p => p == null)) yield return null;

            //Debug.Log("all data recieved");

            for (var i = 0; i < paths.Count; i++)
            {
                for (var w = 0; w <= GMapManager.Size; w++)
                {
                    for (var h = 0; h <= GMapManager.Size; h++)
                    {
                        if (GMapManager.BaseTexture.GetPixel(w, h) != pathTextures[i].GetPixel(w, h))
                        {
                            //if (points.Count < 10000)
                            if (!points.Contains(new Point {X = w, Y = h})) points.Add(new Point {X = w, Y = h});
                        }
                    }
                }
            }
        }
        //Debug.Log("points=" + points.Count);
        foreach (var p in points) outTexture.SetPixel(p.X, p.Y, Color.red);
        outTexture.Apply();

        ApplyTexture(outTexture);

        worked(true);
        //Debug.Log("process END");
    }

    public void ApplyTexture(Texture2D texture)
    {
        var spr = Sprite.Create(texture, new Rect(0, 0, GMapManager.Size, GMapManager.Size), new Vector2(GMapManager.Size, GMapManager.Size));
        MapPanel.GetComponent<Image>().sprite = spr;
    }

    public IEnumerator GetImage(GoogleMapPath path, int index, Action<ActionResults> action)
    {
        //Debug.Log("act=" + threadId + " BEGIN");

        var image = new Texture2D(GMapManager.Size, GMapManager.Size);

        ReqAnswer req = null;
        StartCoroutine(_remoteManager.GetUrl(GMapManager.GetUrl(path), true, x => req = x));
        while (req == null) yield return null;
        
        if (req.Exception == null) image.LoadImage(req.Response.Bytes);
        action(new ActionResults(image, index));
    }

    private IEnumerator Get(Action<bool> endWork)
    {
        var worked = false;
        StartCoroutine(GetRoads(x => worked = x));
        while (!worked) yield return null;

        //Debug.Log("after get isOk=" + isOkAction);
        endWork(true);
    }

    private IEnumerator GetRoads(Action<bool> worked)
    {
        //Debug.Log("Начинаю запрос всех путей");

        var url = MySeenWebApi.ApiHost + MySeenWebApi.ApiSync + UserPref.UserKey + "/" +
                  (int)MySeenWebApi.SyncModesApiSync.GetRoads + "/" + MySeenWebApi.ApiVersion;

        ReqAnswer req = null;
        StartCoroutine(_remoteManager.GetUrl(url, x => req = x));
        while (req == null) yield return null;

        if (req.Exception == null)
        {
            try
            {
                var answer = JsonConvert.DeserializeObject<List<MySeenWebApi.SyncJsonData>>(req.Response.Text);
                //Debug.LogWarning("count=" + answer.Count);
                RoadsCountText.text = answer.Count.ToString();
                UserPref.Roads = answer;
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }
        worked(true);
    }

    // ReSharper disable once UnusedMember.Global
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
        StartCoroutine(CheckUserKey(UserPref.UserKey, x => work = x, x => isKeyOk = x));
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
                  (int)MySeenWebApi.SyncModesApiUsers.IsUserExists + "/" + MySeenWebApi.ApiVersion;

        ReqAnswer req = null;
        StartCoroutine(_remoteManager.GetUrl(url, x => req = x));
        while (req == null) yield return null;

        if (req.Exception == null)
        {
            try
            {
                var answer = JsonConvert.DeserializeObject<MySeenWebApi.SyncJsonAnswer>(req.Response.Text);
                if (answer.Value == MySeenWebApi.SyncJsonAnswer.Values.Ok) isKeyOk(true);
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }
        worked(true);
    }

    private IEnumerator GetUserKey(string login, string password, Action<bool> worked)
    {
        //Debug.Log("GetUserKey");

        //for local test
        //login = "vvevgeny@gmail.555";
        //password = "vvevgeny@gmail.555A";

        ReqAnswer req = null;
        var url = MySeenWebApi.ApiHost + MySeenWebApi.ApiLogin + login + ";" + password + "/" +
                  (int) MySeenWebApi.SyncModesApiLogin.GetKey + "/" + MySeenWebApi.ApiVersion;

        StartCoroutine(_remoteManager.GetUrl(url, x => req = x));
        while (req == null) yield return null;

        if (req.Exception == null)
        {
            try
            {
                var answer = JsonConvert.DeserializeObject<MySeenWebApi.SyncJsonAnswer>(req.Response.Text);
                if (answer.Value == MySeenWebApi.SyncJsonAnswer.Values.Ok && !string.IsNullOrEmpty(answer.Data))
                    UserPref.UserKey = answer.Data;
            }
            catch (Exception e)
            {
                 Debug.LogError(e.Message);
            }
        }
        worked(true);
    }

    public class ActionResults
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