using UnityEngine;
using UnityEngine.UI; 

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject m_mainMenu;
    [SerializeField]
    private GameObject m_levelMenu; 

    //Main Menu
    [Header("Main Menu")]
    [SerializeField]
    private Button m_playButton;
    [SerializeField]
    private Button m_optionsButton;
    [SerializeField]
    private Button m_quitButton;

    //Level Menu
    [Header("Level Menu")]
    [SerializeField]
    private Button m_backButton; 

    private void ShowLevelMenu()
	{
        m_mainMenu.SetActive(false);
        m_levelMenu.SetActive(true); 
	}
    private void ShowMainMenu()
	{
        m_levelMenu.SetActive(false);
        m_mainMenu.SetActive(true);
    }

    private void ShowOptions()
	{
        //WIP Create options menu
	}

	private void Quit()
	{
        Application.Quit(); 
	}

	private void Start()
	{
        m_playButton.onClick.AddListener(delegate { ShowLevelMenu(); });
        m_backButton.onClick.AddListener(delegate { ShowMainMenu(); });
        m_optionsButton.onClick.AddListener(delegate { ShowOptions(); });
        m_quitButton.onClick.AddListener(delegate { Quit(); });
	}
}
