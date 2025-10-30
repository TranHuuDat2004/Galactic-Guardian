// PlayerData.cs
[System.Serializable] // Dòng này rất quan trọng để có thể chuyển đổi thành JSON
public class PlayerData
{
    public int currentLevel;
    public int lives;
    public int weaponLevel;
    public WeaponType currentWeaponType;

    // Hàm khởi tạo để tạo dữ liệu mặc định cho người chơi mới
    public PlayerData()
    {
        currentLevel = 1;
        lives = 5; // Hoặc giá trị startingLives mặc định của bạn
        weaponLevel = 1;
        currentWeaponType = WeaponType.Default;
    }
}