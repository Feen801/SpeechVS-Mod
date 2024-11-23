using UnityEngine;

namespace MyAssetMod;
public class GameObjectHelper
{
    public static GameObject GetGameObjectCheckFound(string path)
    {
        GameObject go = GameObject.Find(path);
        if (go == null)
        {
            MyAssetMod.Logger.LogError(path + " gameobject not found");
        }
        return go;
    }
}

