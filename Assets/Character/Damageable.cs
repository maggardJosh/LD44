using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damageable : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var weapon = collision.GetComponentInParent<Weapon>();
        if (weapon == null)
            return;

        GetComponent<TopDownController>().PushBack(weapon.transform.position, weapon.PushBackValue, weapon.StunTime);

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
