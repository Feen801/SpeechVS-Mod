using System;
using SpeechVS;
using UnityEngine.SceneManagement;

namespace SpeechVS;

public abstract class UIManager
{
    public UIManager(Scene scene)
    {
        if (!scene.isLoaded || !Equals(scene.name, Constants.SessionStartScene))
        {
            throw new ArgumentException("Session scene is incorrect or not yet loaded");
        }
    }

    public virtual bool Interact()
    {
        SpeechVS.Logger.LogWarning("This UIManager Interact should not be called!");
        return false;
    }
}