using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int ingredientsToWin = 6;
    private int collectedIngredients = 0;

    // The Update method and the reference to PanEscape are no longer needed here.

    public void CollectIngredient()
    {
        collectedIngredients++;
        Debug.Log("Ingredients collected: " + collectedIngredients);
        if (collectedIngredients >= ingredientsToWin)
        {
            WinGame();
        }
    }

    void WinGame()
    {
        Debug.Log("You Win!");
        // Add win screen logic here
        Time.timeScale = 0; // A simple way to pause the game
    }

    // This method is now public so it can be called by PanEscape
    public void LoseGame()
    {
        Debug.Log("You Lose!");
        // Add lose screen logic here
        Time.timeScale = 0; // A simple way to pause the game
    }
}