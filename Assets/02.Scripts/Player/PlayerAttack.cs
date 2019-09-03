using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour {

    // 공격 애니메이션 처리
    private Animator ani;
    private readonly int hashWeapon = Animator.StringToHash("Weapon");
    private readonly int hashSwitch = Animator.StringToHash("Switch");
    private readonly int hashAttack = Animator.StringToHash("Attack");

    // 공격을 휘두를 방향전환 여부
    private bool isSwitch = false;

    // Attack 상태 변수
    private bool isAttacking = false;

    private bool isFire = false;

    // 플레이어 아이템 인스턴스
    private PlayerItem pItem;

    // 근거리, 장거리 범위 콜라이더입니다이잉
    public BoxCollider sRange;
    public BoxCollider lRange;

    // 총알 프리팹
    public GameObject bulletPrefab;

    // 파이어포즈
    public Transform firePos;

    // 화면 진동
    Shaker shaker;

    PlayerCtrl pCtrl;

    private AudioSource audioSource;
    public AudioClip punchSFX;
    public AudioClip swingSFX;


    void Awake () {
        ani = GetComponent<Animator>();
        pItem = GetComponent<PlayerItem>();
        pCtrl = GetComponent<PlayerCtrl>();
        audioSource = GetComponent<AudioSource>();
        shaker = GameObject.Find("Main Camera").GetComponent<Shaker>();
    }
	
	void Update () {
        // 애니메이션
        ani.SetInteger(hashWeapon, (int)pItem.cWeaponType);
        ani.SetBool(hashSwitch, isSwitch);

        // 공격
        if (Input.GetMouseButton(0) && !isAttacking && !pCtrl.isExecute && !pCtrl.isDead)
        {
            switch((int)pItem.cWeaponType)
            {
                case 0:
                    StartCoroutine(Punch());
                    isSwitch = !isSwitch;
                    break;
                case 1:
                    StartCoroutine(Swing());
                    isSwitch = !isSwitch;
                    break;
                case 2:
                    StartCoroutine(Swing());
                    isSwitch = !isSwitch;
                    break;
                case 4:
                    if(!isFire)
                    {
                        isFire = true;
                        StartCoroutine(RifleFire());
                    }
                    break;
            }
            ani.SetTrigger(hashAttack);
        }
        
	}

    // 라이플 빵야빵야
    IEnumerator RifleFire()
    {
        shaker.StartCoroutine(shaker.ShakeCamera());
        Weapon cWeapon = pItem.weaponHolder.GetComponentInChildren<Weapon>();
        Instantiate(bulletPrefab, cWeapon.firePos.position, cWeapon.firePos.rotation);
        yield return new WaitForSeconds(0.1f);
        isFire = false;
    }

    // 냥냥펀치
    IEnumerator Punch()
    {
        isAttacking = true;
        sRange.enabled = true;
        audioSource.PlayOneShot(punchSFX);
        yield return new WaitForSeconds(0.32f);
        
        isAttacking = false;
        sRange.enabled = false;
    }

    // 둔기로 퍽퍽
    IEnumerator Swing()
    {
        isAttacking = true;
        lRange.enabled = true;
        audioSource.PlayOneShot(swingSFX);
        yield return new WaitForSeconds(0.5f);

        isAttacking = false;
        lRange.enabled = false;
    }


}
