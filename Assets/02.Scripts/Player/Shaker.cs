using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shaker : MonoBehaviour {

    public float duration = 0.01f;

    public IEnumerator ShakeCamera()
    {
        // 지나간 시간을 누적해 저장할 변수
        float elapsedTime = 0.0f;

        // 진동시간 동안 루프를 돕니다
        while (elapsedTime < duration)
        {
            // 랜덤한 위치 산출
            Vector3 shakePos = Random.insideUnitCircle;

            // 카메라의 위치를 변경
            transform.position = transform.position + new Vector3(shakePos.x, 0, shakePos.z);
            
            elapsedTime += Time.deltaTime;
            yield return null;
        }

    }
}

