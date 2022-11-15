using UnityEngine;
using UnityEditor; 
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

	public RacetrackSaveFile LoadedTrack;

	public delegate void OnRaceStart(RacetrackTile[] tiles);
	public OnRaceStart OnRaceStartHandler;

	public delegate void OnRaceFinished();
	public OnRaceFinished OnRaceFinishedHandler;

	[HideInInspector]
	public float CountdownSeconds = 3;
	public RacetrackTile[] GetNodes() { return m_nodes; } //Getter for m_nodes -> AICarBrain requires the parts of the track to go around it

	[SerializeField]
	private RacetrackTile[] m_nodes; //All individual parts of the racetrack

	private int m_lapsTilFinish = 4; //Amount of laps to win

	[SerializeField]
	private List<RaceContestant> m_contestants = new List<RaceContestant>(); //All cars that are in the race
	public List<RaceContestant> GetFinishedContestants() { return m_finishedContestants; }
	private List<RaceContestant> m_finishedContestants = new List<RaceContestant>(); //All cars that have finished the race

	[SerializeField]
	private GameObject m_carPrefab; 

	private bool m_raceFinished = true; 

	#region DuringRace

	private void UpdateRacePositions()
	{
		if (m_raceFinished)
			return; 

		for (var i = 0; i < m_contestants.Count; i++)
		{
			for (int j = 0; j < m_contestants.Count; j++)
			{
				if (m_contestants[i] == null)
					break; 

				if (m_contestants[j].CurrentLap > m_contestants[i].CurrentLap) //If one of the contestants is one lap ahead -> move up list
				{
					ResortContestants(i, j);
				}
				else if (m_contestants[j].CurrentLap == m_contestants[i].CurrentLap) //If they're on the same lap -> check who's furthest along the track 
				{
					if (m_contestants[j].CurrentCheckpoint < m_contestants[i].CurrentCheckpoint)
						ResortContestants(i, j);
					else if (m_contestants[j].CurrentCheckpoint == m_contestants[i].CurrentCheckpoint) //If they're on the same part of the track -> check who's closest to the next part of the track
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
			{
				ShowRaceResults();
				m_raceFinished = true; 
			}
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

		ReplayManager.Instance.SaveFrames = false;
	}

	#endregion

	#region BeforeRace

	private bool LoadLevel()
	{
		if(GameManager.Instance)
			LoadedTrack = GameManager.Instance.LoadedTrack; 

		if (LoadedTrack == null)
		{
			Debug.LogError("No Loaded Track Found: Racetrack.cs");
			return false;
		}

		m_nodes = new RacetrackTile[LoadedTrack.Tiles.Count];

		for (int i = 0; i < LoadedTrack.Tiles.Count; i++)
			m_nodes[i] = Instantiate(LoadedTrack.Tiles[i].MyPrefab, LoadedTrack.Tiles[i].MyPosition, LoadedTrack.Tiles[i].MyRotation).GetComponent<RacetrackTile>();

		return true;
	}

	private void SpawnCars()
	{
		if (!GameManager.Instance)
			return;

		string[] guids = AssetDatabase.FindAssets("t:CarConfigBase");
		List<CarConfigBase> configs = new List<CarConfigBase>(); //Find all car config files

		foreach(string guid in guids)		
			configs.Add(AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guid), typeof(CarConfigBase)) as CarConfigBase); //Convert guids to asset paths and load assets from paths
		

		for (int i = 0; i < GameManager.Instance.AmountOfPlayers; i++)
		{
			GameObject go = Instantiate(m_carPrefab); //Instantiate car prefab
			go.GetComponent<CarController>().Config = configs[0]; //"Randomly" assign each car one of the config files
			configs.RemoveAt(0); //Remove the config file that was assigned to not use it for more than one car
			m_contestants.Add(go.GetComponent<RaceContestant>()); 
		}

		//Randomly choose one of the cars, disable it's ai script and enable player input
		int rand = Random.Range(0, m_contestants.Count);
		m_contestants[rand].IsPlayer = true; 
		m_contestants[rand].GetComponent<PlayerCarInput>().enabled = true;
		m_contestants[rand].GetComponent<AICarBrain>().enabled = false;
		m_contestants[rand].GetComponent<AudioSource>().enabled = true; 

		if (CameraController.Instance)
			CameraController.Instance.AssignTarget(m_contestants[rand].transform); 
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

	private void PlaceContestantsAtStartLine()
	{
		//WIP -> Not compatable with multiple cars yet

		Vector2 dir = (m_nodes[1].transform.position - m_nodes[0].transform.position).normalized;

		float totX = m_nodes[0].transform.localScale.x * .8f;
		float currX = totX / m_contestants.Count; 
		

		for (int i = 0; i < m_contestants.Count; i++)
		{
			//Check if goaltile's up vector is pointing in the direction of the next tile, if so -> set the cars rotations to the goaltile's rotation else, set to inverted goaltile's rotation
			if (Mathf.RoundToInt(Vector2.Dot(m_nodes[0].transform.up, dir)) != 1)
				m_contestants[i].transform.rotation = Quaternion.Inverse(m_nodes[0].transform.rotation);
			else
				m_contestants[i].transform.rotation = m_nodes[0].transform.rotation;

			m_contestants[i].transform.position = m_nodes[0].transform.position;
			m_contestants[i].transform.position += m_contestants[0].transform.InverseTransformDirection(-Vector2.right * m_nodes[0].transform.localScale.x / 2 + Vector2.right * currX);
			currX += totX / m_contestants.Count; 
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
				if(OnRaceStartHandler != null)
					OnRaceStartHandler.Invoke(m_nodes);

				rc.GetComponent<CarController>().enabled = true;
			}
		}
	}

	#endregion

	private void OnLevelWasLoaded(int level)
	{
		if (LoadLevel()) //If loaded level successfully
		{
			SpawnCars(); 	
			//GetAllContestants(); //Get All Contestants							
			PlaceContestantsAtStartLine(); //Place cars at finishline/startline			
			ReplayManager.Instance.SaveFrames = true; //Start recording frames for replay
			m_raceFinished = false;
			StartCoroutine(StartRace()); //Start countdown for the race

			foreach (RacetrackTile rt in m_nodes)
			{
				rt.OnCheckpointHandler += OnContestantReachedCheckpoint;
			}
		}
		else //If loading level was unsuccessful -> disable this script (no race :( )
			enabled = false;
	}

	private void Awake()
	{
		Instance = this; 	
	}

	private void Update()
	{
		UpdateRacePositions();
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
