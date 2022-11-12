using UnityEngine;

public class RacetrackTile : MonoBehaviour
{
	public bool isFinishLine;

	[SerializeField] [HideInInspector]
	public RacetrackTileSaveFile MySave; 

	public delegate void OnCheckpoint(RaceContestant contestant, RacetrackTile tile);
	public OnCheckpoint OnCheckpointHandler;

	[SerializeField]
	private MyType m_myType;

	//Check if path towards direction is blocked by collider
	public bool IsBlocked(Vector2Int dir)
	{
		switch (m_myType)
		{
			case MyType.Straight:
				if(Mathf.RoundToInt(Vector2.Dot(transform.up, Vector2.up)) == 1 || Mathf.RoundToInt(Vector2.Dot(transform.up, Vector2.up)) == -1)
				{
					if (dir.x == 1 || dir.x == -1)
						return true; 
				}
				else
				{
					if (dir.y == 1 || dir.y == -1)
						return true;
				}
			break;
			case MyType.Corner:
				if(Mathf.RoundToInt(Vector2.Dot(transform.up, Vector2.up)) == 1)
				{
					if (dir.x == -1 || dir.y == 1)
						return true;
				}
				else if(Mathf.RoundToInt(Vector2.Dot(transform.up, Vector2.up)) == -1)
				{
					if (dir.x == 1 || dir.y == -1)
						return true;
				}
				else //Dot up == 0
				{
					if(Mathf.RoundToInt(Vector2.Dot(transform.up, Vector2.right)) == 1)
					{
						if (dir.x == 1 || dir.y == 1)
							return true;
					}
					else if(Mathf.RoundToInt(Vector2.Dot(transform.up, Vector2.right)) == -1)
					{
						if (dir.x == -1 || dir.y == -1)
							return true;
					}
				}
			break;
		}

		return false;
	}

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

	private enum MyType
	{
		Straight, Corner
	}
}
