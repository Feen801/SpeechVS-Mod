using UnityEngine;

namespace MakimaVS;
public class GameObjectHelper
{
    public static GameObject GetGameObjectCheckFound(string path)
    {
        GameObject go = GameObject.Find(path);
        if (go == null)
        {
            MakimaVS.Logger.LogError(path + " gameobject not found");
        }
        return go;
    }
}

