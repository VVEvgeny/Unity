using System.Collections;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using Assets.Scripts;

// ReSharper disable once CheckNamespace
// ReSharper disable once UnusedMember.Global
public class GMapController : MonoBehaviour
{
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public void ZoomIn()
    {
        GMapManager.Zoom++;
        LoadBaseImage();
    }

    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public void ZoomOut()
    {
        GMapManager.Zoom--;
        LoadBaseImage();
    }

    private IEnumerator GetImage()
    {
        var manager = FindObjectOfType<Manager>();
        Manager.ActionResults act = null;
        StartCoroutine(manager.GetImage(null, 0, x => act = x));
        while (act == null) yield return null;

        GMapManager.BaseTexture = act.Texture;
        manager.ApplyTexture(GMapManager.BaseTexture);
    }
    private void LoadBaseImage()
    {
        StartCoroutine(GetImage());
    }
}
