using UnityEngine;
using UnityEngine.UI;

public class HealthGUI : MonoBehaviour
{
    public GameObject HealthContainer;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        int health = FindObjectOfType<PlayerController>().GetComponent<Damageable>().health;
        var hearts = HealthContainer.GetComponentsInChildren<Image>();
        float healthRatio = health / 2f;
        for (int i = 0; i < hearts.Length; i++)
        {
            hearts[i].fillAmount = Mathf.Min(1, healthRatio);
            healthRatio--;
        }
    }

}
