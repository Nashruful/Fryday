using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int ingredientsToWin = 6;
    private int collectedIngredients = 0;


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
        Time.timeScale = 0; 
    }

    public void LoseGame()
    {
        Debug.Log("You Lose!");
        Time.timeScale = 0; 
    }
}