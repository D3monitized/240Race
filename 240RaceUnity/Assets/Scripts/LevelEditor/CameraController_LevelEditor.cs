using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController_LevelEditor : MonoBehaviour
{
	//[SerializeField]
	private float m_moveSpeed = 0;
	//[SerializeField]
	private float m_zoomSpeed = 0;
	private Vector2 m_moveDir;
	private float m_zoomVal;

	private const float m_maxZoomOut = 67;
	private const float m_maxZoomIn = 1;

	private void Update()
	{
		Zoom();
	}

	private void LateUpdate()
	{
		Move();
	}

	private void Zoom()
	{
		float target = GetComponent<Camera>().orthographicSize + m_zoomVal;
		GetComponent<Camera>().orthographicSize = Mathf.Clamp(Mathf.Lerp(GetComponent<Camera>().orthographicSize, target, m_zoomSpeed * Time.deltaTime), m_maxZoomIn, m_maxZoomOut);
	}

	private void Move()
	{
		Vector2 target = (Vector2)transform.position + m_moveDir;
		transform.position = Vector3.Lerp(transform.position, new Vector3(target.x, target.y, transform.position.z), m_moveSpeed * Time.deltaTime);
	}

	private void OnZoomHandle(InputAction.CallbackContext context) => m_zoomVal = context.ReadValue<float>();
	private void OnMoveHandle(InputAction.CallbackContext context) => m_moveDir = context.ReadValue<Vector2>();

	private void Start()
	{
		GetComponent<Camera>().orthographicSize = 67; //"Zoom out" camera to good start zoom for level editing. Hardcoded from testing in editor.

		if (!InputManager_LevelEditor.Instance)
			return;

		//Set camera to good start position for level editing. Hardcoded from testing in editor.
		transform.position = new Vector3(
			(LevelEditor.Instance.GetGridSize().x * Mathf.RoundToInt(LevelEditor.Instance.GetNodeSize())) / 2,
			(LevelEditor.Instance.GetGridSize().y * Mathf.RoundToInt(LevelEditor.Instance.GetNodeSize())) / 2,
			transform.position.z
		);

		InputManager_LevelEditor.Instance.OnZoomHandler += OnZoomHandle;
		InputManager_LevelEditor.Instance.OnMoveHandler += OnMoveHandle;
	}

	private void OnDisable()
	{
		if (!InputManager_LevelEditor.Instance)
			return;

		InputManager_LevelEditor.Instance.OnZoomHandler -= OnZoomHandle;
		InputManager_LevelEditor.Instance.OnMoveHandler -= OnMoveHandle;
	}
}
