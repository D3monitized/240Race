using UnityEngine;
using UnityEngine.InputSystem;

public class CarController : MonoBehaviour
{
	public CarConfigBase Config;

	private float m_throttle;
	private float m_steerAmount;

	private Rigidbody2D m_rb;
	private float m_currentVelocity;

	private void Update()
	{
		Accelerate();
		Steer();
	}

	private void FixedUpdate()
	{
		ApplyRigidbody();
	}

	//Car control method for accelerating/decelerating
	private void Accelerate()
	{
		if (m_throttle == 0) //If no throttleinput -> decelerate to 0 velocity
		{
			//if vel > 0 decelerate else "accelerate"
			m_currentVelocity += m_currentVelocity > 0 ? Config.GetDecelerationAmount() * Time.deltaTime : -Config.GetDecelerationAmount() * Time.deltaTime;
			m_currentVelocity = m_currentVelocity > 0 ? Mathf.Max(m_currentVelocity, 0) : Mathf.Min(m_currentVelocity, 0);
		}
		else //If throttle input -> accelerate to max speed / -max speed (reverse)
		{
			if (m_throttle < 0 && m_currentVelocity > 0 || m_throttle > 0 && m_currentVelocity < 0) //If throttle value is opposite direction of velocity -> use break deceleration
			{
				m_currentVelocity += m_throttle * Config.GetBrakeAmount() * Time.deltaTime;
			}
			else //if throttle value is same direction as velocity -> keep speeding / retain maxspeed
			{
				m_currentVelocity += m_throttle * Config.GetAccelerationAmount() * Time.deltaTime;
			}

			m_currentVelocity = Mathf.Clamp(m_currentVelocity, -Config.GetMaxSpeed(), Config.GetMaxSpeed());
		}

		//print("Velocity: " + m_currentVelocity); 
	}

	//Car control method for steering
	private void Steer()
	{
		//Multiplied by currentVel to prevent car being able to rotate when standing still
		float rotVal = -m_steerAmount * Config.GetSteerSensitivty() * m_currentVelocity * Time.deltaTime;
		transform.Rotate(new Vector3(0, 0, rotVal));
	}

	private void ApplyRigidbody()
	{
		m_rb.velocity = Vector3.Lerp(m_rb.velocity, transform.up * m_currentVelocity, Config.GetTraction() * Time.deltaTime); 
	}


	private void ThrottleHandle(InputAction.CallbackContext context) => m_throttle = context.ReadValue<float>();
	private void SteeringHandle(InputAction.CallbackContext context) => m_steerAmount = context.ReadValue<float>();

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
		TryGetComponent<Rigidbody2D>(out m_rb);
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
	}
}
