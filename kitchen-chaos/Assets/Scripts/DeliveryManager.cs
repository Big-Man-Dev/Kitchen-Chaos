using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class DeliveryManager : NetworkBehaviour
{
    public event Action OnRecipeSpawned;
    public event Action OnRecipeDeleted;
    public event Action OnRecipeSuccess;
    public event Action OnRecipeFailed;
    public static DeliveryManager Instance { get; private set; }
    [SerializeField] private RecipeListSO recipeListSO;
    private List<RecipeSO> waitingRecipeSOList = new();

    private float spawnRecipeTimer = 0f;
    private float spawnRecipeTimerMax = 4f;
    private int waitingRecipeMax = 4;
    private int recipeDeliverCount = 0;

    private void Awake() {
        Instance = this;
    }
    private void Update() {
        if (IsServer == false) return;
        if (GameManager.Instance.isGamePlaying() == false) return;
        spawnRecipeTimer -= Time.deltaTime;
        if (spawnRecipeTimer <= 0f) {
            spawnRecipeTimer = spawnRecipeTimerMax;
            if (waitingRecipeSOList.Count < waitingRecipeMax) {
                int waitingRecipeSOIndex = UnityEngine.Random.Range(0, recipeListSO.recipeSOList.Count);
                RecipeSO waitingRecipeSO = recipeListSO.recipeSOList[UnityEngine.Random.Range(0, recipeListSO.recipeSOList.Count)];
                SpawnNewWaitingRecipeClientRpc(waitingRecipeSOIndex);
            }
        }
    }

    [ClientRpc]
    private void SpawnNewWaitingRecipeClientRpc(int waitingRecipeSOIndex) {
        RecipeSO waitingRecipeSO = recipeListSO.recipeSOList[waitingRecipeSOIndex];
        waitingRecipeSOList.Add(waitingRecipeSO);
        OnRecipeSpawned?.Invoke();
    }

    public void DeliverRecipe(PlateKitchenObject plateKitchenObject) {
        if(plateKitchenObject.GetKitchenObjectSOList().Count == 0) {
            DeliverIncorrectRecipeServerRpc();
            return;
        }
        for (int i = 0; i < recipeListSO.recipeSOList.Count; i++) {
            RecipeSO waitingRecipeSO = waitingRecipeSOList[i];
            Debug.Log(i);
            if(waitingRecipeSO.kitchenObjectSOList.Count == plateKitchenObject.GetKitchenObjectSOList().Count) {
                bool plateContentsMatchesRecipe = true;
                foreach(KitchenObjectSO recipeKitchenObjectSO in waitingRecipeSO.kitchenObjectSOList) {
                    bool ingredientFound = false;
                    foreach (KitchenObjectSO plateKitchenObjectSO in plateKitchenObject.GetKitchenObjectSOList()) {
                        if(plateKitchenObjectSO == recipeKitchenObjectSO) {
                            ingredientFound = true;
                            break;
                        }
                    }
                    if(ingredientFound == false) {
                        plateContentsMatchesRecipe = false;
                    }
                }
                if (plateContentsMatchesRecipe) {
                    DeliverCorrectRecipeServerRpc(i);
                    return;
                }
            }
        }
        DeliverIncorrectRecipeServerRpc();
        return;
    }

    [ServerRpc(RequireOwnership = false)]
    private void DeliverIncorrectRecipeServerRpc() {
        DeliverIncorrectRecipeClientRpc();
    }
    [ClientRpc]
    private void DeliverIncorrectRecipeClientRpc() {
        OnRecipeFailed?.Invoke();
    }

    [ServerRpc(RequireOwnership = false)]
    private void DeliverCorrectRecipeServerRpc(int index) {
        DeliverCorrectRecipeClientRpc(index);
    }

    [ClientRpc]
    private void DeliverCorrectRecipeClientRpc(int index) {
        waitingRecipeSOList.RemoveAt(index);
        OnRecipeDeleted?.Invoke();
        OnRecipeSuccess?.Invoke();
        recipeDeliverCount++;
    }

    public List<RecipeSO> GetWaitingRecipeSOList() => waitingRecipeSOList;
    public int GetRecipeDeliveredCount() => recipeDeliverCount;
}
