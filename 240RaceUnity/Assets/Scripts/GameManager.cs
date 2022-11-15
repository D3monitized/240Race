using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections; 

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

	public RacetrackSaveFile LoadedTrack;
	public int AmountOfPlayers = 1; 

	public delegate void OnSceneLoaded();
	public OnSceneLoaded OnSceneLoadedHandler;

	public delegate void OnSceneReloaded();
	public OnSceneReloaded OnSceneReloadedHandler; 

	public IEnumerator LoadLevel(int buildIndex)
	{
		SceneManager.LoadScene(buildIndex);

		yield return new WaitForSeconds(.1f);

		if (OnSceneLoadedHandler != null)
			OnSceneLoadedHandler.Invoke();
	}

	public void RestartLevel()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}	

	private void Awake()
	{
		if (Instance != null && Instance != this)
			Destroy(gameObject);
		else
		{
			Instance = this;
			DontDestroyOnLoad(gameObject); 
		}
	}	
}
