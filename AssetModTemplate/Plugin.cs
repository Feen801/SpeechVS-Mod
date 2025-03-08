using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.Mono;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System;

namespace MyAssetMod
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    public class MyAssetMod : BaseUnityPlugin
    {
        public static new ManualLogSource Logger;
        private static GameObject face;
        private static GameObject body;

        private static GameObject prison;
        private static GameObject hair;
        private static Texture2D eye_texture;
        private static Texture2D skin_texture;

        private static Material skinmaterial;
        private static Material faceskinmaterial;

        private static bool inSession = false;

        //Disable the default in game background
        private static bool DISABLE_DEFAULT_BACKGROUND = true;

        //Disable default game hair
        private static bool DISABLE_OTHER_HAIR = true;

        //Heart eyes when thirsty
        private static bool DISABLE_HEARTS = false;

        //Override eye glow, make it one color or disable so it doesn't happen during moods.
        private static bool CUSTOM_GLOW = false;
        //R G B A
        //0-1 (not 255)
        private static Color CUSTOM_GLOW_COLOR = new Color(0, 0, 0, 0);

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
        void LateUpdate()
        {
            if (inSession)
            {
                //Set the custom glow color for eyes
                if (CUSTOM_GLOW)
                {
                    if (face == null)
                    {
                        face = GameObjectHelper.GetGameObjectCheckFound("GirlCharacter/Face");
                    }
                    Material m = face.GetComponent<SkinnedMeshRenderer>().materials[1];
                    m.SetColor("_EMISSION", CUSTOM_GLOW_COLOR);
                }

                // Nail emission color and body color example
                // Delete this code if you dont want to change them
                if (body == null)
                {
                    body = GameObjectHelper.GetGameObjectCheckFound("GirlCharacter/Body");
                }
                //nail
                body.GetComponent<SkinnedMeshRenderer>().materials[2].SetColor("_Emission", new Color(0, 0, 0.3f, 1));
                //body
                body.GetComponent<SkinnedMeshRenderer>().materials[0].SetColor("_Color", new Color(0.2f, 0.2f, 1, 1));
            }
        }

        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            /* This checks if the scene being loaded is the "ExtraLoadScene" scene. It loads only after the entire session
             * scene has been loaded but before the loading screen dissapears, so it is a good time to override assets.
             */
            Logger.LogInfo($"Scene {scene.name} is loaded!");
            if (Equals(scene.name, Constants.SessionScene))
            {
                inSession = true;

                //Add the custom background
                ChangeBackground(Assets.background);

                //Update Colors
                ChangeColors();

                //Add the custom hair
                ChangeHair(Assets.hair);

                //Update eye texture
                ChangeEyes(Assets.eyes);
                
                //Change the "Yes" button text to "Yeah"
                //This is just to show how to mess with UI stuff
                EditButton("Yes", "Yeah");
            }
            else
            {
                inSession = false;
            }
        }

        private void ChangeBackground(GameObject background)
        {
            /*
            * Create an instance of the background asset in the 3D world.
            */
            prison = GameObject.Instantiate(background);

            if (DISABLE_DEFAULT_BACKGROUND)
            {
                try
                {
                    GameObject defaultbg = GameObjectHelper.GetGameObjectCheckFound("Root/BasicWorld");
                    defaultbg.SetActive(false);
                    defaultbg = GameObjectHelper.GetGameObjectCheckFound("Root/World");
                    defaultbg.SetActive(false);
                }
                catch (Exception ex)
                {
                    //if the background was already disabled in settings
                }
            }

            //Put the prison in the session scene
            GameObject root = GameObjectHelper.GetGameObjectCheckFound("Root");
            prison.transform.SetParent(root.transform);
            //I found these positional values to work best for my hair background
            prison.transform.localPosition = new Vector3(0.3618f, 5.8927f, 34.4546f);
        }

        private void ChangeColors() {
            /*
             * These are a bunch of materials and parameters you can change the color of!
            */
            GameObject body = GameObjectHelper.GetGameObjectCheckFound("GirlCharacter/Body");
            GameObject face = GameObjectHelper.GetGameObjectCheckFound("GirlCharacter/Face");
            // Main body color
            // VS repeatedly sets this from somewhere else, similar to eye emission color.
            // SO MOVE THIS TO THE LateUpdate() FUNCTION TO MAKE IT ACTUALLY WORK
            body.GetComponent<SkinnedMeshRenderer>().materials[0].SetColor("_Color", new Color(0.2f, 0.2f, 1, 1));

            // Nail color
            body.GetComponent<SkinnedMeshRenderer>().materials[2].SetColor("_Color", new Color(0, 0, 1, 1));
            // Nail emmision color
            // VS repeatedly sets this from somewhere else, similar to eye emission color.
            // SO MOVE THIS TO THE LateUpdate() FUNCTION TO MAKE IT ACTUALLY WORK
            body.GetComponent<SkinnedMeshRenderer>().materials[2].SetColor("_Emission", new Color(0, 0, 0.3f, 1));

            // Face color, you probably want this to be the same as body color
            face.GetComponent<SkinnedMeshRenderer>().materials[7].SetColor("_Color", new Color(0.2f, 0.2f, 1, 1));
            // Blush color. The texture is already moslty red, so the colors are a bit finicky when edited. This will make it green.
            face.GetComponent<SkinnedMeshRenderer>().materials[7].SetColor("_FlushColor", new Color(0, 2, 0, 1));
        }

        private void ChangeHair(GameObject hairAsset)
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
             * !!!NOTE: This function cannot find game objects that are not currently active.
             * For that, you would need to find a parent GameObject that is enabled, get its transform with gameobject.transform,
             * then use transform.Find("name") on the transform to get one of its children.
             * See https://docs.unity3d.com/6000.0/Documentation/ScriptReference/Transform.Find.html
             */
            Logger.LogInfo("hair 1");
            GameObject base_hair = GameObjectHelper.GetGameObjectCheckFound("HairPosition/Hair1");
            Logger.LogInfo("hair 2");
            GameObject other_hair = GameObjectHelper.GetGameObjectCheckFound("HairPosition/AltHairs");
            Logger.LogInfo("hair done");

            /*
             * Disable any active hair. You should do this if you want to load a custom hairstyle.
             */
            if (DISABLE_OTHER_HAIR) {
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
            }

            //This is the object we want to parent the hair asset to.
            GameObject hair_pos = GameObjectHelper.GetGameObjectCheckFound("HairPosition");
            //Then we set the transform of our hair asset to be parented to the HairPosition GameObject. This means the hair will move with her haid now.
            hair.transform.SetParent(hair_pos.transform, false);

            //I found these positional values to work best for my hair asset. You will have to play with them in UnityExplorer to find the right positioning for yours.
            //You may also need to adjust scale.
            /*
             * The Quaternion.identity is just zero rotation. If you need something else:
             * Quaternion myRotation = Quaternion.identity;
             * myRotation.eulerAngles = new Vector3(0, 0, somezrotation);
             * is probably the easiest way to do it. 
             * Or you could just change the rotation of the hair in the editor before you export the asset bundle.
             */
            hair.transform.SetLocalPositionAndRotation(new Vector3(-0.002f, -0.025f, 0.031f), Quaternion.identity);
            //To adjust scale, if you need to:
            //hair.transform.localScale = new Vector3(1, 1, 1);
        }

        private void ChangeEyes(Texture2D eyeTexture)
        {
            /*
             * Unlike a Prefab GameObject, you don't need to instance textures.
            */
            eye_texture = eyeTexture;
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
            if (DISABLE_HEARTS)
            {
                PlayMakerFSM thirsty_eyes = face.GetComponents<PlayMakerFSM>()[5];
                // Removes all of its actions in each event.
                foreach (HutongGames.PlayMaker.FsmState state in thirsty_eyes.FsmStates)
                {
                    state.Actions = [];
                }
            }
        }

        /*
         * This shows how to change some text on an object.
         * This specific method may not always work for all text, sometimes the text is reset by a script or FSM.
         */
        private void EditButton(string button_name, string new_text)
        {
            GameObject buttons = GameObjectHelper.GetGameObjectCheckFound("Positives ------------/");
            Transform button = buttons.transform.Find(button_name + "/DoneBG/DoneText/Text (TMP)");

            button.gameObject.GetComponent<TextMeshProUGUI>().SetText(new_text);
        }
    }
}
