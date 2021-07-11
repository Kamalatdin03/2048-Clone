using UnityEngine;

public class SwipeInput : MonoBehaviour
{
	public static bool SwipedRigth { get; private set; }
	public static bool SwipedLeft { get; private set; }
	public static bool SwipedUp { get; private set; }
	public static bool SwipedDown { get; private set; }

	[SerializeField] private bool _debugWithArrowKeys = true;

	private Vector2 _fristTouchPosition;
	private float _fristTouchTime;

	private const float MAX_SWIPE_TIME = 0.5f;
	private const float MIN_SWIPE_DISTANCE = 0.17f;

    public void Update()
	{
		SwipedRigth = SwipedLeft = SwipedUp = SwipedDown = false;

		if (Input.touches.Length > 0)
		{
			Touch touch = Input.GetTouch(0);
			float screenWidth = Screen.width;

			if (touch.phase == TouchPhase.Began)
			{
				_fristTouchPosition = touch.position / screenWidth;
				_fristTouchTime = Time.time;
			}
			if (touch.phase == TouchPhase.Ended)
			{
				if (Time.time - _fristTouchTime > MAX_SWIPE_TIME) 
					return;

				Vector2 endPos = touch.position / screenWidth;

				Vector2 swipe = endPos - _fristTouchPosition;

				if (swipe.magnitude < MIN_SWIPE_DISTANCE) 
					return;

				if (Mathf.Abs(swipe.x) > Mathf.Abs(swipe.y))
				{ 
					if (swipe.x > 0) SwipedRigth = true;
					else SwipedLeft = true;
				}
				else
				{ 
					if (swipe.y > 0) SwipedUp = true;
					else SwipedDown = true;
				}
			}
		}

		if (_debugWithArrowKeys)
		{
			SwipedDown = SwipedDown || Input.GetKeyDown(KeyCode.DownArrow);
			SwipedUp = SwipedUp || Input.GetKeyDown(KeyCode.UpArrow);
			SwipedRigth = SwipedRigth || Input.GetKeyDown(KeyCode.RightArrow);
			SwipedLeft = SwipedLeft || Input.GetKeyDown(KeyCode.LeftArrow);
		}
	}
}