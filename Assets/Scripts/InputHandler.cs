using UnityEngine;

public class InputHandler
{
    private readonly GameManager gameManager;

    public InputHandler(GameManager manager)
    {
        gameManager = manager;
    }

    public void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.N) || Input.GetKeyDown(KeyCode.R) || Input.GetKeyDown(KeyCode.Space))
        {
            gameManager.InitializeGame();
            return;
        }
        
        if (Input.GetMouseButtonDown(0)) {
            gameManager.RevealCell();
        } else if (Input.GetMouseButtonDown(1)) {
            gameManager.FlagToggle();
        }
    }
    
}