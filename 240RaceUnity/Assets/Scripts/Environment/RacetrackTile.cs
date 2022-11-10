using UnityEngine;

public class RacetrackTile : MonoBehaviour
{
	public bool isFinishLine; 

	public delegate void OnCheckpoint(RaceContestant contestant, RacetrackTile tile);
	public OnCheckpoint OnCheckpointHandler; 

    public virtual void OnCheckpointReached(RaceContestant contestant)
	{
		if (OnCheckpointHandler == null)
			return; 

		OnCheckpointHandler.Invoke(contestant, this); 
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (!collision.transform.parent.GetComponent<RaceContestant>())
			return;

		OnCheckpointReached(collision.transform.parent.GetComponent<RaceContestant>()); 
	}
}
