using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.Mono;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

namespace MyAssetMod
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    public class MyAssetMod : BaseUnityPlugin
    {
        public static new ManualLogSource Logger;
        private static GameObject hair;
        private static Texture2D eye_texture;

        private void Awake()
        {
            /* 
             * Create logger object. You can use it to print to the console window with
             * LogInfo.LogInfo("message")
             */
            Logger = base.Logger;
            Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");

            // Load the assets into memory from your asset bundle file created in Unity.
            Assets.LoadAssets();

            // This makes the "OnSceneLoaded" function call when a scene is loaded.
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            /* This checks if the scene being loaded is the "ExtraLoadScene" scene. It loads only after the entire session
             * scene has been loaded but before the loading screen dissapears, so it is a good time to override assets.
             */
            if (Equals(scene.name, Constants.SessionScene))
            {
                /*
                 * Create an instance of the hair asset in the 3D world. We will still need to set it's position and parent, if we want
                 * it to actually move with her head.
                 */
                hair = GameObject.Instantiate(Assets.hair);

                /* 
                 * GameObjectHelper.GetGameObjectCheckFound finds a game object given part of its location in the scene tree.
                 * You should generally at least provide the gameobject you want to find (e.g. Hair1) and its parent (e.g. HairPosition)
                 * to avoid name conflicts.
                 * If it can't find a GameObject, an error will be sent to the console.
                 * 
                 * This function cannot find game objects that are not currently active.
                 * For that, you would need to find a parent GameObject that is enabled, get its transform with gameobject.transform,
                 * then use transform.Find("name") on the transform to get one of its children.
                 * See https://docs.unity3d.com/6000.0/Documentation/ScriptReference/Transform.Find.html
                 */
                GameObject base_hair = GameObjectHelper.GetGameObjectCheckFound("HairPosition/Hair1");
                GameObject other_hair = GameObjectHelper.GetGameObjectCheckFound("HairPosition/AltHairs");

                /*
                 * Disable any active hair. You should do this if you want to load a custom hairstyle.
                 */
                if (base_hair != null)
                {
                    base_hair.SetActive(false);
                    foreach (Transform hairstyle in base_hair.transform)
                    {
                        hairstyle.gameObject.SetActive(false);
                    }
                }
                if (other_hair != null)
                {
                    other_hair.SetActive(false);
                    foreach (Transform hairstyle in other_hair.transform)
                    {
                        hairstyle.gameObject.SetActive(false);
                    }
                }

                //This is the object we want to parent the hair asset to.
                GameObject hair_pos = GameObjectHelper.GetGameObjectCheckFound("HairPosition");
                //Then we set the transform of our hair asset to be parented to the HairPosition GameObject. This means the hair will move with her haid now.
                hair.transform.SetParent(hair_pos.transform, false);

                //I found these positional values to work best for my hair asset. You will have to play with them to find the right positioning.
                /*
                 * The Quaternion.identity is just zero rotation. If you need something else:
                 * Quaternion myRotation = Quaternion.identity;
                 * myRotation.eulerAngles = new Vector3(0, 0, somezrotation);
                 * is probably the easiest way to do it. 
                 * Or you could just change the rotation of the hair in the editor before you export the asset bundle.
                 */
                hair.transform.SetLocalPositionAndRotation(new Vector3(-0.002f, -0.025f, 0.031f), Quaternion.identity);

                /*
                 * Unlike a Prefab GameObject, you don't need to instance textures.
                 */
                eye_texture = Assets.eyes;
                //The material that holds the eye texture is on the Face GameObject.
                GameObject face = GameObjectHelper.GetGameObjectCheckFound("GirlCharacter/Face");
                face.GetComponent<SkinnedMeshRenderer>().materials[5].SetTexture("_MainTex", eye_texture);
                
                /*
                 * This prevents the hearts from appearing on her eyes when in the "thirsty" mood.
                 * You may want to do this depending on the character you are going for.
                 * 
                 * In general this method is a good way to disable features you can't turn off normally,
                 * because sometimes just disabling a GameObject won't work because an FSM will re-enable it.
                 * You will probably have to poke around and experiment to find the right FSM to empty out.
                 */
                //Finds the FSM responsible for enabling the hearts (the 6th one attached to the face)
                PlayMakerFSM thirsty_eyes = face.GetComponents<PlayMakerFSM>()[5];
                // Removes all of its actions in each event.
                foreach (HutongGames.PlayMaker.FsmState state in thirsty_eyes.FsmStates)
                {
                    state.Actions = [];
                }

                //Change the "Yes" button text.
                EditButton("Yes");
            }
        }

        /*
         * This shows how to change some text on an object.
         * This specific method may not always work for all text, sometimes the text is reset by a script or FSM.
         */
        private void EditButton(string button_name)
        {
            GameObject buttons = GameObjectHelper.GetGameObjectCheckFound("Positives ------------/");
            Transform button = buttons.transform.Find(button_name + "/DoneBG/DoneText/Text (TMP)");

            button.gameObject.GetComponent<TextMeshProUGUI>().SetText("Yeah");
        }
    }
}
