using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof (CarController))]
public class CarVisuals : MonoBehaviour
{
	private CarController m_controller; 
    private SpriteRenderer m_renderer;

	private int m_turnAmount;

	private void SteerHandle(InputAction.CallbackContext context)
	{ 
		m_turnAmount = Mathf.RoundToInt(context.ReadValue<float>());
		//if turnamount = 0 (car's not turning) -> set sprite to default
		m_renderer.sprite = m_turnAmount != 0 ? m_turnAmount > 0 ? m_controller.Config.GetSprites()[2] : m_controller.Config.GetSprites()[1] : m_controller.Config.GetSprites()[0];
	}

	private void Awake()
	{
		TryGetComponent<CarController>(out m_controller);
		if (GetComponentInChildren<SpriteRenderer>() && m_controller) //If sprite renderer was found -> create reference and apply default sprite
		{
			m_renderer = GetComponentInChildren<SpriteRenderer>();
			m_renderer.sprite = m_controller.Config.GetSprites()[0]; //Default sprite
		}

	}

	private void Start()
	{
		if (m_renderer && m_controller)
			InputManager.Instance.SteeringHandler += SteerHandle; 
	}

	private void OnDisable()
	{
		InputManager.Instance.SteeringHandler -= SteerHandle; 
	}
}
