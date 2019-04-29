using UnityEngine;

[ExecuteInEditMode]
public class SceneColorInitializer : MonoBehaviour
{
    public Color sceneColor = Color.white;
    public Color playerColor = Color.white;

    [Header("Materials")]
    public Material PlayerMat;
    public Material SceneMat;

    // Start is called before the first frame update
    void Start()
    {
        PlayerMat.color = playerColor;
        SceneMat.color = sceneColor;
    }

    // Update is called once per frame
    void Update()
    {
        if (!Application.isPlaying)
        {
            PlayerMat.color = playerColor;
            SceneMat.color = sceneColor;
        }
    }
}
