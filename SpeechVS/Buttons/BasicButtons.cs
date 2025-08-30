using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SpeechVS;
public class BasicUIManager : UIManager
{
    private List<VSChoiceButton> vsChoiceButtons = [];

    public BasicUIManager(Scene scene) : base(scene)
    {
        //Reference each postive (left) and negative (right) button by looping through the childern of each parent
        GameObject positiveButtonParent = GameObject.Find("GeneralCanvas/EventManager/Buttons/Positives ------------");
        GameObject negativeButtonParent = GameObject.Find("GeneralCanvas/EventManager/Buttons/Negatives ------------");

        foreach (Transform positiveButton in positiveButtonParent.transform)
        {
            if (Equals(positiveButton.name, "PoTMercy"))
            {
                //?????? what is this button succudev?
                continue;
            }
            VSChoiceButton positiveChoiceButton = new(positiveButtonParent.transform, positiveButton.name, positiveButton.name, VSChoiceButton.ButtonType.Positive);
            SpeechVS.Logger.LogInfo("Found pos choice button: " + positiveChoiceButton.name);
            vsChoiceButtons.Add(positiveChoiceButton);
            positiveChoiceButton.SetTriggerIconLocation(0, 100);
        }

        foreach (Transform negativeButton in negativeButtonParent.transform)
        {
            VSChoiceButton negativeChoiceButton = new(negativeButtonParent.transform, negativeButton.name, negativeButton.name, VSChoiceButton.ButtonType.Negative);
            SpeechVS.Logger.LogInfo("Found neg choice button: " + negativeChoiceButton.name);
            vsChoiceButtons.Add(negativeChoiceButton);
            negativeChoiceButton.SetTriggerIconLocation(0, 100);
        }

        SpeechVS.Logger.LogInfo("Finished setting up basic buttons");

        GameObject centerGameObject = GameObject.Find("NewButtons/Center");
        if (centerGameObject == null)
        {
            SpeechVS.Logger.LogError("centerGameObject not found (basicUI).");
        }
    }

    public void Speech(string speech)
    {
        if (string.IsNullOrEmpty(speech))
        {
            SpeechVS.Logger.LogWarning("Received null or empty speech string.");
            return;
        }

        string speech_cleaned = CleanString(speech);
        foreach (VSChoiceButton button in vsChoiceButtons)
        {
            if (string.IsNullOrEmpty(button.triggerWord))
            {
                continue;
            }
            string button_cleaned = CleanString(button.triggerWord);
            if (button.components.buttonObject.activeSelf && speech_cleaned.Contains(button_cleaned))
            {
                SpeechVS.Logger.LogInfo("Trying to click: " + button.name);
                button.Click();
            }
        }
    }

    private string CleanString(string s)
    {
        return System.Text.RegularExpressions.Regex.Replace(s, @"[^a-zA-Z\s]", "").ToLower().Trim();
    }
}