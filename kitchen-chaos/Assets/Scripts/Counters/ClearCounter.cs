using UnityEngine;

public class ClearCounter : BaseCounter
{
    [SerializeField] private KitchenObjectSO kitchenObjectSO;
    public override void Interact(Player player) {
        if (HasKitchenObject() == false) {
            if (player.HasKitchenObject()) {
                player.GetKitchenObject().SetKitchenObjectParent(this);
            }
        }else {
            if (player.HasKitchenObject() == false) GetKitchenObject().SetKitchenObjectParent(player);
        }
    }

}
