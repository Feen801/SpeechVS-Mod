using System;
using System.Collections.Concurrent;
using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.Mono;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SpeechVS
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    public class SpeechVS : BaseUnityPlugin
    {
        public static new ManualLogSource Logger;

        private static bool inSession = false;

        private void Awake()
        {
            /* 
             * Create logger object. You can use it to print to the console window with
             * LogInfo.LogInfo("message")
             */
            Logger = base.Logger;
            Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");

            // This makes the "OnSceneLoaded" function call when a scene is loaded.
            SceneManager.sceneLoaded += OnSceneLoaded;
            RunOnMainThread(() => Logger.LogInfo("Test action from Awake"));
        }

        private static ConcurrentQueue<Action> mainThreadActions = new ConcurrentQueue<Action>();

        public void Update()
        {
            while (mainThreadActions.TryDequeue(out var action))
            {
                Logger.LogInfo("Taking action");
                try
                {
                    action();
                    Logger.LogInfo("Took action");
                }
                catch (Exception ex)
                {
                    Logger.LogError("Exception while executing main thread action: " + ex);
                }
            }
        }

        public static void RunOnMainThread(Action action)
        {
            Logger.LogInfo("Enqueueing action on main thread");
            mainThreadActions.Enqueue(action);
        }

        BasicUIManager basicUIManager;
        SpeechListener speechListener;

        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            try
            {
                Logger.LogInfo($"Scene {scene.name} is loaded!");
                if (Equals(scene.name, Constants.SessionStartScene))
                {
                    basicUIManager = new BasicUIManager(scene);
                    Logger.LogInfo("Buttons Loaded");

                    speechListener = new SpeechListener();
                    Logger.LogInfo("Speech Connected");
                    speechListener.OnSpeechRecognized += basicUIManager.Speech;
                    speechListener.StartListening();
                }
                else
                {
                    inSession = false;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Exception in OnSceneLoaded: {ex}");
            }
        }
    }
}
