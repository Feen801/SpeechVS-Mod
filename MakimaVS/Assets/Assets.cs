using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.IO;
using UnityEngine;

namespace MakimaVS;
public class MakimaAssets
{
    static AssetBundle makimaAssets;
    public static GameObject hair;
    public static Texture2D eyes;
    public static void LoadAssets()
    {
        string assetPath = Path.Combine(Application.streamingAssetsPath, "makimaassets");
        MakimaVS.Logger.LogInfo("Loading Makima assets at " + assetPath);
        makimaAssets = AssetBundle.LoadFromFile(assetPath);
        hair = makimaAssets.LoadAsset<GameObject>("Makima_Hair");
        eyes = makimaAssets.LoadAsset<Texture2D>("Makima_Eyes");
    }
}
