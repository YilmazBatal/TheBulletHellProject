using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    #region Variables
    [SerializeField] Transform player;
    
    Vector2 cursorPosition;

	bool isFacingRight = true;

	#endregion

    void Update()
    {
        cursorPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);


		if (cursorPosition.x <= player.position.x && !isFacingRight)
			LeanTween.value(gameObject, FlipPlayer, 1, -1, 0.15f).setEase(LeanTweenType.easeInOutCubic);
		else if (cursorPosition.x > player.position.x && isFacingRight)
			LeanTween.value(gameObject, FlipPlayer, -1, 1, 0.15f).setEase(LeanTweenType.easeInOutCubic);
	}

	void FlipPlayer(float val) {
		isFacingRight = !isFacingRight;
        player.localScale = new Vector3(val, 1, 1);

	}
}
