using UnityEngine;

[CreateAssetMenu(fileName = "New Racetrack Tile Collection", menuName = "Editor/Racetrack Tile Collection")]
public class RacetrackTilesBase : ScriptableObject
{
    [SerializeField]
    private GameObject[] m_tiles; 
    public GameObject[] GetTiles() { return m_tiles; }    
}
