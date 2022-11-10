using UnityEngine;

[CreateAssetMenu(fileName = "New Car Config", menuName = "Config/CarConfig")]
public class CarConfigBase : ScriptableObject
{
	[SerializeField]
	private float MaxSpeed; //Max speed :-P
	[SerializeField]
	private float SteeringSensitivity; //How fast the car rotates/turns
	[SerializeField]
	private float AccelerationAmount; //How fast the car reaches Max speed
	[SerializeField]
	private float DecelerationAmount; //How fast the car decelerates (without breaking)
	[SerializeField]
	private float BrakeAmount; //How fast the car decelerates / accelerates in opposite direction (braking)
	[SerializeField]
	private float Traction; //How fast the car regains traction (starts moving in the forward direction of the car)
	[SerializeField]
	private Sprite CarSprite; //The car graphics (0 = default (no turning), 1 = turn left, 2 = turn right)

	//Getters
	public float GetMaxSpeed() { return MaxSpeed; }
	public float GetSteerSensitivty() { return SteeringSensitivity; }
	public float GetAccelerationAmount() { return AccelerationAmount; }
	public float GetDecelerationAmount() { return DecelerationAmount; }
	public float GetBrakeAmount() { return BrakeAmount; }
	public float GetTraction() { return Traction; }
	public Sprite GetSprite() { return CarSprite; }
}
