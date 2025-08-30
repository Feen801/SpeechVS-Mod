using System.IO;
using UnityEngine;

namespace SpeechVS;
public class Assets
{
    static AssetBundle assets;
    public static void LoadAssets()
    {
        string assetPath = Path.Combine(Application.streamingAssetsPath, "");
        SpeechVS.Logger.LogInfo("Loading assets from " + assetPath);
        assets = AssetBundle.LoadFromFile(assetPath);
    }
}
