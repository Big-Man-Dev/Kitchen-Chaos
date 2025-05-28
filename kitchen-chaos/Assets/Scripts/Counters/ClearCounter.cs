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
                        GetKitchenObject().DestroySelf();
                    }
                } else if (GetKitchenObject().TryGetPlate(out plateKitchenObject)) {
                    if (plateKitchenObject.TryAddIngredient(player.GetKitchenObject().GetKitchenObjectSO())) {
                        player.GetKitchenObject().DestroySelf();
                    }
                }
            }
        }
    }

}
