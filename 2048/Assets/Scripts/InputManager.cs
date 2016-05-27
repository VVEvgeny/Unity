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

    private void Awake()
    {
        _gm = FindObjectOfType<GameManager>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (_gm.State != GameStates.Playing) return;

        if (Input.GetKeyDown(KeyCode.RightArrow)) _gm.Move(MoveDirection.Right);
        else if (Input.GetKeyDown(KeyCode.LeftArrow)) _gm.Move(MoveDirection.Left);
        else if (Input.GetKeyDown(KeyCode.UpArrow)) _gm.Move(MoveDirection.Up);
        else if (Input.GetKeyDown(KeyCode.DownArrow)) _gm.Move(MoveDirection.Down);
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (_gm.IsMainMenu)
            {
                //Debug.Log("QUit");
                Application.Quit();
            }
            _gm.ToMainMenuButtonHandler();
        }
    }
}