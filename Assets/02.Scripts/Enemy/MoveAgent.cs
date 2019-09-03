using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class MoveAgent : MonoBehaviour {

    public NavMeshAgent agent;

    private readonly float patrolSpeed = 2.0f;
    private readonly float traceSpeed = 6.0f;
    
    private bool _patrolling;
    public bool patrolling
    {
        get { return _patrolling; }
        set
        {
            _patrolling = value;
            if(_patrolling)
            {
                agent.speed = patrolSpeed;
                agent.isStopped = true;
            }
        }
    }

    private Vector3 _traceTarget;
    public Vector3 traceTarget
    {
        get { return _traceTarget; }
        set
        {
            _traceTarget = value;
            agent.speed = traceSpeed;
            TraceTarget(_traceTarget);
        }
    }

    void TraceTarget(Vector3 pos)
    {
        if (agent.isPathStale) return;
        agent.destination = pos;
        agent.isStopped = false;
    }

    public void GetWeapon(Vector3 pos)
    {
        if (agent.isPathStale) return;
        agent.speed = traceSpeed;
        agent.destination = pos;
        agent.isStopped = false;
    }

    public float speed
    {
        get { return agent.velocity.magnitude; }
    }

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = patrolSpeed;
    }

    private void Update()
    {
        // 플레이어로를 추적 중일때
        if (_patrolling)
        {
            // 앞으로 계속 전진
            transform.Translate(Vector3.forward * patrolSpeed * Time.deltaTime);

            // 장애물과 만났을 때
            Ray ray = new Ray(transform.position + Vector3.up, transform.forward);
            RaycastHit rayHit;
            if (Physics.Raycast(ray, out rayHit, 0.5f))
            {
                if (rayHit.collider != null)
                {
                    if (rayHit.collider.CompareTag("Obstacle"))
                    {
                        transform.Rotate(0, -90, 0);
                    }
                }
            }
        }
        else
        {
           // Quaternion rot = Quaternion.LookRotation(agent.desiredVelocity);
           // transform.rotation = Quaternion.Slerp(transform.rotation, rot, 5.0f);
        }
    }

    public void Stop()
    {
        agent.isStopped = true;
        agent.velocity = Vector3.zero;
        _patrolling = false;
    }
}
