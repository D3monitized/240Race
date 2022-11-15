using UnityEngine;
using UnityEngine.InputSystem;
using UnityEditor; 
using System.Collections.Generic;
using System.Collections; 

public class LevelEditor : MonoBehaviour
{
	/*
		This script is the level editor in
		it's core. This contains all the placeable
		tiles aswell as the grid they're placed on. 
	*/

	public static LevelEditor Instance; 

	public RacetrackTilesBase Tiles; //All tiles that are placeable

	//Grid
	public Vector2Int GetGridSize() { return m_gridSize; }
	[SerializeField]
	private Vector2Int m_gridSize;
	public float GetNodeSize() { return m_nodeDiameter; }
	private float m_nodeDiameter = 10;
	private GridNode[,] m_grid;
	
	//Components
	[SerializeField]
	private Sprite m_sprite; //Node sprite (Grid square)

	private GameObject m_currentTile; //The currently selected tile 

	private Transform m_raceTileParent;
	private Transform m_gridNodeParent;

	//Level Saving
	[SerializeField]
	private List<RacetrackTileSaveFile> m_tiles;
	private const string m_savePath = "Assets/SaveData/UserLevels";
	private string m_latestSavePath; 

	public void ChangeCurrentTile(GameObject tile) => m_currentTile = tile; 
	private void PlaceTile(Vector2 mousePosition)
	{
		Vector2 placePos = Camera.main.ScreenToWorldPoint(mousePosition);
		Vector2Int gridPos = WorldToGridPosition(placePos); 		

		//if targeted tile doesn't exist or is occupied -> return
		if (gridPos.x < 0 || gridPos.x > m_gridSize.x - 1|| gridPos.y < 0 || gridPos.y > m_gridSize.y - 1 || m_grid[gridPos.x, gridPos.y].Occupied)
			return; 

		//Set racetile parent to some empty GameObject
		m_grid[gridPos.x, gridPos.y].RaceTile = Instantiate(m_currentTile, m_grid[gridPos.x, gridPos.y].WorldPosition, Quaternion.identity); //Instantiate racetrack tile at nodeposition (gridPos)
		m_grid[gridPos.x, gridPos.y].RaceTile.GetComponent<RacetrackTile>().MySave = new RacetrackTileSaveFile();
		m_grid[gridPos.x, gridPos.y].RaceTile.GetComponent<RacetrackTile>().MySave.MyPrefab = m_currentTile; //Cache tile's prefab for instantiating later
		m_grid[gridPos.x, gridPos.y].RaceTile.GetComponent<RacetrackTile>().MySave.MyPosition = m_grid[gridPos.x, gridPos.y].RaceTile.transform.position; //Cache position for loading level later	
		m_grid[gridPos.x, gridPos.y].RaceTile.transform.SetParent(m_raceTileParent); 

		m_grid[gridPos.x, gridPos.y].Occupied = true; //Set occupied to true so only one tile can take up a node
	}

	private void RemoveTile(Vector2 mousePosition)
	{
		Vector2 removePos = Camera.main.ScreenToWorldPoint(mousePosition);
		Vector2Int gridPos = WorldToGridPosition(removePos); 

		//if targeted tile doesn't exist or is occupied -> return
		if (gridPos.x < 0 || gridPos.x > m_gridSize.x - 1 || gridPos.y < 0 || gridPos.y > m_gridSize.y - 1 || !m_grid[gridPos.x, gridPos.y].Occupied)
			return;

		//Destroy targeted racetile
		Destroy(m_grid[gridPos.x, gridPos.y].RaceTile);
		m_grid[gridPos.x, gridPos.y].Occupied = false; 
	}

	private void RotateTile(Vector2 mousePosition)
	{
		Vector2 rotatePos = Camera.main.ScreenToWorldPoint(mousePosition);
		Vector2Int gridPos = WorldToGridPosition(rotatePos);

		//if targeted tile doesn't exist or is occupied -> return
		if (gridPos.x < 0 || gridPos.x > m_gridSize.x - 1 || gridPos.y < 0 || gridPos.y > m_gridSize.y - 1 || !m_grid[gridPos.x, gridPos.y].Occupied)
			return;

		m_grid[gridPos.x, gridPos.y].RaceTile.transform.eulerAngles += Vector3.forward * 90; //Rotate 90 degrees
		m_grid[gridPos.x, gridPos.y].RaceTile.GetComponent<RacetrackTile>().MySave.MyRotation = m_grid[gridPos.x, gridPos.y].RaceTile.transform.rotation; //Cache rotation for loading level later
	}

	private Vector2Int WorldToGridPosition(Vector2 position)
	{		
		float percentageX = position.x / (m_nodeDiameter * m_gridSize.x); //Divide by gridsize to get percentual position
		float percentageY = position.y / (m_nodeDiameter * m_gridSize.y);

		return new Vector2Int(Mathf.FloorToInt(m_gridSize.x * percentageX), Mathf.FloorToInt(m_gridSize.y * percentageY)); //Return closest gridPosition to worldPosition
	}

	private void OnPlaceTileHandle(InputAction.CallbackContext context)
	{
		//Since this function will be called once a mouse button is released,
		//the value will be 0 resulting in the RaceTile instantly being destroyed after created. (This will prevent that :D)
		if (context.canceled) 
			return; 

		if (context.ReadValue<float>() == 1)
			PlaceTile(Mouse.current.position.ReadValue());
		else
			RemoveTile(Mouse.current.position.ReadValue()); 
	}

	private void OnRotateTileHandle(InputAction.CallbackContext context)
	{
		if(context.started) //Only call when key is pressed (not when it's released)
			RotateTile(Mouse.current.position.ReadValue()); 
	}

	private bool IsNotOutOfBounds(Vector2Int gridPos)
	{
		return gridPos.x > -1 && gridPos.x < m_gridSize.x && gridPos.y > -1 && gridPos.y < m_gridSize.y;
	}

	private void CreateGrid()
	{
		m_grid = new GridNode[m_gridSize.x, m_gridSize.y]; //Initialize grid with it's gridsize
		m_raceTileParent = new GameObject().transform; //Create an empty GameObject that can act as a parent for all racetiles. (More pleasing for the eyes)
		m_raceTileParent.name = "Racetrack Tiles";
		m_gridNodeParent = new GameObject().transform;
		m_gridNodeParent.name = "Grid"; 

		for (int i = 0; i < m_gridSize.y; i++)
		{
			for (int j = 0; j < m_gridSize.x; j++)
			{
				m_grid[j, i] = new GridNode(new Vector2Int(j, i), m_nodeDiameter, m_sprite);
				GameObject temp = new GameObject();
				temp.name = "Node [" + j + ", " + i + "]"; //Name node to node and it's gridpos
				temp.transform.position = m_grid[j, i].WorldPosition; //Assing the node (GameObject) to the corresponding nodes WorldPosition. (WorldPosition is calculated in node constructor).
				temp.transform.localScale = new Vector2(m_nodeDiameter, m_nodeDiameter);
				temp.transform.SetParent(m_gridNodeParent); 
				temp.AddComponent<SpriteRenderer>().sprite = m_sprite; //Set sprite to node sprite (Grid square)
			}
		}
	}

	//Create a new savefile asset in directory
	public bool SaveLevel(string name)
	{
		SortLevelTiles(); //Create a list out of the level tiles that's sorted in correct order

		RacetrackSaveFile saveFile = ScriptableObject.CreateInstance<RacetrackSaveFile>(); //Create a new instance of a save file
		saveFile.Tiles = m_tiles; //Copy list of tiles to savefile
		saveFile.Name = name;
		
		string savePath = m_savePath + "/" + name + ".asset";
		m_latestSavePath = savePath; 

		string[] userLevels = AssetDatabase.FindAssets("t:RacetrackSaveFile"); //Serch for assets of type RacetrackSaveFile
		
		if (userLevels.Length > 0)
			foreach (string guid in userLevels)
				if (AssetDatabase.GUIDToAssetPath(guid) == savePath) //if found asset has same path as new savepath -> return false
					return false; 

		AssetDatabase.CreateAsset(saveFile, savePath); //Create an asset out of savefile instance
		AssetDatabase.SaveAssets(); //Save asset
		AssetDatabase.Refresh(); //Refresh directory
		Selection.activeObject = saveFile; //Focus on savefile in directory

		DestroyAndRecreateLevel(); //Visualize the order the AI will traverse the level in
		return true;
	}

	private void SortLevelTiles() //Sort in correct order for ai to be able to traverse the level in the correct order
	{
		m_tiles = new List<RacetrackTileSaveFile>(); 

		Vector2Int currentTile = new Vector2Int();
		Vector2Int previousTile = new Vector2Int(); 

		for (int i = 0; i < m_raceTileParent.childCount; i++)
		{
			//Check if this racetrack tile is finishline
			if (m_raceTileParent.GetChild(i).GetComponent<RacetrackTile>().isFinishLine)
			{
				RacetrackTile rt = m_raceTileParent.GetChild(i).GetComponent<RacetrackTile>();
				m_tiles.Add(rt.MySave); //Add goal to racetrack tile list
				currentTile = WorldToGridPosition(m_raceTileParent.GetChild(i).transform.position); //Cache finishline grid position as previous tile
				break; 
			}
		}

		//Find the next tile by going through the neigbours, checking that they're not empty and that they're not the previously added tile
		for (int i = 0; i < m_raceTileParent.childCount - 1; i++)
		{
			Vector2Int top = currentTile + Vector2Int.up;
			Vector2Int bottom = currentTile - Vector2Int.up;
			Vector2Int right = currentTile + Vector2Int.right;
			Vector2Int left = currentTile - Vector2Int.right;

			//Go through all the neighbours and find one that is not out of bounds, not the previous tile, that is occupied by a tile and doesn't have a collider in the way.


			if(IsNotOutOfBounds(top) && m_grid[top.x, top.y].Occupied && top != previousTile && !m_grid[currentTile.x, currentTile.y].RaceTile.GetComponent<RacetrackTile>().IsBlocked(Vector2Int.up))
			{
				RacetrackTile rt = m_grid[top.x, top.y].RaceTile.GetComponent<RacetrackTile>();			
				m_tiles.Add(rt.MySave);
				previousTile = currentTile;
				currentTile = top;
				continue; 
			}
			else if(IsNotOutOfBounds(bottom) && m_grid[bottom.x, bottom.y].Occupied && bottom != previousTile && !m_grid[currentTile.x, currentTile.y].RaceTile.GetComponent<RacetrackTile>().IsBlocked(Vector2Int.down))
			{
				RacetrackTile rt = m_grid[bottom.x, bottom.y].RaceTile.GetComponent<RacetrackTile>();
				m_tiles.Add(rt.MySave);
				previousTile = currentTile;
				currentTile = bottom;
				continue;
			}
			else if (IsNotOutOfBounds(right) && m_grid[right.x, right.y].Occupied && right != previousTile && !m_grid[currentTile.x, currentTile.y].RaceTile.GetComponent<RacetrackTile>().IsBlocked(Vector2Int.right))
			{
				RacetrackTile rt = m_grid[right.x, right.y].RaceTile.GetComponent<RacetrackTile>();
				m_tiles.Add(rt.MySave);
				previousTile = currentTile;
				currentTile = right;
				continue;
			}
			else if (IsNotOutOfBounds(left) && m_grid[left.x, left.y].Occupied && left != previousTile && !m_grid[currentTile.x, currentTile.y].RaceTile.GetComponent<RacetrackTile>().IsBlocked(Vector2Int.left))
			{
				RacetrackTile rt = m_grid[left.x, left.y].RaceTile.GetComponent<RacetrackTile>();
				m_tiles.Add(rt.MySave);
				previousTile = currentTile; 
				currentTile = left;
				continue;
			}
		}
	}

	private void DestroyAndRecreateLevel()
	{
		for (int i = 0; i < m_raceTileParent.childCount; i++)
		{
			Destroy(m_raceTileParent.GetChild(i).gameObject);
		}

		StartCoroutine(RecreateLevelInOrder()); 
	}	

	private int iteration; 
	private IEnumerator RecreateLevelInOrder() //Visualization of sorted level -> reconstructed in correct order
	{
		yield return new WaitForSecondsRealtime(.25f);

		GameObject go = Instantiate(
			m_tiles[iteration].MyPrefab, 
			m_tiles[iteration].MyPosition, 
			m_tiles[iteration].MyRotation
		);

		go.transform.SetParent(m_raceTileParent); 

	
		if(iteration < m_tiles.Count - 1)
		{
			iteration++;
			StartCoroutine(RecreateLevelInOrder());
		}
		else
		{
			GameObject.Find("UI").GetComponent<Canvas>().enabled = false;
			yield return new WaitForEndOfFrame(); 
			Texture2D tex = ScreenCapture.CaptureScreenshotAsTexture();
			GameObject.Find("UI").GetComponent<Canvas>().enabled = true;
			RacetrackSaveFile saveFile = AssetDatabase.LoadAssetAtPath(m_latestSavePath, typeof(RacetrackSaveFile)) as RacetrackSaveFile;
			AssetDatabase.AddObjectToAsset(tex, saveFile);
			saveFile.Image = tex;
			AssetDatabase.SaveAssets(); 
		}
	}	

	private void Awake()
	{
		Instance = this; 
	}

	private void Start()
	{
		CreateGrid();

		if (!InputManager_LevelEditor.Instance)
			return;

		InputManager_LevelEditor.Instance.OnPlaceTileHandler += OnPlaceTileHandle;
		InputManager_LevelEditor.Instance.OnRotateTileHandler += OnRotateTileHandle;
	}

	private void OnDisable()
	{
		if (!InputManager_LevelEditor.Instance)
			return;

		InputManager_LevelEditor.Instance.OnPlaceTileHandler -= OnPlaceTileHandle;
		InputManager_LevelEditor.Instance.OnRotateTileHandler -= OnRotateTileHandle;
	}
}

public class GridNode
{
	public Vector2Int GridPosition;
	public Vector2 WorldPosition;
	public float NodeDiameter; 
	public GameObject RaceTile;
	public Sprite Node;
	public bool Occupied = false; 

	public GridNode(Vector2Int gridPos, float diameter, Sprite node)
	{
		GridPosition = gridPos;
		NodeDiameter = diameter;
		Node = node; 
		WorldPosition = GridToWorldPosition(GridPosition);
	}

	private Vector2 GridToWorldPosition(Vector2Int gridPos)
	{
		Vector2 temp = gridPos;
		return new Vector2((temp.x + 0.5f) * NodeDiameter, (temp.y + 0.5f) * NodeDiameter);
	}
}