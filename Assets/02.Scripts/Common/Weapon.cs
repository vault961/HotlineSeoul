using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour {

    GameObject player;
    GameObject weaponHolder;
    PlayerItem pItem;

    public Transform firePos;
    public ParticleSystem muzzleFlash;

    public bool isEquip = false;
    public bool isThrowing = false;

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        weaponHolder = GameObject.FindGameObjectWithTag("WeaponHolder");
        pItem = player.GetComponent<PlayerItem>();
    }
	
	void Update () {

        float f = 0;
        f = Time.time * 1.0f;

        if (!isEquip)
        {
            Vector3 v = transform.position;
            //v.y = Mathf.Sin(Mathf.PI * Time.time) + 1.0f;
            v.z = Mathf.Sin(Mathf.PI * Time.time) + 1.0f;
            //transform.position = v;
        }
    }

    // 무기 던지기 함수
    public void Throw(Vector3 direction)
    {
        transform.rotation = Quaternion.Euler(90.0f, transform.rotation.y, transform.rotation.z);
        Rigidbody rb = GetComponent<Rigidbody>();
        CapsuleCollider collider = GetComponent<CapsuleCollider>();
        rb.centerOfMass = collider.center;
        gameObject.layer = LayerMask.NameToLayer("THROWINGWEAPON"); // 무기를 던지는 순간 THROWINGWEAPON 레이어로 전환
        rb.AddForce(direction * 1500.0f);
        rb.angularVelocity = new Vector3(0.0f, 50.0f, 0.0f);
        collider.isTrigger = false;
        isThrowing = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject == player)
        {
            pItem.AddAbleToPickup(transform);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(other.gameObject == player)
        {
            pItem.RemoveAbleToPickup(transform);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // 벽에 맞았을 경우 튕겨져요
        if (collision.collider.gameObject.CompareTag("Obstacle"))
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            rb.angularVelocity = Vector3.zero;
            rb.velocity = collision.contacts[0].normal * 2f;
            StartCoroutine(DropAfterHitWall(0.4f)); // 코루틴 호출 (0.4f 동안 튕겨나감)

            isThrowing = false;
        }

        //// 적에게 맞았을때
        //else if (collision.collider.gameObject.CompareTag("Enemy"))
        //{
        //    if (isThrowing)
        //    {
        //        EnemyDamage ed = collision.collider.GetComponent<EnemyDamage>();
        //        ed.KnockDown(); // 적을 기절시켜요

        //        isThrowing = false;
        //    }

        //    Rigidbody rb = GetComponent<Rigidbody>();
        //    rb.angularVelocity = Vector3.zero;
        //    rb.velocity = collision.contacts[0].normal * 2f;
        //    StartCoroutine(DropAfterHitWall(0.4f)); // 코루틴 호출 (0.4f 동안 튕겨나감)
        //}

    }

    // 벽에 부딪힌 이후 튕겨나가는 코루틴
    public IEnumerator DropAfterHitWall(float duration)
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        Vector3 startVel = rb.velocity;
        float elapsedTime = 0.0f;
        while(elapsedTime < duration)
        {
            rb.velocity = Vector3.Lerp(startVel, Vector3.zero, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        gameObject.layer = LayerMask.NameToLayer("WEAPON"); // 튕겨나간 이후에 WEAPON 레이어로 전환
        GetComponent<Collider>().isTrigger = true;
        yield break;
    }
}
