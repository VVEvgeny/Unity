using System;
using UnityEngine;

[Serializable]
public class TileStyle
{
    public int Number;
    public Color32 TextColor;
    public Color32 TileColor;
}

public class TileStyleHolder : MonoBehaviour
{
    public TileStyle[] TileStyles;
    // SINGLETON
    public static TileStyleHolder Instance;

    private void Awake()
    {
        Instance = this;
    }
}