using UnityEngine;

[RequireComponent(typeof(CarController))]
public class AICarBrain : MonoBehaviour
{
	/*
		This is the AI brain for controlling the car. 
		AI is to get a position and try to move to that
		by changing throttle/steering inputs in car controller.
	*/
	
	private RacetrackTile[] m_nodes;
	public int m_currentNode; 

    private CarController m_controller;

	private void Update()
	{
		MoveToTarget(); 
	}

	private void MoveToTarget()
	{
		if (m_nodes.Length == 0)
			return; 

		m_controller.m_throttle = 1;

		if (transform.InverseTransformPoint(m_nodes[m_currentNode].transform.position).x < transform.InverseTransformPoint(transform.position).x) //if target's on the leftside of the car -> turn left
			m_controller.m_steerAmount = -1;
		else // turn right
			m_controller.m_steerAmount = 1; 

		if(Vector3.Distance(m_nodes[m_currentNode].transform.position, transform.position) < 7)
		{
			if (m_currentNode == m_nodes.Length -1)
				m_currentNode = 0;
			else
				m_currentNode++; 
		}
	}

	private void Awake()
	{
		TryGetComponent<CarController>(out m_controller); 
	}

	private void Start()
	{
		if (Racetrack.Instance)
			m_nodes = Racetrack.Instance.GetNodes(); 
	}

	private void OnDisable()
	{
		m_controller.m_steerAmount = 0;
		m_controller.m_throttle = 0; 
	}
}
