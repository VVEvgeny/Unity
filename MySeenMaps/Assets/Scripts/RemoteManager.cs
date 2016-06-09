using System;
using System.Collections;
using HTTP;
using UnityEngine;

namespace Assets.Scripts
{
    public class ReqAnswer
    {
        public Exception Exception;
        public Response Response;
    }

// ReSharper disable once ClassNeverInstantiated.Global
    public class RemoteManager : MonoBehaviour
    {
        public IEnumerator GetUrl(string url, Action<ReqAnswer> answer)
        {
            return GetUrl(url, false, answer);
        }
        public IEnumerator GetUrl(string url, bool useProxy, Action<ReqAnswer> answer)
        {
            if (useProxy) Request.proxy = new Uri(MySeenWebApi.Proxy);
            else Request.proxy = null;

            var req = new Request("GET", url, true);
            req.Send();
            while (!req.isDone) yield return null;

            if (req.exception == null)
            {
                //Debug.Log("responce=" + req.response.Text);
                answer(new ReqAnswer { Response = req.response });
            }
            else
            {
                Debug.LogError("RemoteManager.GetUrl exception=" + req.exception.Message);
                answer(new ReqAnswer { Exception = req.exception });
            }
        }
    }
}