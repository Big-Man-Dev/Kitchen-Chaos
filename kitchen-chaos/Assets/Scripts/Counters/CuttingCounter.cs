using System;
using UnityEngine;
using UnityEngine.Windows;
using static IHasProgress;

public class CuttingCounter : BaseCounter, IHasProgress
{
    [SerializeField] private CuttingRecipeSO[] cuttingRecipeSOs;
    public event EventHandler<OnProgressChangeEventArgs> OnProgressChange;
    public event EventHandler OnCut;
    public static event EventHandler OnAnyCut;

    new public static void ResetStaticData() {
        OnAnyCut = null; 
    }

    private int cuttingProgress;
    public override void Interact(Player player) {
        if (HasKitchenObject() == false) {
            if (player.HasKitchenObject() && HasRecipeWithInput(player.GetKitchenObject().GetKitchenObjectSO())) {
                player.GetKitchenObject().SetKitchenObjectParent(this);
                cuttingProgress = 0;
                CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());
                OnProgressChange?.Invoke(this, new OnProgressChangeEventArgs {
                    progressNormalized = (float)cuttingProgress / cuttingRecipeSO.cuttingProgressMax
                });
            }
        } else if (player.HasKitchenObject() == false) {
            OnProgressChange?.Invoke(this, new OnProgressChangeEventArgs {
                progressNormalized = 0
            });
            GetKitchenObject().SetKitchenObjectParent(player);
        } else if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject)) {
            if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO())) {
                GetKitchenObject().DestroySelf();
            }
        }
    }
    public override void InteractAlternate(Player player) {
        if (HasKitchenObject() && HasRecipeWithInput(GetKitchenObject().GetKitchenObjectSO())) {
            cuttingProgress++;
            OnCut?.Invoke(this, EventArgs.Empty);
            OnAnyCut?.Invoke(this, EventArgs.Empty);
            CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());

            OnProgressChange?.Invoke(this, new OnProgressChangeEventArgs {
                progressNormalized = (float)cuttingProgress / cuttingRecipeSO.cuttingProgressMax
            });

            if (cuttingProgress >= cuttingRecipeSO.cuttingProgressMax) {
                KitchenObjectSO outputKitchenObjectSO = GetOutputForInput(GetKitchenObject().GetKitchenObjectSO());
                if (outputKitchenObjectSO != null) {
                    GetKitchenObject().DestroySelf();
                    KitchenObject.SpawnKitchenObject(outputKitchenObjectSO, this);
                } else return;
            } else {

            }
        }
    }
    private bool HasRecipeWithInput(KitchenObjectSO input) {
        CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOWithInput(input);
        if (cuttingRecipeSO != null) return true;
        else return false;
    }
    private KitchenObjectSO GetOutputForInput(KitchenObjectSO input) {
        CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOWithInput(input);
        if(cuttingRecipeSO != null) return cuttingRecipeSO.output;
        else return null;
    }
    private CuttingRecipeSO GetCuttingRecipeSOWithInput(KitchenObjectSO input) {
        for (int i = 0; i < cuttingRecipeSOs.Length; i++) {
            if (cuttingRecipeSOs[i].input == input) return cuttingRecipeSOs[i];
        }
        return null;
    }
}
