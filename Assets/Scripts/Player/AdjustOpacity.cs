using UnityEngine;

public class AdjustOpacity : MonoBehaviour
{
	SpriteRenderer obstacleSprite;

	private void OnTriggerEnter2D(Collider2D collision) {
		if (collision.CompareTag("Obstacle")) {
			obstacleSprite = collision.GetComponent<SpriteRenderer>();
			AdjustTheOpacity(0.4f);
		}
	}
	private void OnTriggerExit2D(Collider2D collision) {
		if (collision.CompareTag("Obstacle")) {
			obstacleSprite = collision.GetComponent<SpriteRenderer>();
			AdjustTheOpacity(1f);
		}
	}
	void AdjustTheOpacity(float targetOpacity) {
		LeanTween.value(gameObject, SetOpacity, obstacleSprite.color.a, targetOpacity, 0.15f).setEase(LeanTweenType.easeInOutCubic);
	}

	private void SetOpacity(float opacity) {
		Color color = obstacleSprite.color;
		color.a = opacity;
		obstacleSprite.color = color;
	}
}