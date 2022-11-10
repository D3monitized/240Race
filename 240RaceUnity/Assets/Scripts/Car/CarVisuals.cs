using UnityEngine;

[RequireComponent(typeof (CarController))]
public class CarVisuals : MonoBehaviour
{
	private CarController m_controller; 
    private SpriteRenderer m_renderer;

	private void Awake()
	{
		m_controller = GetComponent<CarController>();

		if (GetComponentInChildren<SpriteRenderer>())
			m_renderer = GetComponentInChildren<SpriteRenderer>();
	}

	private void Start()
	{
		if(m_renderer)
			m_renderer.sprite = m_controller.Config.GetSprite();
	}
}
