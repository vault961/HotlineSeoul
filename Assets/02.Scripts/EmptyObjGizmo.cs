using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmptyObjGizmo : MonoBehaviour {

    public enum GizmoType
    {
        SPAWN_POINT,
        WAY_POINT
    }

    private const string wayPointFile = "Enemy";

    public GizmoType type = GizmoType.SPAWN_POINT;

    public Color _color = Color.red;
    public float _radius = 0.1f;
    
    private void OnDrawGizmos()
    {
        // 기즈모 색상 설정
        Gizmos.color = _color;

        if (type == GizmoType.SPAWN_POINT)
        {
            // Enemy 이미지 파일을 표시
            Gizmos.DrawIcon(transform.position + Vector3.up * 1.0f, wayPointFile, true);
        }
        else
        {
            // 구체 모양의 기즈모 생성, 파라미터는 1.생성위치, 2.반지름
            Gizmos.DrawSphere(transform.position, _radius);
        }
    }
}
