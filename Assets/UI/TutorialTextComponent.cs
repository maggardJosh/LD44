using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTextComponent : MonoBehaviour
{
    [TextArea(2, 4)]
    public string TutorialMessage;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDestroy()
    {
        TutorialTextManager.Instance.HideText();
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            TutorialTextManager.Instance.ShowText(TutorialMessage);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            TutorialTextManager.Instance.HideText();
        }
    }

}
