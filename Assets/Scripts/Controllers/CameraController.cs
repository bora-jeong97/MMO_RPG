using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    Define.CameraMode _mode = Define.CameraMode.QuarterView;

    [SerializeField]
    Vector3 _delta = new Vector3(0.0f, 6.0f, -5.0f);

    [SerializeField]
    GameObject _player = null;

    // 외부에서 카메라에 플레이어 적용
    public void SetPlayer(GameObject player) { _player = player; } 

    void Start()
    {
        
    }

    void LateUpdate()
    { 
        if (_mode == Define.CameraMode.QuarterView)
        {
            if(_player.IsValid() == false)   // Extension으로 체크
            {
                return; // "null" 메모리에서 유니티가 여전히 들고 있는데 null로 인식하도록 하는것. operator 
                // 어찌됐든 == null로 접근 가능함. C#에서 "null"은 실제로 문자열이 아니라, 특별한 값으로 사용됩니다. "null"은 참조 타입 변수가 아무런 객체를 가리키지 않을 때 사용되는 예약된 키워드입니다.
            }

            RaycastHit hit;
            // Block뒤로 가면 카메라가 전환됨.
            if (Physics.Raycast(_player.transform.position, _delta, out hit, _delta.magnitude, 1 << (int)Define.Layer.Block)) // 1 << (int)Define.Layer.Block == LayerMask.GetMask("Block")
            {
                float dist = (hit.point - _player.transform.position).magnitude * 0.8f;
                transform.position = _player.transform.position + _delta.normalized * dist;
            }
            else
            {
				transform.position = _player.transform.position + _delta;
				transform.LookAt(_player.transform);
			}
		}
    }

    public void SetQuarterView(Vector3 delta)
    {
        _mode = Define.CameraMode.QuarterView;
        _delta = delta;
    }
}
