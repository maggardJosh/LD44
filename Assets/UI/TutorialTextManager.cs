using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialTextManager : MonoBehaviour
{
    public Text TutorialText;
    
    private static TutorialTextManager _instance;
    public static TutorialTextManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<TutorialTextManager>();
            return _instance;
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        TutorialText.enabled = false;
    }
    
    public void ShowText(string tutorialText)
    {
        TutorialText.text = tutorialText;
        TutorialText.enabled = true;
    }

    public void HideText()
    {
        TutorialText.enabled = false;
    }

}
