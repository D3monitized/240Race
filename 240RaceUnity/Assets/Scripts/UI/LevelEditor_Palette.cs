using UnityEngine;
using UnityEngine.UI;

public class LevelEditor_Palette : MonoBehaviour
{
	[SerializeField]
	private Button m_saveButton; 
	[SerializeField]
	private Button m_arrowButton;
	[SerializeField]
	private Button[] m_tileButtons;

	private bool m_paletteOpen = false; 

	private void SelectTile(Button button)
	{
		if (!LevelEditor.Instance)
			return;

		foreach (GameObject go in LevelEditor.Instance.Tiles.GetTiles()) //Go through all tile prefabs in collection
		{
			//Compare the button sprite to the sprite of the prefabs to determine which prefab to use. (not very orthodox)
			if (go.GetComponent<SpriteRenderer>().sprite == button.GetComponent<Image>().sprite)
			{
				LevelEditor.Instance.ChangeCurrentTile(go);
				break;
			}
		}

		foreach (Button b in m_tileButtons) //Reset color of previously chosen tile
			b.GetComponent<Image>().color = Color.white;

		button.GetComponent<Image>().color = Color.green; //Set selected tile button color to green to indicate that it's chosen
	}

	private void ShowHidePalette()
	{
		m_paletteOpen = !m_paletteOpen;

		GetComponent<Animator>().SetBool("Palette", m_paletteOpen);

		if (m_paletteOpen)
			m_arrowButton.GetComponentInChildren<Text>().text = "<";
		else
			m_arrowButton.GetComponentInChildren<Text>().text = ">";
	}

	private void Start()
	{
		m_tileButtons[0].onClick.AddListener(delegate { SelectTile(m_tileButtons[0]); });
		m_tileButtons[1].onClick.AddListener(delegate { SelectTile(m_tileButtons[1]); });
		SelectTile(m_tileButtons[1]);
		m_tileButtons[2].onClick.AddListener(delegate { SelectTile(m_tileButtons[2]); });
		
		m_arrowButton.onClick.AddListener(delegate { ShowHidePalette(); });

		if (!LevelEditor.Instance)
			return;

		m_saveButton.onClick.AddListener(delegate { LevelEditor.Instance.SaveLevel(); });
	}
}
