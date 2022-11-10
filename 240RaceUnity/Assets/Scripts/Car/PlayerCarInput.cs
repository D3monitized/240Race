using UnityEngine;
using UnityEngine.InputSystem; 

[RequireComponent(typeof(CarController))]
public class PlayerCarInput : MonoBehaviour
{
	/*
		This script is soley used for subscribing to input callbacks 
		and sending it to the CarController script. This script is 
		required for the player in order to control the car.	  
	*/

	private CarController m_controller; 

	private void ThrottleHandle(InputAction.CallbackContext context) => m_controller.m_throttle = context.ReadValue<float>();
	private void SteeringHandle(InputAction.CallbackContext context) => m_controller.m_steerAmount = context.ReadValue<float>();

	private void SubscribeInputMethods()
	{
		InputManager.Instance.ThrottleHandler += ThrottleHandle;
		InputManager.Instance.SteeringHandler += SteeringHandle;
	}

	private void UnsubscribeInputMethods()
	{
		InputManager.Instance.ThrottleHandler -= ThrottleHandle;
		InputManager.Instance.SteeringHandler -= SteeringHandle;
	}

	private void Awake()
	{
		TryGetComponent<CarController>(out m_controller); 
	}

	private void Start() //InputManager.Instance is set on Awake!
	{
		//Subscribe car control methods to input callback
		SubscribeInputMethods();
	}

	private void OnDisable()
	{
		//Unsubscribe car control methods from input callback
		UnsubscribeInputMethods();
		m_controller.m_throttle = 0;
		m_controller.m_steerAmount = 0; 
	}
}
