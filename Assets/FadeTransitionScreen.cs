using System;
using UnityEngine;
using UnityEngine.UI;

public class FadeTransitionScreen : MonoBehaviour
{
    private Image image;
    private static FadeTransitionScreen _instance;
    public static FadeTransitionScreen Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<FadeTransitionScreen>();
            return _instance;
        }
    }

    public bool IsTransitioning { get { return currentState != FadeState.FADED_IN; } }
    public float fadeSpeed = .3f;
    private enum FadeState
    {
        CINEMATIC,
        FADED_OUT,
        FADED_IN,
        FADING_OUT,
        FADING_IN
    }
    private void Start()
    {
        image = GetComponent<Image>();
        image.color = Color.black;
    }
    private FadeState currentState = FadeState.FADING_IN;
    private Action LoadAction;

    void Update()
    {
        var oldColor = image.color;
        switch (currentState)
        {
            case FadeState.FADING_IN:
                oldColor.a -= fadeSpeed * Mathf.Min(0.1f, Time.unscaledDeltaTime);
                Time.timeScale = Mathf.Clamp01(.5f - oldColor.a);
                image.color = oldColor;
                if (oldColor.a <= 0)
                {
                    Time.timeScale = 1;
                    oldColor.a = 0;
                    currentState = FadeState.FADED_IN;
                }
                break;
            case FadeState.FADED_OUT:
                break;
            case FadeState.FADED_IN:
                break;
            case FadeState.FADING_OUT:
                oldColor.a += fadeSpeed * Time.unscaledDeltaTime;
                Time.timeScale = Mathf.Clamp01(.5f - oldColor.a);
                image.color = oldColor;
                if (oldColor.a >= 1)
                {
                    Time.timeScale = 0;
                    if (LoadAction != null)
                        LoadAction.Invoke();
                    currentState = FadeState.FADING_IN;
                }
                break;
        }
    }

    public void SetCinematic(bool shouldSet)
    {
        if (shouldSet)
            currentState = FadeState.CINEMATIC;
        else
            currentState = FadeState.FADED_IN;
    }

    public void Transition(Action transAction)
    {
        this.LoadAction = transAction;
        currentState = FadeState.FADING_OUT;
    }

}
