using UnityEngine;
using System.Collections.Generic; 

public class RacetrackSaveFile : ScriptableObject
{
    [SerializeField] [HideInInspector]
    public string Name; //Name of savefile

    [SerializeField] [HideInInspector]
    public List<RacetrackTileSaveFile> Tiles; //A list of all racetrack tiles
}
