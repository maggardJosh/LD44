using System.Collections;
using UnityEngine;

public class Damageable : MonoBehaviour
{
    public int health = 3;
    TopDownController controller;
    public float invulnerableTime = 2;
    public float stunTime = 2;

    private bool invulnerable = false;
    public float invulnerableFrameSpeed = .1f;
    private void Start()
    {
        controller = GetComponent<TopDownController>();
    }
    int frameCount = 0;
    private void Update()
    {

    }

    private IEnumerator Flash(float time)
    {
        float count = 0;
        int i = 0;
        SpriteRenderer sRend = GetComponent<SpriteRenderer>();
        while (count < time)
        {
            count += Time.deltaTime;
            if (count >= invulnerableFrameSpeed * i)
            {
                if (sRend != null)
                {
                    sRend.enabled = !sRend.enabled;
                }
                i++;
            }
            yield return null;
        }
        sRend.enabled = true;
        invulnerable = false;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (controller.StunTimeLeft > 0 || invulnerable)
            return;
        var weapon = collision.GetComponentInParent<Weapon>();
        if (weapon == null)
            return;

        TakeDamage(weapon.Damage, weapon.transform.position, weapon.PushBackValue);
    }

    public void TakeDamage(int damage, Vector3 position, float pushBackValue)
    {
        if (controller.StunTimeLeft <= 0 && !invulnerable)
        {
            TakeDamage(damage);
            PushBack(position, pushBackValue);
        }
    }

    private void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
            Destroy(gameObject);
        invulnerable = true;
        StartCoroutine(Flash(invulnerableTime));
    }

    private void PushBack(Vector3 position, float PushBackValue)
    {
        controller.PushBack(position, PushBackValue, stunTime);
    }
}
