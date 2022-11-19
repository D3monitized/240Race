using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class CarController : MonoBehaviour
{
	/*
	 This is script controls the car through
	 the RigidBody2D component that is required to 
	 sit on the car. 
	 
	 The car is controlled through 
	 two float inputs that are m_throttle and m_steerAmount
	 which determines acceleration and steering.
	*/

	public CarConfigBase Config;

	[HideInInspector]
	public float m_throttle; //Need to be accessible by PlayerCarInput / AICarBrain
	[HideInInspector]
	public float m_steerAmount; //Need to be accessible by PlayerCarInput / AICarBrain

	private Rigidbody2D m_rb;
	private float m_currentVelocity;

	private float m_speedometer; 

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
		m_rb.angularVelocity = rotVal; 
	}

	private Vector3 previousPos; 	

	private void ApplyRigidbody()
	{
		m_rb.velocity = Vector3.Lerp(m_rb.velocity, transform.up * m_currentVelocity, Config.GetTraction() * Time.deltaTime); 
	}

	private void Awake()
	{
		TryGetComponent<Rigidbody2D>(out m_rb);
		previousPos = transform.position; 
	}	
}
