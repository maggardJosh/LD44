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
        if (!enabled)
            return;
        if (controller.StunTimeLeft <= 0 && !invulnerable)
        {
            TakeDamage(damage);
            PushBack(position, pushBackValue);
        }
    }

    private void TakeDamage(int damage)
    {
        health -= damage;
        //TODO: Decide if this is how we wanna handle death sounds
        PlayDamageSfx(gameObject.tag, health);
        if (health <= 0)
        {
            Destroy(gameObject);
        }
        invulnerable = true;
        StartCoroutine(Flash(invulnerableTime));
    }

    private void PushBack(Vector3 position, float PushBackValue)
    {
        controller.PushBack(position, PushBackValue, stunTime);
    }

    private void PlayDamageSfx(string gameObjectTag, int health)
    {
        switch (gameObjectTag.ToUpper())
        {
            case "PLAYER":
                if (health > 0)
                    PlayDamageSound(SoundManager.Sound.SFX_Player_Damage);
                else
                    PlayDamageSound(SoundManager.Sound.SFX_Player_Die);
                break;
            default:
                if (health > 0)
                    PlayDamageSound(SoundManager.Sound.SFX_Enemy_EnergyDamage);
                else
                    PlayDamageSound(SoundManager.Sound.SFX_Enemy_EnergyDie);
                break;
        }
    }

    private void PlayDamageSound(SoundManager.Sound soundToPlay)
    {
        SoundManager.Instance.PlaySound(soundToPlay);
    }
}
