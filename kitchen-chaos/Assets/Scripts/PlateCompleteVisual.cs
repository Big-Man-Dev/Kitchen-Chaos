using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PlateCompleteVisual : MonoBehaviour
{
    [Serializable]
    public struct KitckenObjectSO_GameObject {
        public KitchenObjectSO kitchenObjectSO;
        public GameObject gameObject;
    }

    [SerializeField] private PlateKitchenObject plateKitchenObject;
    [SerializeField] private List<KitckenObjectSO_GameObject> kitchenObjectSOGameObjectList = new();

    private void Start() {
        plateKitchenObject.OnIngredientAdded += PlateKitchenObject_OnIngredientAdded;
        foreach (KitckenObjectSO_GameObject kitckenObjectSO_GameObject in kitchenObjectSOGameObjectList) {
            kitckenObjectSO_GameObject.gameObject.SetActive(false);
        }
    }

    private void PlateKitchenObject_OnIngredientAdded(object sender, PlateKitchenObject.OnIngredientAddedEventArgs e) {
        foreach (KitckenObjectSO_GameObject kitckenObjectSO_GameObject in kitchenObjectSOGameObjectList) {
            if (kitckenObjectSO_GameObject.kitchenObjectSO == e.kitchenObjectSO) {
                kitckenObjectSO_GameObject.gameObject.SetActive(true);
            }
        }
    }
}
