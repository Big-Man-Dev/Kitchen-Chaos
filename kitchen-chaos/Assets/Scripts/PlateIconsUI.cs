using UnityEngine;

public class PlateIconsUI : MonoBehaviour
{
    [SerializeField] private PlateKitchenObject plateKitchenObject;
    [SerializeField] private GameObject iconTemplate;

    private void Awake() {
        iconTemplate.SetActive(false);
    }
    private void Start() {
        plateKitchenObject.OnIngredientAdded += PlateKitchenObject_OnIngredientAdded;
    }

    private void PlateKitchenObject_OnIngredientAdded(object sender, PlateKitchenObject.OnIngredientAddedEventArgs e) {
        UpdateVisual();
    }

    private void UpdateVisual() {
        foreach(Transform child in transform) {
            if (child.gameObject == iconTemplate) continue;
            Destroy(child.gameObject);
        }

        foreach (KitchenObjectSO kitchenObjectSO in plateKitchenObject.GetKitchenObjectSOList()) {
            GameObject newIcon = Instantiate(iconTemplate, transform);
            newIcon.GetComponent<PlateIconSingleUI>().SetKitchenObjectSO(kitchenObjectSO);
            newIcon.SetActive(true);
        }
    }
}
