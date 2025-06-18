using UnityEngine;

public class ClearCounter : BaseCounter
{
    public override void Interact(Player player) {
        if (HasKitchenObject() == false) {
            if (player.HasKitchenObject()) {
                player.GetKitchenObject().SetKitchenObjectParent(this);
            }
        }else {
            if (player.HasKitchenObject() == false) {
                GetKitchenObject().SetKitchenObjectParent(player);
            }else {
                if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject)) {
                    if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO())) {
                        KitchenObject.DestroyKitchenObject(GetKitchenObject());
                    }
                } else if (GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObjectCounter)) {
                    if (plateKitchenObjectCounter.TryAddIngredient(player.GetKitchenObject().GetKitchenObjectSO())) {
                        KitchenObject.DestroyKitchenObject(player.GetKitchenObject());
                    }
                }
            }
        }
    }

}
