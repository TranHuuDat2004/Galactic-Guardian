using UnityEngine;

[CreateAssetMenu(fileName = "New Formation", menuName = "Wave System/Enemy Formation")]
public class EnemyFormationData : ScriptableObject
{
    public GameObject enemyPrefab;
    public int numberOfEnemies = 5;
    public float moveSpeed = 2f;
    public MovementPattern movementPattern = MovementPattern.StraightDown;
    public Vector2 startPosition = new Vector2(0, 6);
    public Vector2 spacing = new Vector2(1.5f, 0);
}