using UnityEngine;

[RequireComponent(typeof(CarController))]
public class AICarBrain : MonoBehaviour
{
	/*
		This is the AI brain for controlling the car. 
		AI is to get a position and try to move to that
		by changing throttle/steering inputs in car controller.
	*/
	
	private RacetrackTile[] m_nodes = new RacetrackTile[0];
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

		if (transform.InverseTransformPoint(m_nodes[m_currentNode].transform.position).x - transform.InverseTransformPoint(transform.position).x < -2) //if target's on the leftside of the car -> turn left
			m_controller.m_steerAmount = -1;
		else if (transform.InverseTransformPoint(m_nodes[m_currentNode].transform.position).x - transform.InverseTransformPoint(transform.position).x > 2)// turn right
			m_controller.m_steerAmount = 1;
		else
			m_controller.m_steerAmount = 0; 

		if(Vector3.Distance(m_nodes[m_currentNode].transform.position, transform.position) < 7)
		{
			if (m_currentNode == m_nodes.Length -1)
				m_currentNode = 0;
			else
				m_currentNode++; 
		}
	}

	private void GetTiles(RacetrackTile[] tiles)
	{
		m_nodes = tiles; 
	}

	private void Awake()
	{
		TryGetComponent<CarController>(out m_controller);		
	}

	private void Start()
	{
		if (Racetrack.Instance)
			Racetrack.Instance.OnRaceStartHandler += GetTiles;
	}
	
	private void OnDisable()
	{
		m_controller.m_steerAmount = 0;
		m_controller.m_throttle = 0; 
	}
}
