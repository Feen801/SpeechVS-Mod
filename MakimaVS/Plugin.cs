using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.Mono;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

namespace MakimaVS
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    public class MakimaVS : BaseUnityPlugin
    {
        public static new ManualLogSource Logger;
        private static GameObject hair;
        private static Texture2D eye_texture;

        private void Awake()
        {
            // Plugin startup logic
            Logger = base.Logger;
            Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");

            MakimaAssets.LoadAssets();
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (Equals(scene.name, Constants.SessionScene))
            {
                hair = GameObject.Instantiate(MakimaAssets.hair);
                GameObject base_hair = GameObjectHelper.GetGameObjectCheckFound("HairPosition/Hair1");
                GameObject other_hair = GameObjectHelper.GetGameObjectCheckFound("HairPosition/AltHairs");
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
                GameObject hair_pos = GameObjectHelper.GetGameObjectCheckFound("HairPosition");
                hair.transform.SetParent(hair_pos.transform, false);
                hair.transform.SetLocalPositionAndRotation(new Vector3(-0.002f, -0.025f, 0.031f), Quaternion.identity);
                eye_texture = MakimaAssets.eyes;
                GameObject face = GameObjectHelper.GetGameObjectCheckFound("GirlCharacter/Face");
                face.GetComponent<SkinnedMeshRenderer>().materials[5].SetTexture("_MainTex", eye_texture);
                WoofifyButton("Yes");
                WoofifyButton("Ok");
                WoofifyButton("Ok2");
                WoofifyButton("Obey");
                PlayMakerFSM thirsty_eyes = face.GetComponents<PlayMakerFSM>()[5];
                foreach (HutongGames.PlayMaker.FsmState state in thirsty_eyes.FsmStates)
                {
                    state.Actions = [];
                }
            }
        }

        private void WoofifyButton(string button_name)
        {
            GameObject buttons = GameObjectHelper.GetGameObjectCheckFound("Positives ------------/");
            Transform button = buttons.transform.Find(button_name + "/DoneBG/DoneText/Text (TMP)");

            button.gameObject.GetComponent<TextMeshProUGUI>().SetText("Woof!");
        }
    }
}
