using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyItem : MonoBehaviour {

    // 무기 타입
    public enum WeaponType
    {
        FIST = 0,
        KNIFE,
        BLUNT,
        PISTOL,
        RIFLE,
        SHOTGUN,
    }

    public WeaponType currentWeapon = WeaponType.FIST;

    private Animator ani;
    private EnemyAI eAi;
    public GameObject weaponHolder;

    private readonly int hashWeapon = Animator.StringToHash("Weapon");

    private void Start()
    {
        ani = GetComponent<Animator>();
        eAi = GetComponent<EnemyAI>();
    }

    private void Update()
    {
        // 현재 무기에 따른 애니메이션
    }

    private void OnTriggerEnter(Collider other)
    {
        if (currentWeapon == WeaponType.FIST && !eAi.isDown && !eAi.isDead && !eAi.isArmed)
            PickUp(other);
    }

    void PickUp(Collider item)
    {
        item.transform.SetParent(weaponHolder.transform);
        item.enabled = false;
        switch (item.tag)
        {
            case "Knife":
                currentWeapon = WeaponType.KNIFE;
                break;
            case "Blunt":
                currentWeapon = WeaponType.BLUNT;
                item.transform.localPosition = new Vector3(-0.006f, -0.0955f, -0.0296f);
                item.transform.localRotation = Quaternion.Euler(16.067f, 0, 0);
                break;
            case "Rifle":
                currentWeapon = WeaponType.RIFLE;
                item.transform.localPosition = new Vector3(0.1575f, 0.0358f, 0.0818f);
                item.transform.localRotation = Quaternion.Euler(13.308f, 77.22601f, 33.834f);
                break;
        }
        transform.localRotation = Quaternion.identity;
        eAi.isArmed = true;
        eAi.state = EnemyAI.EnemyState.PATROL;
        ani.SetInteger(hashWeapon, (int)currentWeapon);
    }

    public void Drop()
    {
        if(weaponHolder.transform.childCount != 0)
        {
            weaponHolder.GetComponentInChildren<Collider>().enabled = true;
            weaponHolder.transform.DetachChildren();
        }
        currentWeapon = WeaponType.FIST;
        ani.SetInteger(hashWeapon, (int)currentWeapon);
    }
}
