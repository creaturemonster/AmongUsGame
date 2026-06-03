// This is a template Unity script to demonstrate how the SpotlightGameLogic can be integrated.
// To use this, you need to create a Unity project, copy the logic files, and attach this script to a GameObject.

/*
using UnityEngine;
using TMPro; // Assuming use of TextMeshPro for UI
using CrowdPleaser.Logic;

public class UnitySpotlightGame : MonoBehaviour
{
    private SpotlightGameLogic _gameLogic;

    [Header("UI Elements")]
    public TextMeshProUGUI statsText;
    public GameObject winScreen;
    public GameObject lossScreen;

    [Header("Game Objects")]
    public Transform playerTransform;
    public Transform spotlightTransform;

    // We need a visual representation of the grid size.
    public float gridCellSize = 1.0f;
    public Vector3 gridOffset = Vector3.zero;

    void Start()
    {
        _gameLogic = new SpotlightGameLogic(40, 20);

        if (winScreen) winScreen.SetActive(false);
        if (lossScreen) lossScreen.SetActive(false);
    }

    void Update()
    {
        if (_gameLogic.CurrentState != GameState.Playing)
            return;

        HandleInput();

        // Pass Unity's Time.deltaTime to the logic
        _gameLogic.Update(Time.deltaTime);

        UpdateVisuals();
        UpdateUI();
        CheckWinLoss();
    }

    void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
            _gameLogic.MovePlayer(Direction.Up);
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
            _gameLogic.MovePlayer(Direction.Down);
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            _gameLogic.MovePlayer(Direction.Left);
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            _gameLogic.MovePlayer(Direction.Right);
    }

    void UpdateVisuals()
    {
        // Map logical grid coordinates to Unity world space coordinates
        if (playerTransform != null)
        {
            playerTransform.position = GridToWorldPosition(_gameLogic.PlayerX, _gameLogic.PlayerY);
        }

        if (spotlightTransform != null)
        {
            spotlightTransform.position = GridToWorldPosition((float)_gameLogic.SpotlightX, (float)_gameLogic.SpotlightY);
        }
    }

    Vector3 GridToWorldPosition(float gridX, float gridY)
    {
        // Example mapping: assumes origin (0,0) is bottom-left or top-left depending on your camera setup.
        // You may need to invert Y depending on if Unity Y goes up but console Y goes down.
        // Assuming Console: Y=0 is top, Y=19 is bottom.
        // Assuming Unity: Y=0 is bottom, Y=19 is top. We'll invert Y for standard Unity 2D.
        float worldX = gridX * gridCellSize;
        float worldY = (_gameLogic.Height - 1 - gridY) * gridCellSize;

        return new Vector3(worldX, worldY, 0) + gridOffset;
    }

    void UpdateUI()
    {
        if (statsText != null)
        {
            string hearts = new string('♥', _gameLogic.Lives);
            statsText.text = $"Audience: {hearts}\n" +
                             $"Time in Spotlight: {_gameLogic.TimeInSpotlight:F1} / 30.0 s\n" +
                             $"Time Out (loss at 3s): {_gameLogic.TimeOutOfSpotlight:F1} s";
        }
    }

    void CheckWinLoss()
    {
        if (_gameLogic.CurrentState == GameState.Win)
        {
            if (winScreen) winScreen.SetActive(true);
            Debug.Log("YOU WIN! You kept the crowd pleased!");
        }
        else if (_gameLogic.CurrentState == GameState.Loss)
        {
            if (lossScreen) lossScreen.SetActive(true);
            Debug.Log("GAME OVER! All audience members left.");
        }
    }
}
*/
