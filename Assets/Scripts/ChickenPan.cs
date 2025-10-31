using UnityEngine;

public class ChickenPan : MonoBehaviour
{
    public GameManager gameManager; // Assign in Inspector

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object that entered is the Player.
        if (other.CompareTag("Player"))
        {
            // Get the IngredientPickup script from the player.
            IngredientPickup playerInventory = other.GetComponent<IngredientPickup>();

            // Check if the player is actually holding an ingredient.
            if (playerInventory != null && playerInventory.heldIngredient != null)
            {
                // Take the ingredient from the player.
                GameObject ingredient = playerInventory.DropAndGiveIngredient();

                // Tell the GameManager we've collected one.
                gameManager.CollectIngredient();

                // The ingredient has served its purpose, so we destroy it.
                Destroy(ingredient);
            }
        }
    }
}