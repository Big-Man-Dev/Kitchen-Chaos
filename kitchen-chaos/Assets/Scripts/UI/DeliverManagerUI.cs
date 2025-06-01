using UnityEngine;

public class DeliverManagerUI : MonoBehaviour
{
    [SerializeField] private GameObject container;
    [SerializeField] private GameObject recipeTemplate;
    [SerializeField] private GameObject titleText;

    private void Awake() {
        recipeTemplate.gameObject.SetActive(false);
    }
    private void Start() {
        UpdateVisual();
        DeliveryManager.Instance.OnRecipeSpawned += DeliverManager_OnRecipeSpawned;
        DeliveryManager.Instance.OnRecipeDeleted += DeliverManager_OnRecipeDeleted;
    }

    private void DeliverManager_OnRecipeDeleted() {
        UpdateVisual();
    }

    private void DeliverManager_OnRecipeSpawned() {
        UpdateVisual();
    }

    private void UpdateVisual() {
        foreach (Transform transformChild in container.transform) {
            if (transformChild == recipeTemplate.transform) continue;
            if (transformChild == titleText.transform) continue;
            Destroy(transformChild.gameObject);
        }
        foreach (RecipeSO recipeSO in DeliveryManager.Instance.GetWaitingRecipeSOList()) {
            GameObject recipeObject = Instantiate(recipeTemplate, container.transform);
            recipeObject.GetComponent<DeliverManagerSinlgeUI>().SetRecipeSO(recipeSO);
            recipeObject.SetActive(true);
        }
    }
}
