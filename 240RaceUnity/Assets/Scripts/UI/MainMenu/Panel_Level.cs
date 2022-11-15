using UnityEngine;
using UnityEngine.UI;
using UnityEditor; 

public class Panel_Level : MonoBehaviour
{
	[HideInInspector]
	public RacetrackSaveFile MySaveFile;

	[SerializeField]
	private Text m_saveName;
	[SerializeField]
	private RawImage m_image; 
	[SerializeField]
	private Button m_delete;
	[SerializeField]
	private Button m_load;

	private void LoadLevel()
	{
		if (!GameManager.Instance)
			return;

		GameManager.Instance.LoadedTrack = MySaveFile;
		GameManager.Instance.StartCoroutine(GameManager.Instance.LoadLevel(2));
	}

	private void DeleteLevel()
	{
		AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(MySaveFile));
		Destroy(gameObject);
	}

	private void Start()
	{
		m_saveName.text = MySaveFile.Name;

		if (MySaveFile.Image)
			m_image.texture = MySaveFile.Image; 

		m_delete.onClick.AddListener(delegate { DeleteLevel(); });
		m_load.onClick.AddListener(delegate { LoadLevel(); });
	}
}
