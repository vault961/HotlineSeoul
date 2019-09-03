using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCtrl : MonoBehaviour {

    // 총알 속도
    public float speed = 1000.0f;

    Rigidbody rb;
    Transform tr;
    TrailRenderer trail;
    private AudioSource audioSource;
    public AudioClip fireSFX;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        tr = GetComponent<Transform>();
        trail = GetComponent<TrailRenderer>();
        audioSource = GetComponent<AudioSource>();

    }

    private void OnEnable()
    {
        rb.AddForce(transform.forward * speed);
        audioSource.PlayOneShot(fireSFX);
    }

    private void OnDisable()
    {
        trail.Clear();
    }

    private void OnTriggerEnter(Collider other)
    {
        Destroy(gameObject);
    }
}
