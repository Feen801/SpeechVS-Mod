using System.IO;
using UnityEngine;

namespace MyAssetMod;
public class Assets
{
    static AssetBundle assets;
    public static GameObject hair;
    public static Texture2D eyes;
    public static GameObject background;
    public static Texture2D skin;
    public static void LoadAssets()
    {
        //Find where the asset bundle is, and load it.
        //PUT THE NAME OF YOUR ASSET BUNDLE
        string assetPath = Path.Combine(Application.streamingAssetsPath, "myassetbundle");
        MyAssetMod.Logger.LogInfo("Loading assets from " + assetPath);
        assets = AssetBundle.LoadFromFile(assetPath);

        //3D Prefab called "hair"
        hair = assets.LoadAsset<GameObject>("Hair");
        //2D texture called "RedEye"
        eyes = assets.LoadAsset<Texture2D>("RedEye");
        //3D Prefab called "Prison"
        background = assets.LoadAsset<GameObject>("Prison");
    }
}
