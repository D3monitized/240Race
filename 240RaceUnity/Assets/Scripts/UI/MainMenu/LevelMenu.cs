using UnityEngine;
using UnityEngine.UI;
using UnityEditor; 

public class LevelMenu : MonoBehaviour
{
	public static LevelMenu Instance; 

	[SerializeField]
	private Transform m_levelSelectorParent; 

	[SerializeField]
	private GameObject m_levelPrefab;

	private string[] m_guids;
	private RacetrackSaveFile[] m_saveFiles;

	private void CreateLevelButtons()
	{
		foreach(RacetrackSaveFile sf in m_saveFiles)
		{
			GameObject lb = Instantiate(m_levelPrefab);
			lb.transform.SetParent(m_levelSelectorParent);
			lb.GetComponent<Panel_Level>().MySaveFile = sf; 
		}
	}

	private void GetSaveFiles()
	{
		m_guids = AssetDatabase.FindAssets("t:RacetrackSaveFile");

		m_saveFiles = new RacetrackSaveFile[m_guids.Length]; 

		for (int i = 0; i < m_guids.Length; i++)
		{
			string path = AssetDatabase.GUIDToAssetPath(m_guids[i]);
			m_saveFiles[i] = AssetDatabase.LoadAssetAtPath(path, typeof(RacetrackSaveFile)) as RacetrackSaveFile;
		}
	}

	private void Awake()
	{
		Instance = this; 
	}

	private void Start()
	{
		GetSaveFiles();
		CreateLevelButtons();
	}
}
