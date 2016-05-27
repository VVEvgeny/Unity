using UnityEngine;

public class TouchInputManager : MonoBehaviour
{
    private Vector2 _fingerStartPos = Vector2.zero;
    private float _fingerStartTime;

    private GameManager _gm;

    private bool _isSwipe;
    private readonly float maxSwipeTime = 0.5f;
    private readonly float minSwipeDist = 50.0f;

    private void Awake()
    {
        _gm = FindObjectOfType<GameManager>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (_gm.State == GameStates.Playing && Input.touchCount > 0)
        {
            foreach (var touch in Input.touches)
            {
                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        /* this is a new touch */
                        _isSwipe = true;
                        _fingerStartTime = Time.time;
                        _fingerStartPos = touch.position;
                        break;

                    case TouchPhase.Canceled:
                        /* The touch is being canceled */
                        _isSwipe = false;
                        break;

                    case TouchPhase.Ended:

                        var gestureTime = Time.time - _fingerStartTime;
                        var gestureDist = (touch.position - _fingerStartPos).magnitude;

                        if (_isSwipe && gestureTime < maxSwipeTime && gestureDist > minSwipeDist)
                        {
                            var direction = touch.position - _fingerStartPos;
                            var swipeType = Vector2.zero;

                            if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
                            {
                                // the swipe is horizontal:
                                swipeType = Vector2.right*Mathf.Sign(direction.x);
                            }
                            else
                            {
                                // the swipe is vertical:
                                swipeType = Vector2.up*Mathf.Sign(direction.y);
                            }

                            if (swipeType.x != 0.0f)
                            {
                                if (swipeType.x > 0.0f)
                                {
                                    // MOVE RIGHT
                                    _gm.Move(MoveDirection.Right);
                                }
                                else
                                {
                                    // MOVE LEFT
                                    _gm.Move(MoveDirection.Left);
                                }
                            }

                            if (swipeType.y != 0.0f)
                            {
                                if (swipeType.y > 0.0f)
                                {
                                    // MOVE UP
                                    _gm.Move(MoveDirection.Up);
                                }
                                else
                                {
                                    // MOVE DOWN
                                    _gm.Move(MoveDirection.Down);
                                }
                            }
                        }
                        break;
                }
            }
        }
    }
}