using UnityEngine;

[System.Serializable]
public class RacetrackTileSaveFile
{
    [SerializeField]
    public Vector2 MyPosition;
    [SerializeField]
    public Quaternion MyRotation; 
    [SerializeField]
    public GameObject MyPrefab; 
}
