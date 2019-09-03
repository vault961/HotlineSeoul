using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour {

    EnemyItem eItem;
    GameObject player;
    Animator ani;
    EnemyAI eAi;

    public GameObject bulletPrefab;
    public Transform firePos;

    public bool isFire = false;
    float nextFire = 0.0f;
    float fireRate = 0.1f;

    private readonly int hashAttack = Animator.StringToHash("Attack");

	// Use this for initialization
	void Start () {
        eItem = GetComponent<EnemyItem>();
        ani = GetComponent<Animator>();
        eAi = GetComponent<EnemyAI>();
        player = GameObject.FindGameObjectWithTag("Player");
	}
	
	// Update is called once per frame
	void Update () {
        if (isFire && !eAi.isDead && !eAi.isDown)
        {
            if (Time.time >= nextFire)
            {
                RifleFire();
                nextFire = Time.time + fireRate;
            }
        }

        if (!player.GetComponent<PlayerCtrl>().isDead && !eAi.isDown)
        {
            Ray ray = new Ray(transform.position + Vector3.up, transform.forward);
            RaycastHit rayHit;
            if(Physics.Raycast(ray, out rayHit))
            {
                if(rayHit.collider != null)
                {
                    float dist = Vector3.Distance(transform.position, player.transform.position);

                    if (rayHit.collider.CompareTag("Player") && dist <= 0.4f)
                    {
                        Attack();
                    }
                }
            }
        }
    }

    void Attack()
    {
        ani.SetTrigger(hashAttack);
        player.GetComponent<PlayerDamage>().Damage();
    }

    void RifleFire()
    {
        Weapon eWeapon = eItem.weaponHolder.GetComponentInChildren<Weapon>();

        if (firePos == null)
            firePos = eItem.weaponHolder.GetComponentInChildren<Weapon>().firePos;
        else if (firePos != null)
            Instantiate(bulletPrefab, eWeapon.firePos.position, eWeapon.firePos.rotation);


        Quaternion rotation = Quaternion.LookRotation(player.transform.position - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 10.0f);
    }
}
