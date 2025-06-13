using UnityEngine;
using Unity.Netcode;
public class KitchenObject : NetworkBehaviour
{
    [SerializeField] private KitchenObjectSO kitchenObjectSO;
    private IKitchenObjectParent kitchenObjectParent;
    private FollowTransform followTransform;
    public KitchenObjectSO GetKitchenObjectSO() => kitchenObjectSO;

    protected virtual void Awake() {
        followTransform = GetComponent<FollowTransform>();
    }
    public void SetKitchenObjectParent(IKitchenObjectParent kitchenObjectParent) {
        SetKitchenObjectParentServerRpc(kitchenObjectParent.GetNetworkObject());
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetKitchenObjectParentServerRpc(NetworkObjectReference networkObjectReference) {
        SetKitchenObjectParentClientRpc(networkObjectReference);
    }
    [ClientRpc]
    private void SetKitchenObjectParentClientRpc(NetworkObjectReference networkObjectReference) {
        networkObjectReference.TryGet(out NetworkObject networkObject);
        IKitchenObjectParent kitchenObjectParent = networkObject.GetComponent<IKitchenObjectParent>();
        if (this.kitchenObjectParent != null) {
            this.kitchenObjectParent.ClearKitchenObject();
        }
        this.kitchenObjectParent = kitchenObjectParent;

        if (kitchenObjectParent.HasKitchenObject()) {
            Debug.LogError("kitchenObjectParent already has a kitchen object");
        }

        kitchenObjectParent.SetKitchenObject(this);
        followTransform.SetTargetTransform(kitchenObjectParent.GetKitchenObjectFollowTransform());
    }
    public IKitchenObjectParent GetKitchenObjectParent() => kitchenObjectParent;

    public void DestroySelf() {
        kitchenObjectParent.ClearKitchenObject();
        Destroy(gameObject);
    }

    public void ClearKitchenObjectOnParent() {
        kitchenObjectParent.ClearKitchenObject();
    }

    public bool TryGetPlate(out PlateKitchenObject plateKitchenObject) {
        if(this is PlateKitchenObject) {
            plateKitchenObject = this as PlateKitchenObject;
            return true;
        }
        plateKitchenObject = null;
        return false;
    }

    public static void SpawnKitchenObject(KitchenObjectSO kitchenObjectSO, IKitchenObjectParent kitchenObjectParent) {
        KitchenGameMultiplayer.Instance.SpawnKitchenObject(kitchenObjectSO, kitchenObjectParent);
    }

    public static void DestroyKitchenObject(KitchenObject kitchenObject) {
        KitchenGameMultiplayer.Instance.DestroyKitchenObject(kitchenObject); 
    }
}
