using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerCtrl : MonoBehaviour {

    // 캐릭터 이동
    private float h = 0.0f;
    private float v = 0.0f;
    public float moveSpeed = 8.0f;
    public Vector3 movement = Vector3.zero;

    public bool isDead = false;
    bool isMove = false;

    bool isStop = false;
   
    // 캐릭터 애니메이션 초기화
    public Animator ani;

    // 플레이어 이동방향
    float fDir = 0.0f;
    float rDir = 0.0f;

    // 애니메이션 헤시값
    private readonly int hashH = Animator.StringToHash("H");
    private readonly int hashV = Animator.StringToHash("V");
    private readonly int hashMove = Animator.StringToHash("isMove");
    private readonly int hashExe = Animator.StringToHash("Execution");

    // 무기 습득 범위
    public float equipRadius = 5.0f;
    private Rigidbody rb;

    public bool isExecute = false;

    GameObject weaponHolder;

    private AudioSource audioSource;
    public AudioClip killhitSFX;


    void Awake () {
        ani = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();

        weaponHolder = GameObject.FindGameObjectWithTag("WeaponHolder");
    }

    void Update() {

        if(!isExecute && !isDead)
        {
            // 플레이어 이동
            h = Input.GetAxisRaw("Horizontal");
            v = Input.GetAxisRaw("Vertical");
            movement = (Vector3.forward * v) + (Vector3.right * h);
            movement = movement.normalized * moveSpeed * Time.deltaTime;
            transform.position += movement;

            //movement = movement.normalized * moveSpeed;
            //rb.velocity = movement;

            // 플레이어가 마우스를 바라보도록 회전 
            Vector3 mPos = Input.mousePosition;
            mPos.z = Camera.main.transform.position.y;
            Vector3 target = Camera.main.ScreenToWorldPoint(mPos);
            transform.LookAt(new Vector3(target.x, transform.position.y, target.z));

            // 플레이어가 바라보는 방향에 따라 정상적으로 애니메이션 실행
            float pAngle = transform.eulerAngles.y;

            if (movement != Vector3.zero)
            {
                isMove = true;
                if (45.0f < pAngle && pAngle <= 135.0f)
                {
                    fDir = h;
                    rDir = -v;
                }
                else if (135.0f < pAngle && pAngle <= 225.0f)
                {
                    fDir = -v;
                    rDir = -h;
                }
                else if (225.0f < pAngle && pAngle <= 315.0f)
                {
                    fDir = -h;
                    rDir = v;
                }
                else if (315.0f < pAngle || pAngle < 45.0f)
                {
                    fDir = v;
                    rDir = h;
                }
            }
            else
            {
                isMove = false;
                fDir = 0;
                rDir = 0;
            }

            // 애니메이션 
            ani.SetFloat(hashV, fDir);
            ani.SetFloat(hashH, rDir);
            ani.SetBool(hashMove, isMove);
        }

        if(Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log("r");
            if (!isStop)
            {
                GetComponent<Rigidbody>().isKinematic = true;
                Time.timeScale = 0;
                isStop = true;
            }
            else
            {
                GetComponent<Rigidbody>().isKinematic = false;
                transform.position = new Vector3(transform.position.x, 0.1f, transform.position.z);
                Time.timeScale = 1;
                isStop = false;
            }
        }

        if(Input.GetKeyDown(KeyCode.R))
        {
            Restart();
        }
    }

    void Restart()
    {
        SceneManager.LoadScene("Tension");
    }

    public IEnumerator Execution()
    {
        isExecute = true; 
        ani.SetTrigger(hashExe);
        weaponHolder.SetActive(false);

        yield return new WaitForSeconds(0.3f);
        audioSource.PlayOneShot(killhitSFX);

        yield return new WaitForSeconds(0.3f);
        audioSource.PlayOneShot(killhitSFX);

        yield return new WaitForSeconds(0.3f);
        audioSource.PlayOneShot(killhitSFX);

        yield return new WaitForSeconds(0.5f);

        isExecute = false;
        weaponHolder.SetActive(true);
    }
}


