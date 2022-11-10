using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
	/*
		This script updates all HUD/UI elements on screen
	*/

	[SerializeField]
	private Text m_racePosText;
	[SerializeField]
	private Text m_countdownText;
	[SerializeField]
	private GameObject m_resultsPanel; 

	private RaceContestant m_player; //Player ref
	private RaceContestant[] allContestants; //All cars in race

	private Animator m_animator; //Animator for UI animations

	private void Update()
	{
		UpdateRacePosUI();
		UpdateCountdownSeconds(); 
	}

	private void UpdateRacePosUI()
	{
		if (m_player == null)
			return;

		m_racePosText.text = (m_player.CurrentPosition + 1).ToString() + "/" + allContestants.Length;
	}

	private int m_countdownPreviousFrame; //required for checking whether or not a second has passed
	private void UpdateCountdownSeconds()
	{
		if (!Racetrack.Instance || Racetrack.Instance.CountdownSeconds <= 0) //Check that there's an instance of racetrack and that countdownseconds > 0 (meaning that countdown is still active)
		{
			m_countdownText.enabled = false; 
			return;
		}

		if (m_countdownPreviousFrame != Racetrack.Instance.CountdownSeconds && m_animator) //Animation that should trigger each second that goes down
			m_animator.SetTrigger("Countdown"); 

		m_countdownText.text = Mathf.RoundToInt(Racetrack.Instance.CountdownSeconds).ToString();
		m_countdownPreviousFrame = Mathf.RoundToInt(Racetrack.Instance.CountdownSeconds); 
	}

	private void Awake()
	{
		TryGetComponent<Animator>(out m_animator); 
	}

	private void Start()
    {
		GetPlayerReference();
		SubscribeEvent(); 
    }    

	private void GetPlayerReference() //Find all cars and check if any of them is a player
	{
		allContestants = FindObjectsOfType<RaceContestant>();

		if (allContestants.Length > 0)
		{
			for (int i = 0; i < allContestants.Length; i++)
			{
				if (allContestants[i].IsPlayer)
					m_player = allContestants[i];
			}
		}

		if (m_player == null)
			m_racePosText.enabled = false;
	}

	private void ShowResultsPanel()
	{
		m_resultsPanel.SetActive(true); 
	}

	private void HideResultsPanel()
	{
		m_resultsPanel.SetActive(false);
	}

	private void SubscribeEvent()
	{
		if (!Racetrack.Instance)
			return;

		Racetrack.Instance.OnRaceFinishedHandler += ShowResultsPanel;

		if (!GameManager.Instance)
			return;

		GameManager.Instance.OnReplayStartedHandler += HideResultsPanel; 
		GameManager.Instance.OnReplayFinishedHandler += ShowResultsPanel; 
	}

	private void UnsubscribeEvent()
	{
		if (!Racetrack.Instance)
			return;

		Racetrack.Instance.OnRaceFinishedHandler -= ShowResultsPanel;

		if (!GameManager.Instance)
			return;

		GameManager.Instance.OnReplayStartedHandler -= HideResultsPanel;
		GameManager.Instance.OnReplayFinishedHandler -= ShowResultsPanel;
	}

	private void OnDisable()
	{
		UnsubscribeEvent(); 
	}
}
