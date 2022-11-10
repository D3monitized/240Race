using UnityEngine;
using System.Collections.Generic;
using System.Collections; 

public class Racetrack : MonoBehaviour
{
	/*
		This script holds information about the racetrack and
		the cars in the race. The purpose of it is to update
		information about the race like who's in the lead etc.
	*/

	public static Racetrack Instance; //Singleton

	public delegate void OnRaceFinished();
	public OnRaceFinished OnRaceFinishedHandler; 

	[HideInInspector]
	public float CountdownSeconds = 3; 

	public RacetrackTile[] GetNodes() { return m_nodes; } //Getter for m_nodes -> AICarBrain requires the parts of the track to go around it
	[SerializeField]
	private RacetrackTile[] m_nodes; //All individual parts of the racetrack

	private int m_lapsTilFinish = 1; //Amount of laps to win

	private List<RaceContestant> m_contestants = new List<RaceContestant>(); //All cars that are in the race
	public List<RaceContestant> GetFinishedContestants() { return m_finishedContestants; }
	private List<RaceContestant> m_finishedContestants = new List<RaceContestant>(); //All cars that have finished the race


	private void Update()
	{
		UpdateRacePositions();
	}

	private void UpdateRacePositions()
	{
		for (var i = 0; i < m_contestants.Count; i++)
		{
			for (int j = 0; j < m_contestants.Count; j++)
			{
				if(m_contestants[j].CurrentLap > m_contestants[i].CurrentLap) //If one of the contestants is one lap ahead -> move up list
				{
					ResortContestants(i, j); 
				}
				else if(m_contestants[j].CurrentLap == m_contestants[i].CurrentLap) //If they're on the same lap -> check who's furthest along the track 
				{
					if (m_contestants[j].CurrentCheckpoint < m_contestants[i].CurrentCheckpoint)
						ResortContestants(i, j); 
					else if(m_contestants[j].CurrentCheckpoint == m_contestants[i].CurrentCheckpoint) //If they're on the same part of the track -> check who's closest to the next part of the track
					{
						int current = m_contestants[j].CurrentCheckpoint + 1 < m_nodes.Length ? m_contestants[j].CurrentCheckpoint + 1 : 0;
						float disti = Vector2.Distance(m_contestants[i].transform.position, m_nodes[current].transform.position);
						float distj = Vector2.Distance(m_contestants[j].transform.position, m_nodes[current].transform.position);

						if (distj > disti)
							ResortContestants(i, j); 
					}
				}
			}
		}
	}

	private void ResortContestants(int i, int j) //Switch places with two cars to update race positions
	{
		m_contestants[i].CurrentPosition = j;
		m_contestants[j].CurrentPosition = i; 

		RaceContestant temp = m_contestants[i];
		m_contestants[i] = m_contestants[j];
		m_contestants[j] = temp;
	}

	private void OnContestantReachedCheckpoint(RaceContestant contestant, RacetrackTile tile) //Updates which part of the track the car is on
	{
		if (tile.isFinishLine) //if this part of the track is the finishline -> update which lap the car is on (if not cheating)
		{
			OnContestantReachedFinishLine(contestant); 
			return;
		}

		for (int i = 0; i < m_nodes.Length; i++)
		{
			int current;
			if (m_nodes[i].transform.position == tile.transform.position)
				current = i;
			else
				continue;

			if (contestant.CurrentCheckpoint == current - 1)
				contestant.CurrentCheckpoint = current;

			break;
		}
	}

	private void OnContestantReachedFinishLine(RaceContestant contestant) 
	{
		if (contestant.CurrentCheckpoint != m_nodes.Length - 1) //Check that car has completed an entire lap and isn't cheating
			return;

		if (contestant.CurrentLap == m_lapsTilFinish) //If car has completed all laps -> 
		{
			m_finishedContestants.Add(contestant); 
			print(contestant.name + " Finished!");
			contestant.OnFinished(); 

			if (m_finishedContestants.Count == m_contestants.Count)
				ShowRaceResults(); 
		}
		else 
			contestant.CurrentLap++;

		contestant.CurrentCheckpoint = 0; 
	}

	private void ShowRaceResults()
	{
		if (OnRaceFinishedHandler == null)
			return; 

		OnRaceFinishedHandler.Invoke();

		if (!GameManager.Instance)
			return;

		GameManager.Instance.SaveFrames = false; 
	}

	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
		GetAllContestants();
		StartCoroutine(StartRace()); 
	}

	private void GetAllContestants()
	{
		RaceContestant[] temp; //FindObjectsOfType returns array 
		temp = FindObjectsOfType<RaceContestant>();

		//Add objects from array to list. Needs to be a dynamic list since moving around element positions will be easier
		foreach (RaceContestant rc in temp)
		{
			m_contestants.Add(rc);
			m_contestants[m_contestants.Count - 1].GetComponent<CarController>().enabled = false; //Disable car movement before race starts
		}

		if (m_contestants.Count == 0) //If there are no cars with the contestant script in scene -> disable script as it's useless
			enabled = false;

		if (m_nodes.Length > 0) //If racetrack has 1 ore more tiles -> Subscribe to events
		{
			foreach (RacetrackTile rt in m_nodes)
			{
				rt.OnCheckpointHandler += OnContestantReachedCheckpoint;
			}
		}
	}

	private IEnumerator StartRace() //Race countdown to start
	{
		yield return new WaitForEndOfFrame();
		CountdownSeconds -= Time.deltaTime;

		if (CountdownSeconds > 0)
			StartCoroutine(StartRace());
		else
		{
			foreach (RaceContestant rc in m_contestants)
			{
				rc.GetComponent<CarController>().enabled = true;
			}
		}		
	}

	private void OnDisable()
	{
		if (m_nodes.Length > 0) //If racetrack has 1 ore more tiles -> Unsubscribe to events
		{
			foreach (RacetrackTile rt in m_nodes)
			{
				rt.OnCheckpointHandler -= OnContestantReachedCheckpoint;
			}
		}
	}
}
