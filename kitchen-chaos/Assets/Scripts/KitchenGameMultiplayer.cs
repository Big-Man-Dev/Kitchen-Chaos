using UnityEngine;
using Unity.Netcode;

public class KitchenGameMultiplayer : NetworkBehaviour
{
    [SerializeField] private KitchenObjectListSO kitchenObjectListSO;
    public static KitchenGameMultiplayer Instance { get; private set; }
    private void Awake() {
        Instance = this;
    }
    public void SpawnKitchenObject(KitchenObjectSO kitchenObjectSO, IKitchenObjectParent kitchenObjectParent) {
        SpawnKitchenObjectServerRpc(GetKitchenObjectSOIndex(kitchenObjectSO), kitchenObjectParent.GetNetworkObject());
    }
    [ServerRpc(RequireOwnership = false)]
    private void SpawnKitchenObjectServerRpc(int kitchenObjectSOIndex, NetworkObjectReference kitchenObjectParentNetworkReference) {
        KitchenObjectSO kitchenObjectSO = GetKitchenObjectFromIndex(kitchenObjectSOIndex);
        GameObject kitchenObjectGameobject = Instantiate(kitchenObjectSO.prefab);
        kitchenObjectGameobject.transform.position = new Vector3(2, 2, 2);
        NetworkObject kitchenObjectNetworkObject = kitchenObjectGameobject.GetComponent<NetworkObject>();
        kitchenObjectNetworkObject.Spawn(true);
        KitchenObject kitchenObject = kitchenObjectGameobject.GetComponent<KitchenObject>();
        kitchenObjectParentNetworkReference.TryGet(out NetworkObject kitchenObjectParentNetworkObject);
        kitchenObject.SetKitchenObjectParent(kitchenObjectParentNetworkObject.GetComponent<IKitchenObjectParent>());
    }
    public int GetKitchenObjectSOIndex(KitchenObjectSO kitchenObjectSO) {
        return kitchenObjectListSO.kitchenObjectSOList.IndexOf(kitchenObjectSO);
    }
    public KitchenObjectSO GetKitchenObjectFromIndex(int index) {
        return kitchenObjectListSO.kitchenObjectSOList[index];
    }

    public void DestroyKitchenObject(KitchenObject kitchenObject) {
        DestroyKitchenObjectServerRpc(kitchenObject.NetworkObject);
    }
    [ServerRpc(RequireOwnership = false)]
    private void DestroyKitchenObjectServerRpc(NetworkObjectReference networkObjectReference) {
        networkObjectReference.TryGet(out NetworkObject networkObject);
        KitchenObject kitchenObject = networkObject.GetComponent<KitchenObject>();
        ClearKitchenObjectOnParentClientRpc(networkObjectReference);
        kitchenObject.DestroySelf();
    }
    [ClientRpc]
    private void ClearKitchenObjectOnParentClientRpc(NetworkObjectReference networkObjectReference) {
        networkObjectReference.TryGet(out NetworkObject networkObject);
        KitchenObject kitchenObject = networkObject.GetComponent<KitchenObject>();
        kitchenObject.ClearKitchenObjectOnParent();
    }
}
