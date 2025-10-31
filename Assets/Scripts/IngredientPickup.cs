using UnityEngine;

public class IngredientPickup : MonoBehaviour
{
    public GameObject heldIngredient = null;
    public Transform holdPosition; // Assign this in the Inspector

    void Update()
    {
        // We only check for pickup if the player is NOT holding anything.
        if (heldIngredient == null)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                // Find the closest ingredient within a small radius.
                float pickupRadius = 2f;
                Collider[] colliders = Physics.OverlapSphere(transform.position, pickupRadius);
                float minSqrDistance = float.MaxValue;
                GameObject closestIngredient = null;

                foreach (Collider col in colliders)
                {
                    if (col.CompareTag("Ingredient"))
                    {
                        float sqrDistance = (transform.position - col.transform.position).sqrMagnitude;
                        if (sqrDistance < minSqrDistance)
                        {
                            minSqrDistance = sqrDistance;
                            closestIngredient = col.gameObject;
                        }
                    }
                }

                if (closestIngredient != null)
                {
                    PickUp(closestIngredient);
                }
            }
        }
    }

    void PickUp(GameObject ingredient)
    {
        heldIngredient = ingredient;
        // Make the ingredient a child of the holdPosition, so it moves with the player.
        ingredient.transform.SetParent(holdPosition);
        ingredient.transform.localPosition = Vector3.zero;
        // Disable its collider so it doesn't bump into things.
        ingredient.GetComponent<Collider>().enabled = false;
    }

    // This method will be called by the pan to take the ingredient.
    public GameObject DropAndGiveIngredient()
    {
        GameObject ingredientToGive = heldIngredient;
        heldIngredient = null; // Player is no longer holding anything.
        return ingredientToGive;
    }
}