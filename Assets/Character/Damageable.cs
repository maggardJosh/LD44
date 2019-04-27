using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damageable : MonoBehaviour
{
    public int health = 3;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        var weapon = collision.GetComponentInParent<Weapon>();
        if (weapon == null)
            return;

        GetComponent<TopDownController>().PushBack(weapon.transform.position, weapon.PushBackValue, weapon.StunTime);
        health -= weapon.Damage;
        if (health <= 0)
            Destroy(gameObject);

    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
