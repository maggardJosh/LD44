using UnityEngine;

public class Damageable : MonoBehaviour
{
    public int health = 3;
    TopDownController controller;

    private void Start()
    {
        controller = GetComponent<TopDownController>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (controller.StunTimeLeft > 0)
            return;
        var weapon = collision.GetComponentInParent<Weapon>();
        if (weapon == null)
            return;

        TakeDamage(weapon.Damage);
        PushBack(weapon.transform.position, weapon.PushBackValue, weapon.StunTime);
    }

    public void TakeDamage(int damage)
    {
        if (controller.StunTimeLeft > 0)
            return;
        health -= damage;
        if (health <= 0)
            Destroy(gameObject);
    }

    public void PushBack(Vector3 position, float PushBackValue, float stunTime)
    {
        controller.PushBack(position, PushBackValue, stunTime);

    }
}
