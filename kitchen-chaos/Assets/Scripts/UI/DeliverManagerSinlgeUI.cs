using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeliverManagerSinlgeUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI recipeNameText;
    [SerializeField] private GameObject iconContainer;
    [SerializeField] private GameObject iconTemplate;
    private void Awake() {
        iconTemplate.SetActive(false);
    }
    public void SetRecipeSO(RecipeSO recipeSO) {
        recipeNameText.text = recipeSO.recipeName;

        foreach (Transform child in iconContainer.transform) {
            if (child == iconTemplate.transform) continue;
            Destroy(child.gameObject);
        }

        foreach (KitchenObjectSO kitchenObjectSO in recipeSO.kitchenObjectSOList) {
            GameObject iconObject = Instantiate(iconTemplate, iconContainer.transform);
            iconObject.GetComponent<Image>().sprite = kitchenObjectSO.iconSprite;
            iconObject.SetActive(true);
        }
    }
}
