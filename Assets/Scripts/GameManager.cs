using UnityEngine;

public class GameManager : MonoBehaviour
{
	[SerializeField] Texture2D cursorTexture;	
	private void Awake() {
		Application.targetFrameRate = 60;
		Cursor.SetCursor(cursorTexture, new Vector2(32, 32), CursorMode.Auto);
	}
}
