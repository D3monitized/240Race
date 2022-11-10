using UnityEngine;

public class RaceContestant : MonoBehaviour
{
    /*
        This script holds information about the car
        in terms of the race. 
    */

    [HideInInspector]
    public int CurrentCheckpoint; //Which part of the map the car is on
    [HideInInspector]
    public int CurrentLap = 1; //Which lap the car is on
    [HideInInspector]
    public int CurrentPosition; //Which position in the race the car currently holds
    [HideInInspector]
    public int FinalPosition; //Which position the car finished the race in
    [HideInInspector] [Tooltip("Set on Awake!")]
    public bool IsPlayer = false; //Whether or not the car is controlled by the player
    

    public void OnFinished()
	{
        GetComponent<AICarBrain>().enabled = false;
        GetComponent<PlayerCarInput>().enabled = false;

        FinalPosition = CurrentPosition; 
	}

	private void Awake()
	{
        if (GetComponent<PlayerCarInput>().enabled)
            IsPlayer = true;
    }

	private void Start()
	{
        CurrentLap = 1;
        name = GetComponent<CarController>().Config.name; 
	}
}
