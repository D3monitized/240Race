using UnityEngine;

[RequireComponent(typeof (CarController))]
public class CarVisuals : MonoBehaviour
{
	private CarController m_controller; 
    private SpriteRenderer m_renderer;

	private int m_turnAmount;

	private void Update()
	{
		UpdateVisuals(); 
	}

	private void UpdateVisuals()
	{ 
		m_turnAmount = Mathf.RoundToInt(m_controller.m_steerAmount);
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
}
