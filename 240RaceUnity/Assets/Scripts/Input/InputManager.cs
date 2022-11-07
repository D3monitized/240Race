using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class InputManager : MonoBehaviour
{
	//Singleton
	[Tooltip("Set on Awake")]
	public static InputManager Instance;

	//Defining Delegates
	public delegate void Throttle(InputAction.CallbackContext context);
	public delegate void Steering(InputAction.CallbackContext context);

	//Delegates that other scripts subscribe to for input callbacks
	public Throttle ThrottleHandler;
	public Steering SteeringHandler;

	private System.Delegate[] m_pausedThrottleSubs; //Cached subscribers for throttle input callback
	private System.Delegate[] m_pausedSteeringSubs; //Cached subscribers for steering input callback

	private void Update() //Debug
	{
		if (Keyboard.current.spaceKey.wasPressedThisFrame)
			PauseCarInput();		
		if (Keyboard.current.enterKey.wasPressedThisFrame)
			ResumeCarInput();		
	}

	public void Throttl(InputAction.CallbackContext context) //Called from input action (Input Action Event)
	{
		if(ThrottleHandler != null) //null check required due to ThrottleHandler resulting in null when "paused"
			ThrottleHandler.Invoke(context);
	}

	public void Steer(InputAction.CallbackContext context) //Called from input action (Input Action Event)
	{
		if(SteeringHandler != null) //null check required due to SteeringHandler resulting in null when "paused"
			SteeringHandler.Invoke(context);
	}

	public void PauseCarInput() //Pause all player input for controlling the car
	{
		m_pausedThrottleSubs = ThrottleHandler.GetInvocationList();
		foreach (System.Delegate del in ThrottleHandler.GetInvocationList())
			ThrottleHandler -= del as Throttle;
		m_pausedSteeringSubs = SteeringHandler.GetInvocationList();
		foreach (System.Delegate del in SteeringHandler.GetInvocationList())
			SteeringHandler -= del as Steering;
	}

	public void ResumeCarInput() //Resume all paused inputs for controlling the car
	{
		foreach (System.Delegate del in m_pausedThrottleSubs)
			ThrottleHandler += del as Throttle;
		foreach (System.Delegate del in m_pausedSteeringSubs)
			SteeringHandler += del as Steering;
	}

	private void Awake()
	{
		Instance = this; //Singleton setup
	}
}
