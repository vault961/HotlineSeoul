using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDamage : MonoBehaviour {

    PlayerCtrl pCtrl;
    Animator ani;

    private AudioSource audioSource;
    public AudioClip hitSFX;

    private readonly int hashDead = Animator.StringToHash("Dead");

    private void Start()
    {
        ani = GetComponent<Animator>();
        pCtrl = GetComponent<PlayerCtrl>();
        audioSource = GetComponent<AudioSource>();
    }

    public void Damage()
    {
        audioSource.PlayOneShot(hitSFX);
      //  pCtrl.isDead = true;
      //  ani.SetTrigger(hashDead);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Bullet"))
        {
            Damage();
        }
    }
}
