using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCtrl : MonoBehaviour {

    public Transform target;
    public float dampingSpeed = 8.0f;
    public float height = 7.0f;

    private float dampingX;
    private float dampingY;

    public PlayerCtrl pCtrl;

    void Start()
    {
        transform.rotation = Quaternion.Euler(90.0f, 0, 0);
        dampingSpeed = pCtrl.moveSpeed;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            dampingX = target.transform.forward.x * Input.mousePosition.x;
            dampingY = target.transform.forward.z * Input.mousePosition.y;
        }
        else
        {
            dampingX = target.transform.forward.x;
            dampingY = target.transform.forward.z;
        }

        Vector3 TargetPos = new Vector3(target.transform.position.x + dampingX, height, target.transform.position.z + dampingY);
        transform.position = Vector3.Slerp(transform.position, TargetPos, Time.deltaTime * dampingSpeed);

        if (pCtrl.movement != null)
        {
            CameraRoll();
        }
           
    }

    void CameraRoll()
    {
        if(!pCtrl.isExecute && !pCtrl.isDead)
        {
            float rolling = Input.GetAxisRaw("Horizontal") * Time.deltaTime * 3.0f;
            transform.Rotate(0, 0, rolling);

            Vector3 euler = transform.rotation.eulerAngles;
            if (euler.y >= 180.0f) euler.y -= 360.0f;
            euler.y = Mathf.Clamp(euler.y, -4.0f, 4.0f);
            if (euler.y < 0) euler.y += 360.0f;
            Quaternion rot = transform.rotation;
            rot.eulerAngles = euler;
            transform.rotation = rot;
        }
    }
}