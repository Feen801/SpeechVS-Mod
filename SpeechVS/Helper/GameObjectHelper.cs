using UnityEngine;

namespace SpeechVS;
public class GameObjectHelper
{
    public static GameObject GetGameObjectCheckFound(string path)
    {
        GameObject go = GameObject.Find(path);
        if (go == null)
        {
            SpeechVS.Logger.LogError(path + " gameobject not found");
        }
        return go;
    }
}

