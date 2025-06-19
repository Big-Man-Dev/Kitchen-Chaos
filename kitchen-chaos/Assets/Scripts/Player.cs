using System;
using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;

public class Player : NetworkBehaviour, IKitchenObjectParent {

    public static Player LocalInstance { get; private set; }

    public event EventHandler OnPickedSomething;

    public event EventHandler<OnSelectedCounterChangeEventArgs> OnSelectedCounterChange;
    public class OnSelectedCounterChangeEventArgs : EventArgs {
        public BaseCounter selectedCounter;
    }
    public static event EventHandler OnAnyPlayerSpawn;
    public static event EventHandler OnAnyPickedSomething;
    public static void ResetStaticData() {
        OnAnyPlayerSpawn = null;
        OnAnyPickedSomething = null;
    }

    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotateSpeed = 10f;
    [SerializeField] private float playerSize = 0.6f;
    [SerializeField] private float playerHeight = 2f;
    [SerializeField] private float interactDistance = 2f;
    [SerializeField] private LayerMask countersLayerMask;
    [SerializeField] private LayerMask collisionsLayerMask;
    [SerializeField] private Transform kitchenObjectHoldPoint;
    [SerializeField] private List<Vector3> spawnPositionList = new();  

    private KitchenObject kitchenObject;
    private BaseCounter selectedCounter;
    Vector2 inputVector = Vector2.zero;
    Vector3 moveDir = Vector3.zero;
    Vector3 lastInteractDirection = Vector3.zero;
    bool isWalking = false;
    bool canMove = false;
    float moveDistance;
    private void Start() {
        GameInput.Instance.OnInteractAction += GameInput_OnInteractAction;
        GameInput.Instance.OnInteractAlternateAction += GameInput_OnInteractAlternateAction;
    }
    public override void OnNetworkSpawn() {
        if (IsOwner) LocalInstance = this;
        transform.position = spawnPositionList[(int)OwnerClientId];
        OnAnyPlayerSpawn?.Invoke(this, EventArgs.Empty);
        if (IsServer) {
            NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
        }
    }

    private void NetworkManager_OnClientDisconnectCallback(ulong clientId) {
        if(clientId == OwnerClientId && HasKitchenObject()) {
            KitchenObject.DestroyKitchenObject(GetKitchenObject());
        }
    }

    private void GameInput_OnInteractAlternateAction(object sender, EventArgs e) {
        if (GameManager.Instance.isGamePlaying() == false) return;
        if (selectedCounter != null) {
            selectedCounter.InteractAlternate(this);
        }
    }

    private void GameInput_OnInteractAction(object sender, System.EventArgs e) {
        if (GameManager.Instance.isGamePlaying() == false) return;
        if (selectedCounter != null) {
            selectedCounter.Interact(this);
        }
    }

    public void Update() {
        if (IsOwner == false) {
            return;
        }
        HandleMovement();
        HandleInteractions();
    }

    public bool IsWalking() => isWalking; 
    private void HandleInteractions() {
        inputVector = GameInput.Instance.GetMovementVectorNormalized();
        moveDir = new Vector3(inputVector.x, 0, inputVector.y);
        if (moveDir != Vector3.zero) lastInteractDirection = moveDir;

        if (Physics.Raycast(transform.position, lastInteractDirection, out RaycastHit raycastHit, interactDistance, countersLayerMask)) {
            if (raycastHit.transform.TryGetComponent(out BaseCounter baseCounter)) {
                if (baseCounter != selectedCounter) SetSelectedCounter(baseCounter);
            } else SetSelectedCounter(null);
        } else SetSelectedCounter(null);
    }
    private void HandleMovement() {
        inputVector = GameInput.Instance.GetMovementVectorNormalized();
        moveDir = new Vector3(inputVector.x, 0, inputVector.y);
        moveDistance = moveSpeed * Time.deltaTime;
        canMove = !Physics.BoxCast(transform.position, Vector3.one * playerSize, moveDir, Quaternion.identity, moveDistance, collisionsLayerMask);

        if (canMove == false) {
            Vector3 moveDirX = new Vector3(moveDir.x, 0, 0).normalized;
            canMove = (moveDir.x < -0.5f || moveDir.x > +0.5f) && !Physics.BoxCast(transform.position, Vector3.one * playerSize, moveDirX, Quaternion.identity, moveDistance, collisionsLayerMask);

            if (canMove) moveDir = moveDirX;
            else {
                Vector3 moveDirZ = new Vector3(0, 0, moveDir.z).normalized;
                canMove = (moveDir.z < -0.5f || moveDir.z > +0.5f) && !Physics.BoxCast(transform.position, Vector3.one * playerSize, moveDirZ, Quaternion.identity, moveDistance, collisionsLayerMask);
                if (canMove) moveDir = moveDirZ;
            }
        }

        if (canMove) transform.position += moveDir * moveDistance;
        transform.forward = Vector3.Slerp(transform.forward, moveDir, rotateSpeed * Time.deltaTime);

        if (moveDir != Vector3.zero) isWalking = true;
        else isWalking = false;
    }

    private void SetSelectedCounter(BaseCounter selectedCounter) {
        this.selectedCounter = selectedCounter;
        OnSelectedCounterChange?.Invoke(this, new OnSelectedCounterChangeEventArgs { selectedCounter = selectedCounter });
    }

    public Transform GetKitchenObjectFollowTransform() => kitchenObjectHoldPoint;
    public KitchenObject GetKitchenObject() => kitchenObject;
    public void SetKitchenObject(KitchenObject kitchenObject) {
        this.kitchenObject = kitchenObject;
        if (kitchenObject != null) {
            OnPickedSomething?.Invoke(this, EventArgs.Empty);
            OnAnyPickedSomething?.Invoke(this, EventArgs.Empty);
        }
    }
    public void ClearKitchenObject() => kitchenObject = null;
    public bool HasKitchenObject() => kitchenObject != null;
    public NetworkObject GetNetworkObject() {
        return NetworkObject;
    }
}
