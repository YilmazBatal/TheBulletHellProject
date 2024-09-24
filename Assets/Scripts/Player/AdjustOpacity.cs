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

//using System.Collections.Generic;
//using UnityEngine;

//public class AdjustOpacity : MonoBehaviour {
//	private List<SpriteRenderer> obstacleSprites = new List<SpriteRenderer>();

//	private void OnTriggerEnter2D(Collider2D collision) {
//		if (collision.CompareTag("Obstacle")) {
//			SpriteRenderer obstacleSprite = collision.GetComponent<SpriteRenderer>();
//			if (obstacleSprite != null && !obstacleSprites.Contains(obstacleSprite)) {
//				obstacleSprites.Add(obstacleSprite);
//				AdjustTheOpacity(obstacleSprite, 0.4f);
//			}
//		}
//	}

//	private void OnTriggerExit2D(Collider2D collision) {
//		if (collision.CompareTag("Obstacle")) {
//			SpriteRenderer obstacleSprite = collision.GetComponent<SpriteRenderer>();
//			if (obstacleSprite != null && obstacleSprites.Contains(obstacleSprite)) {
//				AdjustTheOpacity(obstacleSprite, 1f);
//				obstacleSprites.Remove(obstacleSprite);
//			}
//		}
//	}

//	void AdjustTheOpacity(SpriteRenderer obstacleSprite, float targetOpacity) {
//		LeanTween.value(gameObject, opacity => SetOpacity(obstacleSprite, opacity), obstacleSprite.color.a, targetOpacity, 0.15f).setEase(LeanTweenType.easeInOutCubic);
//	}

//	private void SetOpacity(SpriteRenderer sprite, float opacity) {
//		Color color = sprite.color;
//		color.a = opacity;
//		sprite.color = color;
//	}
//}
