using System.Collections.Generic;
using UnityEngine;

public class UserPref : MonoBehaviour
{
    public static UserPref Instance;
    public string UserKey
    {
        get { return PlayerPrefs.GetString("UserKey"); }
        set { PlayerPrefs.SetString("UserKey", value); }
    }

    public List<MySeenWebApi.SyncJsonData> Roads { get; set; }

    private void Awake()
    {
        Instance = this;
        if (!PlayerPrefs.HasKey("UserKey")) PlayerPrefs.SetString("UserKey",string.Empty);
    }
}