using UnityEngine;
using System.Collections;
using System.Collections.Generic; 

public class ReplayManager : MonoBehaviour
{
	public static ReplayManager Instance; 

	public delegate void OnReplayStarted();
	public OnReplayStarted OnReplayStartedHandler;
	public delegate void OnReplayFinished();
	public OnReplayFinished OnReplayFinishedHandler;

	[HideInInspector]
	public bool SaveFrames = false;

	private List<RaceContestant> m_contestants = new List<RaceContestant>();
	private List<List<Vector2>> m_contestantPositions = new List<List<Vector2>>();
	private List<List<Vector3>> m_contestantRotations = new List<List<Vector3>>();

	private int m_replayIteration = 0;

	public void StartReplay()
	{
		if (OnReplayStartedHandler != null)
			OnReplayStartedHandler.Invoke();

		m_replayIteration = 0; 

		if (Racetrack.Instance)
			Racetrack.Instance.enabled = false;

		for (int i = 0; i < m_contestants.Count; i++)
		{
			m_contestants[i].transform.position = m_contestantPositions[i][0]; //Place all cars at their first saved positions
			m_contestants[i].transform.eulerAngles = m_contestantRotations[i][0];
		}

		StartCoroutine(Replay());
	}

	private void SaveFramesForReplay()
	{
		if (!Racetrack.Instance)
			return;

		for (int i = 0; i < m_contestants.Count; i++)
		{
			if (m_contestants[i] == null)
				break; 

			m_contestantPositions[i].Add(m_contestants[i].transform.position);
			m_contestantRotations[i].Add(m_contestants[i].transform.eulerAngles);
		}
	}

	
	private IEnumerator Replay()
	{
		yield return new WaitForEndOfFrame();
		for (int i = 0; i < m_contestants.Count; i++)
		{
			m_contestants[i].transform.position = m_contestantPositions[i][m_replayIteration];
			m_contestants[i].transform.eulerAngles = m_contestantRotations[i][m_replayIteration];
		}

		if (m_replayIteration < m_contestantPositions[0].Count - 1)
		{
			StartCoroutine(Replay());
			m_replayIteration++;
		}
		else
		{
			if (OnReplayFinishedHandler != null)		
				OnReplayFinishedHandler.Invoke();
		}
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

	private void OnLevelWasLoaded(int level)
	{
		GetContestants();
	}

	private void Awake()
	{
		Instance = this; 
	}

	private void Update()
	{
		if (SaveFrames)
			SaveFramesForReplay();
	}	
}
