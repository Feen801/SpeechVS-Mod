using System.IO;
using UnityEngine;

namespace MyAssetMod;
public class Assets
{
    static AssetBundle assets;
    public static GameObject hair;
    public static Texture2D eyes;
    public static void LoadAssets()
    {
        //Find where the asset bundle is, and load it.
        string assetPath = Path.Combine(Application.streamingAssetsPath, "your_asset_bundle_name");
        MyAssetMod.Logger.LogInfo("Loading assets from " + assetPath);
        assets = AssetBundle.LoadFromFile(assetPath);

        //I had a hair Prefab called "hair" and a texture called "eyes"
        hair = assets.LoadAsset<GameObject>("hair");
        eyes = assets.LoadAsset<Texture2D>("eyes");
    }
}
