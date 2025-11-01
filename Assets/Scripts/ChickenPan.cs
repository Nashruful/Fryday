using UnityEngine;

public class ChickenPan : MonoBehaviour
{
    public GameManager gameManager; 

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            IngredientPickup playerInventory = other.GetComponent<IngredientPickup>();

            if (playerInventory != null && playerInventory.heldIngredient != null)
            {
                GameObject ingredient = playerInventory.DropAndGiveIngredient();
                Sound.Instance.PlayDropItemSound(); // Play drop sound
                gameManager.CollectIngredient();

                Destroy(ingredient);
            }
        }
    }
}