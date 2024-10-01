using UnityEngine;

[CreateAssetMenu(fileName = "Mob", menuName = "ScriptableObjects/Mob", order = 1)]
public class MobSO : ScriptableObject {
	public new string name;
	public float hp;
	public float attack;
	public float movementSpeed;
}