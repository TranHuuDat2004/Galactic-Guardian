using UnityEngine;

// Lớp này không cần file riêng, nó sẽ được dùng bên trong WaveData
[System.Serializable]
public class FormationStep
{
    public EnemyFormationData formationData;
    public float delayBefore = 1f; // Chờ bao lâu trước khi cho đội hình này xuất hiện
}

[CreateAssetMenu(fileName = "New Wave", menuName = "Wave System/Wave")]
public class WaveData : ScriptableObject
{
    public FormationStep[] formationSteps; // Một chuỗi các đội hình
}