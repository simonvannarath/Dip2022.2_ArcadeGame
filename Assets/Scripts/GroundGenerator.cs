using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GroundGenerator : MonoBehaviour
{
    public Camera mainCamera;
    public Transform startPoint; // Ground tiles will start from this point
    public PlatformTile tilePrefab; // Reference to the tile prefab
    public float moveSpeed = 12.0f;
    public int tilesToPreSpawn = 15; // Number of tiles to pre-spawn
    public int tilesNoObstacles = 3; // Number of clear tiles at start of run

    readonly List<PlatformTile> spawnedTiles = new List<PlatformTile>();

    [HideInInspector]
    public bool gameOver = false;
    public bool gameStarted = false;
    float score = 0;

    public static GroundGenerator instance;

    private void Start()
    {
        instance = this;

        Vector3 spawnPos = startPoint.position;
        int tilesNoObstaclesTmp = tilesNoObstacles;
        for (int i = 0; i < tilesToPreSpawn; i++)
        {
            spawnPos -= tilePrefab.startPoint.localPosition;
            PlatformTile spawnedTile = Instantiate(tilePrefab, spawnPos, Quaternion.identity) as PlatformTile;

            if (tilesNoObstaclesTmp > 0)
            {
                spawnedTile.DeactivateAllObstacles();
                tilesNoObstaclesTmp--;
            }

            else
            {
                spawnedTile.ActivateRandomObstacle();
            }

            spawnPos = spawnedTile.endPoint.position;
            spawnedTile.transform.SetParent(transform);
            spawnedTiles.Add(spawnedTile);
        }
    }

    private void Update()
    {
        /* Move the object upwards in world space by units per second
         * Increase speed progressively as points increase */

        if (!gameOver && gameStarted)
        {
            transform.Translate(-spawnedTiles[0].transform.forward * Time.deltaTime * (moveSpeed + (score / 500)), Space.World);
            score += Time.deltaTime * moveSpeed;
        }

        if (mainCamera.WorldToViewportPoint(spawnedTiles[0].endPoint.position).z < 0)
        {
            // Move the tile to the front if it's behind the camera
            PlatformTile tileTmp = spawnedTiles[0];
            spawnedTiles.RemoveAt(0);
            tileTmp.transform.position = spawnedTiles[spawnedTiles.Count - 1].endPoint.position - tileTmp.startPoint.localPosition;
            tileTmp.ActivateRandomObstacle();
            spawnedTiles.Add(tileTmp);
        }

        if (gameOver || !gameStarted)
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.F))
            {
                if (gameOver)
                {
                    // Restart the current scene
                    Scene scene = SceneManager.GetActiveScene();
                    SceneManager.LoadScene(scene.name);
                }

                else
                {
                    // Start the game
                    gameStarted = true;
                    Time.timeScale = 1f;
                }
            }

            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Alpha1))
            {
                if (gameOver) SceneManager.LoadScene("00_TitleScene");
            }

        }
        
        // Quick exit to title
        if (Input.GetKeyDown(KeyCode.F12) || (Input.GetKeyDown(KeyCode.Y) && (Input.GetKeyDown(KeyCode.H))))
            {
                SceneManager.LoadScene("00_TitleScene");
            }

    }

    // Initial interface implementation; replace with nicer looking UIToolkit-based GUI in future builds
    private void OnGUI()
    {
        if (gameOver)
        {
            Time.timeScale = 0;
            
            GUI.color = Color.red;
            GUI.Label(new Rect(Screen.width / 2 - 50, Screen.height / 2, 300, 300), "Game Over\nYour Score is: " + ((int)score) + "\nPress B1/SPACE to restart" + "\nPress START/ESC to return to title");
        }

        else 
        {
            if (!gameStarted)
            {
                GUI.color = Color.red;
                GUI.Label(new Rect(Screen.width / 2 - 100, Screen.height / 2, 200, 200), "Press B1/SPACE to start");
            }
        }

        GUI.color = Color.green;
        GUI.Label(new Rect(5, 5, 200, 25), "Score: " + ((int)score));
    }
}
