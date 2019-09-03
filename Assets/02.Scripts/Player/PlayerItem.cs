using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerItem : MonoBehaviour, IComparer<Transform> {

    private AudioSource audioSource;

    public AudioClip pickUpSFX;
    public AudioClip throwSFX;

    // 플레이어 무기 타입
    public enum WeaponType
    {
        FIST = 0,
        KNIFE,
        BLUNT,
        PISTOL,
        RIFLE,
        SHOTGUN,
    }

    // 초기 무기 설정 = 주먹
    public WeaponType cWeaponType = WeaponType.FIST;
    
    public GameObject weaponHolder;
    private Transform equipedWeapon;
    PlayerCtrl pCtrl;

    List<Transform> weapons = new List<Transform>();

    void Start()
    {
        pCtrl = GetComponent<PlayerCtrl>();
        audioSource = GetComponent<AudioSource>();
    }

	void Update ()
    {
        // 마우스 오른쪽 버튼
        if(Input.GetMouseButtonUp(1) && !pCtrl.isExecute)
        {
            Transform weaponTransform = CheckNearestWeaponInPickupRange();

            // 무기를 장착 중이라면 버리기
            if (equipedWeapon != null)
                Drop();

            // 근처에 무기가 있다면 줍기
            if (weaponTransform != null)
            {
                Pickup(weaponTransform);
            }
        }
	}

    // 범위 안에 있는 무기들 중 가장 가까운 무기를 반환 (소팅 함수 사용)
    Transform CheckNearestWeaponInPickupRange()
    {
        if (weapons.Count > 0)
        {
            SortByDistance(weapons);
            return weapons[0];
        }

        return null;
    }

    // 리스트 안에서 소팅 함
    public void SortByDistance(List<Transform> list)
    {
        list.Sort(this);
    }

    // 리스트에 주울 수 있는 무기 추가
    public void AddAbleToPickup(Transform weapon)
    {
        if(weapons.Contains(weapon) == false)
        {
            weapons.Add(weapon);
        }
    }

    // 리스트에 주울 수 없는 무기 추가
    public void RemoveAbleToPickup(Transform weapon)
    {
        if (weapons.Contains(weapon) == true)
        {
            weapons.Remove(weapon);
        }
    }

    // 무기 줍기
    void Pickup(Transform item)
    {
        SetEquip(item, true);   // 콜라이더 false, 키네마틱 true
        item.SetParent(weaponHolder.transform);
        RemoveAbleToPickup(item);
        equipedWeapon = item;

        audioSource.PlayOneShot(pickUpSFX);

        switch(item.tag)
        {
            case "Knife":
                cWeaponType = WeaponType.KNIFE;
                break;
            case "Blunt":
                cWeaponType = WeaponType.BLUNT;
                item.transform.localPosition = new Vector3(-0.006f, -0.0955f, -0.0296f);
                item.transform.localRotation = Quaternion.Euler(16.067f, 0, 0);
                break;
            case "Rifle":
                cWeaponType = WeaponType.RIFLE;
                item.transform.localPosition = new Vector3(0.1575f, 0.0358f, 0.0818f);
                item.transform.localRotation = Quaternion.Euler(13.308f, 77.22601f, 33.834f);
                break;
        }
    }

    // 무기 버리기
    void Drop()
    {
        if (equipedWeapon == null)
            return;

        audioSource.PlayOneShot(throwSFX);

        SetEquip(equipedWeapon.transform, false);   // 콜라이더 true, 키네마틱 false
        weaponHolder.transform.DetachChildren();
        equipedWeapon.GetComponent<Weapon>().Throw(transform.forward); // 쓰로우(무기 던지기) 함수 호출
        equipedWeapon = null;
        cWeaponType = WeaponType.FIST;
    }

    // 장착 상태 설정
    void SetEquip(Transform item, bool isEquip)
    {
        Collider collider = item.GetComponent<Collider>();
        Rigidbody rb = item.GetComponent<Rigidbody>();
        collider.enabled = !isEquip;            
        rb.isKinematic = isEquip;                  
    }

    // 소팅에 사용되는 컴페어 함수
    int IComparer<Transform>.Compare(Transform x, Transform y)
    {
        if (x == null)
        {
            if (y == null)
            {
                // If x is null and y is null, they're
                // equal. 
                return 0;
            }
            else
            {
                // If x is null and y is not null, y
                // is greater. 
                return -1;
            }
        }
        else
        {
            if (y == null)
            {
                return 1;
            }
            else
            {
                float d1 = Vector3.Distance(transform.position, x.position);
                float d2 = Vector3.Distance(transform.position, y.position);

                return d1.CompareTo(d2);
            }
        }
    }
}
