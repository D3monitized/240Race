using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems; 

public class InputManager_LevelEditor : MonoBehaviour
{
	public static InputManager_LevelEditor Instance; 

	public delegate void OnPlaceTile(InputAction.CallbackContext context);
	public OnPlaceTile OnPlaceTileHandler;

	public delegate void OnRotateTile(InputAction.CallbackContext context);
	public OnRotateTile OnRotateTileHandler; 

	public delegate void OnZoom(InputAction.CallbackContext context);
	public OnZoom OnZoomHandler;

	public delegate void OnMove(InputAction.CallbackContext context);
	public OnMove OnMoveHandler; 

	public void PlaceTile(InputAction.CallbackContext context)
	{
		if (EventSystem.current.IsPointerOverGameObject()) //Prevent clicking through UI elements
			return; 

		if (OnPlaceTileHandler != null)
			OnPlaceTileHandler.Invoke(context);
	}

	public void RotateTile(InputAction.CallbackContext context)
	{
		if (OnRotateTileHandler != null)
			OnRotateTileHandler.Invoke(context); 
	}

	public void Zoom(InputAction.CallbackContext context)
	{
		if (OnZoomHandler != null)
			OnZoomHandler.Invoke(context); 
	}

	public void Move(InputAction.CallbackContext context)
	{
		if (OnMoveHandler != null)
			OnMoveHandler.Invoke(context); 
	}

	private void Awake()
	{
		Instance = this; 
	}
}
