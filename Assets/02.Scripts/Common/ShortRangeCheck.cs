using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShortRangeCheck : MonoBehaviour {

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
        {
            EnemyDamage ed = other.gameObject.GetComponent<EnemyDamage>();
            //ed.StartCoroutine(ed.KnockDown());
            //ed.KnockDown();
        }
    }
}
