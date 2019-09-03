using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDamage : MonoBehaviour {

    EnemyAI eAi;
    EnemyItem eItem;

    Transform weaponholder;
    GameObject player;
    PlayerCtrl pCtrl;
    Animator ani;

    private AudioSource audioSource;
    public AudioClip hitSFX;
    public AudioClip killhitSFX;

    public GameObject bloodEffect;
    Shaker shaker;


    void Start () {
        eAi = GetComponent<EnemyAI>();
        eItem = GetComponent<EnemyItem>();

        ani = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        player = GameObject.FindGameObjectWithTag("Player");
        pCtrl = player.GetComponent<PlayerCtrl>();

        weaponholder = GameObject.FindGameObjectWithTag("WeaponHolder").transform;
        shaker = GameObject.Find("Main Camera").GetComponent<Shaker>();
    }
	
	void Update () {
        float dist = Vector3.Distance(transform.position, player.transform.position);

		if(Input.GetKeyDown("space") && dist <= 1.0f && eAi.isDown && !eAi.isDead && !pCtrl.isExecute)
        {
            Die(gameObject.GetComponent<Collider>());
            player.transform.position = transform.position;
            player.transform.rotation = Quaternion.Euler(transform.rotation.x, -transform.rotation.y, transform.rotation.z);
            pCtrl.StartCoroutine(pCtrl.Execution());
        }
    }

    // 기절
    public void KnockDown()
    {
        eAi.isDown = true;
        eItem.Drop();
        audioSource.PlayOneShot(hitSFX);
    }

    // 으앙 뒤짐
    public void Die(Collider other)
    {
        eItem.Drop();
        ShowBloodEffect(other);
        eAi.state = EnemyAI.EnemyState.DIE;
        GetComponent<Collider>().enabled = false;
    }

    // 이거슨 트리거!! ^ㅅ^
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Short"))
        {
            // 주먹에 맞은 경우
            KnockDown();
        }

        // 이하 살상무기에 맞았을 때
        else if (other.CompareTag("Long") || other.CompareTag("Bullet"))
        {
            if (other.CompareTag("Bullet"))
            {
                Destroy(other.gameObject);
            }
            else
            {
                audioSource.PlayOneShot(killhitSFX);
                shaker.StartCoroutine(shaker.ShakeCamera());
            }
            Die(other);
        }

        else if(other.tag != "Obstacle" && other.GetComponent<Weapon>().isThrowing == true)
        {
            KnockDown();
            other.GetComponent<Weapon>().StartCoroutine(other.GetComponent<Weapon>().DropAfterHitWall(0.4f)); 
        }

    }

    void ShowBloodEffect(Collider other)
    {
        Vector3 pos = other.transform.position;
        Vector3 normal = other.transform.forward;

        Quaternion rot = Quaternion.FromToRotation(-Vector3.forward, normal);

        GameObject blood = Instantiate(bloodEffect, pos, rot);
    }
}
