using UnityEngine;
using UnityEngine.UI; 

public class Panel_LevelEditor : MonoBehaviour
{
    [SerializeField]
    private Button m_loadButton;

	private void LoadLevelEditor()
	{
		if (!GameManager.Instance)
			return;

		GameManager.Instance.StartCoroutine(GameManager.Instance.LoadLevel(1));
	}

	private void Start()
	{
		m_loadButton.onClick.AddListener(delegate { LoadLevelEditor(); });
	}
}
