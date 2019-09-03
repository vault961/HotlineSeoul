using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongRangeCheck : MonoBehaviour {

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
        {
            EnemyDamage ed = other.gameObject.GetComponent<EnemyDamage>();
            //ed.KnockDown();
        }
    }
}
