using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections; 
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

	public delegate void OnReplayStarted();
	public OnReplayStarted OnReplayStartedHandler;
	public delegate void OnReplayFinished();
	public OnReplayFinished OnReplayFinishedHandler; 

	public bool SaveFrames = true; 

	private List<RaceContestant> m_contestants = new List<RaceContestant>(); 
	private List<List<Vector2>> m_contestantPositions = new List<List<Vector2>>();
	private List<List<Vector3>> m_contestantRotations = new List<List<Vector3>>(); 

	public void RestartLevel()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); 
	}

	public void StartReplay()
	{
		if(OnReplayStartedHandler != null)
			OnReplayStartedHandler.Invoke();

		if (Racetrack.Instance)
			Racetrack.Instance.enabled = false; 

		for (int i = 0; i < m_contestants.Count; i++)
		{
			m_contestants[i].transform.position = m_contestantPositions[i][0]; //Place all cars at their first saved positions
			m_contestants[i].transform.eulerAngles = m_contestantRotations[i][0];			
		}

		StartCoroutine(Replay()); 
	}

	private void Update()
	{
		if(SaveFrames)
			SaveFramesForReplay(); 
	}

	private void SaveFramesForReplay()
	{
		if (!Racetrack.Instance)
			return;

		for (int i = 0; i < m_contestants.Count; i++)
		{
			m_contestantPositions[i].Add(m_contestants[i].transform.position);
			m_contestantRotations[i].Add(m_contestants[i].transform.eulerAngles); 
		}
	}

	private int iteration = 0; 
	private IEnumerator Replay()
	{
		yield return new WaitForEndOfFrame();
		for (int i = 0; i < m_contestants.Count; i++)
		{
			m_contestants[i].transform.position = m_contestantPositions[i][iteration];
			m_contestants[i].transform.eulerAngles = m_contestantRotations[i][iteration]; 
		}

		if(iteration < m_contestantPositions[0].Count - 1)
		{
			StartCoroutine(Replay()); 
			iteration++;
		}
		else
		{
			if (OnReplayFinishedHandler != null)
				OnReplayFinishedHandler.Invoke();
		}
	}

	private void Awake()
	{
		Instance = this; 
	}

	private void Start()
	{
		GetContestants(); 
	}

	private void GetContestants()
	{
		RaceContestant[] temp = FindObjectsOfType<RaceContestant>();

		if (temp.Length == 0)
			return;

		foreach (RaceContestant rc in temp)
		{
			m_contestants.Add(rc);
			m_contestantPositions.Add(new List<Vector2>());
			m_contestantRotations.Add(new List<Vector3>());
		}
	}
}
