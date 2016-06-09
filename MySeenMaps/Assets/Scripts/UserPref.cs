using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public static class UserPref
    {
        public static string UserKey
        {
            get
            {
                if (!PlayerPrefs.HasKey("UserKey")) PlayerPrefs.SetString("UserKey", string.Empty);
                return PlayerPrefs.GetString("UserKey");
            }
            set { PlayerPrefs.SetString("UserKey", value); }
        }

        public static List<MySeenWebApi.SyncJsonData> Roads { get; set; }
    }
}