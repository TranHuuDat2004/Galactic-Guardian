using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))] // Bắt buộc phải có Sprite Renderer
public class SelfDestructEffect : MonoBehaviour
{
    public float displayDuration = 2.0f; // Tổng thời gian hiệu ứng tồn tại
    public float fadeOutTime = 0.5f;   // Thời gian để mờ đi

    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        StartCoroutine(DisplayAndFadeOut());
    }

    private IEnumerator DisplayAndFadeOut()
    {
        // Chờ một khoảng thời gian để người chơi nhìn thấy cảnh báo
        yield return new WaitForSeconds(displayDuration - fadeOutTime);

        // Bắt đầu quá trình làm mờ
        float elapsedTime = 0f;
        Color startColor = spriteRenderer.color;

        while (elapsedTime < fadeOutTime)
        {
            // Tính toán màu mới với độ alpha giảm dần
            float alpha = Mathf.Lerp(startColor.a, 0, elapsedTime / fadeOutTime);
            spriteRenderer.color = new Color(startColor.r, startColor.g, startColor.b, alpha);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Sau khi mờ hoàn toàn, hủy đối tượng
        Destroy(gameObject);
    }
}