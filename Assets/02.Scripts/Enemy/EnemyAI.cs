using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour {

    public GameObject player;
    Vector3 playerLastPos;
    Animator ani;

    PlayerCtrl pCtrl;
    EnemyAttack ea;
    EnemyItem eItem;

    public bool isArmed = true;
    public bool isDead = false;
    public bool isDown = false;

    // AI 상태 체크 주기
    private float refreshAICycle = 0.1f;
    // 지연 시간
    private WaitForSeconds waitSeconds;
    // 네비 매쉬 에이전투
    private MoveAgent moveAgent;

    // 적 상태
    public enum EnemyState
    {
        IDLE = 0,
        PATROL,
        TRACE,
        DIE,
        SEARCH,
        UNARMED,
        KNOCKDOWN,
        CHICKEN,
        FIRE,
    }
    public EnemyState state = EnemyState.UNARMED;

    private readonly int hashMove = Animator.StringToHash("isMove");
    private readonly int hashDown = Animator.StringToHash("isDown");

	void Awake() {
        player = GameObject.FindGameObjectWithTag("Player");

        ani = GetComponent<Animator>();
        moveAgent = GetComponent<MoveAgent>();

        waitSeconds = new WaitForSeconds(refreshAICycle);
        pCtrl = player.GetComponent<PlayerCtrl>();

        ea = GetComponent<EnemyAttack>();
        eItem = GetComponent<EnemyItem>();
	}
	
	void Update () {
        if (player.GetComponent<PlayerCtrl>().isDead == true)
            StopAllCoroutines();
    }

    private void OnEnable()
    {
        // checkstate 코루틴
        StartCoroutine(CheckState());

        // action 코루틴
        StartCoroutine(Action());
    }

    IEnumerator CheckState()
    {
        while (!isDead)
        {
            // 뒈짓
            if (state == EnemyState.DIE)
                yield break;

            else if (state == EnemyState.PATROL || state == EnemyState.IDLE || state == EnemyState.SEARCH)
            {
                if (WatchingPlayer())
                    state = EnemyState.TRACE;
            }

            //else if(isArmed && !isDown)
            //{
            //    state = EnemyState.PATROL;
            //}

            else if(state == EnemyState.FIRE)
            {
                float dist = Vector3.Distance(transform.position, playerLastPos);

                if (dist >= 7.0f || !WatchingPlayer())
                    state = EnemyState.TRACE;
            }

            else if(state == EnemyState.KNOCKDOWN)
            {
                if (!isDown)
                    state = EnemyState.PATROL;
            }

            else if(isDown)
            {
                state = EnemyState.KNOCKDOWN;
            }

            // 추격상태일 때
            else if (state == EnemyState.TRACE)
            {
                float dist = Vector3.Distance(transform.position, playerLastPos);
                float dist2 = Vector3.Distance(transform.position, player.transform.position);

                if (dist2 <= 7.0f && WatchingPlayer() && eItem.currentWeapon == EnemyItem.WeaponType.RIFLE)
                    state = EnemyState.FIRE;

                else if (dist < 0.1f && !WatchingPlayer())
                {
                    // 추격한 자리에서 플레이어가 보이지 않는다면 수색상태로 변환
                    state = EnemyState.SEARCH;
                }
            }


            yield return waitSeconds;
        }
    }

    IEnumerator Action()
    {
        while(!isDead)
        {
            yield return waitSeconds;

            switch(state)
            {
                case EnemyState.IDLE:
                    ea.isFire = false;
                    moveAgent.patrolling = false;
                    ani.SetBool(hashMove, false);
                    break;
                case EnemyState.PATROL:
                    ea.isFire = false;
                    moveAgent.patrolling = true;
                    GetComponent<Collider>().enabled = true;
                    ani.SetBool(hashMove, true);
                    ani.SetBool(hashDown, false);
                    break;
                case EnemyState.TRACE:
                    ea.isFire = false;
                    moveAgent.patrolling = false;
                    WatchingPlayer();
                    moveAgent.traceTarget = playerLastPos;
                    ani.SetBool(hashMove, true);
                    break;
                case EnemyState.DIE:
                    moveAgent.Stop();
                    isDead = true;
                    ani.SetBool(hashMove, false);
                    ani.SetBool(hashDown, true);
                    break;
                case EnemyState.SEARCH:
                    ea.isFire = false;
                    float elapsedTime = 0.0f;
                    while (!WatchingPlayer())
                    {
                        elapsedTime += Time.deltaTime;
                        moveAgent.Stop();
                        ani.SetBool(hashMove, false);
                        if (elapsedTime <= 2.0f)
                            transform.rotation = Quaternion.Euler(0, 90, 0);
                        else if (elapsedTime <= 4.0f)
                            transform.rotation = Quaternion.Euler(0, -90, 0);
                        else
                        {
                            transform.rotation = Quaternion.identity;
                            moveAgent.agent.isStopped = true;
                            state = EnemyState.PATROL;
                            break;
                        }
                        yield return null;
                    }
                    break;
                case EnemyState.UNARMED:
                    if(!isDown && !isArmed)
                        SearchWeapon();
                    break;
                case EnemyState.KNOCKDOWN:
                    moveAgent.Stop();
                    ea.isFire = false;
                    isDown = true;
                    GetComponent<Collider>().enabled = false;
                    ani.SetBool(hashMove, false);
                    ani.SetBool(hashDown, true);

                    float knockDownTime = 0.0f;
                    while(knockDownTime <= 3.0f)
                    {
                        if (state == EnemyState.DIE)
                            break;

                        knockDownTime += Time.deltaTime;
                        yield return null;
                    }

                    if(state != EnemyState.DIE)
                    {
                        isDown = false;
                    }
                    break;
                case EnemyState.CHICKEN:
                    ani.SetBool(hashMove, false);
                    Quaternion rot = Quaternion.LookRotation(player.transform.position);
                    transform.rotation = Quaternion.Slerp(transform.rotation, rot, 5.0f);
                    break;
                case EnemyState.FIRE:
                    yield return new WaitForSeconds(0.2f);
                    if (ea.isFire == false)
                        ea.isFire = true;
                    moveAgent.Stop();
                    ani.SetBool(hashMove, false);
                    break;
            }
        }
        yield break;
    }

    bool WatchingPlayer()
    {
        bool playerFound = false;


        float dist = Vector3.Distance(transform.position, player.transform.position);
        Vector3 dir = player.transform.position - transform.position;
        Ray traceRay = new Ray(transform.position + Vector3.up, dir);
        RaycastHit rayHit;
        if (Physics.Raycast(traceRay, out rayHit))
        {
            if (rayHit.collider != null)
            {
                // 플레이 발견시
                if (rayHit.collider.CompareTag("Player") && dist < 9.0f)
                {
                    //yield return new WaitForSeconds(0.3f);
                    playerLastPos = new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z);
                    playerFound = true;
                }
                else
                    playerFound = false;
            }
        }
        return playerFound;
    }
    
    void SearchWeapon()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, 3.0f,
            1 << LayerMask.NameToLayer("WEAPON"));

        if (colliders.Length >= 1)
        {
            ani.SetBool(hashMove, true);
            Vector3 weaponPos = colliders[0].transform.position;
            moveAgent.GetWeapon(weaponPos);
        }
        //else state = EnemyState.CHICKEN;
    }

}
