using UnityEngine;

public class CameraController : MonoBehaviour
{
	public static CameraController Instance; 

    public Transform Target;

	[SerializeField]
	private float m_moveSpeed = .5f; 

	public void AssignTarget(Transform target)
	{
		Target = target; 
	}

	private void LateUpdate()
	{
		if (Target == null)
			return; 

		transform.position = Vector3.Lerp(transform.position, new Vector3(Target.position.x, Target.position.y, transform.position.z), m_moveSpeed); 
	}

	private void Awake()
	{
		Instance = this;
	}
}
