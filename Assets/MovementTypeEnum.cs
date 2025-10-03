public enum MovementType
{
    Vertical,       // Di chuyển thẳng xuống
    Diagonal,       // Di chuyển chéo và tự xoay
    Formation,       // Đứng yên và di chuyển theo đội hình cha
    Roaming, // << THÊM DÒNG NÀY VÀO

    // --- 5 KIỂU MỚI ---
    DiagonalRightToLeft, // Chéo 40 độ từ phải sang trái (hướng xuống)
    DiagonalLeftToRight, // Chéo 40 độ từ trái sang phải (hướng xuống)
    MoveUp,              // Di chuyển thẳng từ dưới lên
    HorizontalLeftToRight, // Di chuyển ngang từ trái sang phải
    HorizontalRightToLeft  // Di chuyển ngang từ phải sang trái
}