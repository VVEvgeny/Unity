using UnityEngine;

public enum MoveDirection
{
    Left,
    Right,
    Up,
    Down
}

public class InputManager : MonoBehaviour
{
    private GameManager _gm;

    void Awake()
    {
        _gm = GameObject.FindObjectOfType<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_gm.State != GameStates.Playing) return;

        if (Input.GetKeyDown(KeyCode.RightArrow)) _gm.Move(MoveDirection.Right);
        else if (Input.GetKeyDown(KeyCode.LeftArrow)) _gm.Move(MoveDirection.Left);
        else if (Input.GetKeyDown(KeyCode.UpArrow)) _gm.Move(MoveDirection.Up);
        else if (Input.GetKeyDown(KeyCode.DownArrow)) _gm.Move(MoveDirection.Down);
    }
}
