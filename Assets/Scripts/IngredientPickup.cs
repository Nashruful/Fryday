using UnityEngine;

public class IngredientPickup : MonoBehaviour
{
    public GameObject heldIngredient = null;
    public Transform holdPosition;

    void Update()
    {
        if (heldIngredient == null)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
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
        ingredient.transform.SetParent(holdPosition);
        ingredient.transform.localPosition = Vector3.zero;
        ingredient.GetComponent<Collider>().enabled = false;
    }

    public GameObject DropAndGiveIngredient()
    {
        GameObject ingredientToGive = heldIngredient;
        heldIngredient = null; 
        return ingredientToGive;
    }
}