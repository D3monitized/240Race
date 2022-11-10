using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform Target;

	[SerializeField]
	private float m_moveSpeed = .5f; 

	private void LateUpdate()
	{
		transform.position = Vector3.Lerp(transform.position, new Vector3(Target.position.x, Target.position.y, transform.position.z), m_moveSpeed); 
	}
}
