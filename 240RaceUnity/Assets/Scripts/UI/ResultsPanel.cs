using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic; 

public class ResultsPanel : MonoBehaviour
{
    /*
        The purpose of this script is to update
        the results panel with the contestants
        in their respective positions on the board.

        This script also sets up the panels button events.
    */

    [SerializeField]
    private Text[] m_contestantTexts;
    [SerializeField]
    private Button m_restartButton;
    [SerializeField]
    private Button m_replayButton; 

    private List<RaceContestant> m_contestants;

    private void UpdateResults()
	{
		for (int i = 0; i < m_contestants.Count; i++)
		{
            m_contestantTexts[i].text = m_contestants[i].name; 
		}
	}

	private void OnEnable()
	{
        GetContestants();
        UpdateResults();
        SetupButtonListeners(); 
	}

    private void GetContestants()
	{
        if (!Racetrack.Instance)
            return;

        m_contestants = Racetrack.Instance.GetFinishedContestants(); 
	}   

    private void SetupButtonListeners()
	{
        if (!GameManager.Instance)
            return; 

        m_restartButton.onClick.AddListener(delegate { GameManager.Instance.RestartLevel(); });
        m_replayButton.onClick.AddListener(delegate { GameManager.Instance.StartReplay(); });
	}    
}
