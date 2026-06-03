# Unity Migration Guide: CrowdPleaser

This document contains exhaustive research and guidelines for porting the `CrowdPleaser` C# console game to the Unity platform and ensuring it is production-ready.

## 1. Architecture Refactoring (Separation of Concerns)

The current implementation in `Program.cs` tightly couples game logic with `System.Console` rendering and a blocking `while(true)` game loop.

**Steps to resolve:**
- **Decouple Logic and Presentation:** Extract the core game state (player position, spotlight position, lives, time) into a separate non-Unity-specific C# class (e.g., `GameEngine`).
- **Remove the While Loop:** Unity uses an event-driven architecture based on `MonoBehaviour` scripts. The infinite `while` loop with `Thread.Sleep` must be removed. Instead, game logic should be updated once per frame inside Unity's `Update()` method.

## 2. Input Handling

The console game uses `Console.KeyAvailable` and `Console.ReadKey` for input.

**Unity Replacement:**
- Use the traditional `Input` manager (e.g., `Input.GetKeyDown(KeyCode.UpArrow)`) for quick prototyping.
- For production-readiness, use the **New Unity Input System** package. This provides cross-platform support (gamepad, touch, keyboard) and allows for easy remapping of controls.

## 3. Time and Frame Rate Management

Currently, the game relies on `System.Diagnostics.Stopwatch` to track elapsed time and compute delta time (`dt`).

**Unity Replacement:**
- Replace manual stopwatch calculations with `Time.deltaTime` inside the `Update()` loop.
- Use `Time.time` for tracking overall elapsed time.
- Avoid `Thread.Sleep(33)` to control frame rates. Unity manages the frame rate automatically. You can set the target frame rate via `Application.targetFrameRate = 60;`.

## 4. Rendering and UI

The current game draws characters to the console grid using `Console.SetCursorPosition`, `Console.ForegroundColor`, and `Console.BackgroundColor`.

**Unity Replacement:**
- **2D Grid:** Represent the 40x20 grid using Unity's **Tilemap** system or a grid of Sprite Renderers. The player, spotlight, and empty spaces can be distinct sprites or tiles.
- **UI:** The text for Lives, Time in Spotlight, and Time Out should be rendered using **TextMeshPro (TMP)** on a Unity Canvas for crisp, scalable text.
- **Visuals:** Replace the `P` character and background colors with actual 2D sprites (e.g., a character sprite for the player, a semi-transparent yellow sprite or 2D Light for the spotlight).
- **Lighting (Optional but Recommended):** Utilize Unity's **Universal Render Pipeline (URP)** with 2D Lights. The spotlight can be a `Light2D` component to create a genuine visual spotlight effect.

## 5. Production Readiness Checklist

To make the game production-ready in Unity, address the following areas:

### A. Asset Management & Object Pooling
Instead of instantiating and destroying objects frequently, use **Object Pooling** if dynamically generating entities. While `CrowdPleaser` has a fixed grid, keep this in mind for future scaling (like adding moving audience members).

### B. Resolution Independence
Ensure the Canvas UI scales correctly across different screen resolutions and aspect ratios by configuring the `CanvasScaler` component to "Scale With Screen Size". Adjust the Orthographic size of the Main Camera to fit the 40x20 grid on any screen.

### C. Audio
Add audio feedback. The console game is silent. Add an `AudioSource` to play sounds for:
- Winning/Losing
- Moving the player
- Being caught in the spotlight vs. being in the dark

### D. State Management
Use a `GameManager` (Singleton or ScriptableObject based) to handle the different game states:
- Main Menu
- Playing
- Game Over
- Victory

### E. Code Quality and Testing
- Keep the pure logic C# classes independent of `MonoBehaviour` where possible to allow for fast unit testing via the **Unity Test Framework**.
- Write PlayMode and EditMode tests.

### F. Building and Deployment
- Set up **Build Profiles** in Unity for target platforms (PC, WebGL, Mobile, Console).
- Ensure player settings are configured with icons, splash screens, and correct resolution handling.

## Example Unity Script Stub

```csharp
using UnityEngine;
using TMPro;

public class CrowdPleaserManager : MonoBehaviour
{
    public Transform player;
    public Transform spotlight;
    public TextMeshProUGUI statsText;

    private float timeInSpotlight;
    private float timeOutOfSpotlight;
    private int lives = 5;

    void Update()
    {
        HandleInput();
        UpdateSpotlight();
        CheckWinLoss();
        UpdateUI();
    }

    void HandleInput()
    {
        // Example input handling
        Vector3 move = Vector3.zero;
        if (Input.GetKeyDown(KeyCode.UpArrow)) move.y += 1;
        if (Input.GetKeyDown(KeyCode.DownArrow)) move.y -= 1;
        if (Input.GetKeyDown(KeyCode.LeftArrow)) move.x -= 1;
        if (Input.GetKeyDown(KeyCode.RightArrow)) move.x += 1;
        player.position += move;
    }

    // Additional logic...
}
```
