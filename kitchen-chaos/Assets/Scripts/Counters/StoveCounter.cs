using System;
using Unity.Netcode;
using UnityEngine;
using static CuttingCounter;

public class StoveCounter : BaseCounter, IHasProgress
{
    public event EventHandler<OnStateChangedEventArgs> OnStateChanged;
    public event EventHandler<IHasProgress.OnProgressChangeEventArgs> OnProgressChange;

    public class OnStateChangedEventArgs : EventArgs {
        public State state;
    }
    public enum State
    {
        Idle,
        Frying,
        Fried,
        Burned,
    }

    [SerializeField] private FryingRecipeSO[] fryingRecipeSOArray;
    [SerializeField] private BurningRecipeSO[] burningRecipeSOArray;

    private NetworkVariable<State> state = new NetworkVariable<State>(State.Idle);
    private FryingRecipeSO fryingRecipeSO;
    private BurningRecipeSO burningRecipeSO;
    private NetworkVariable<float> fryingTimer = new NetworkVariable<float>(0f);
    private NetworkVariable<float> burningTimer = new NetworkVariable<float>(0f);

    public override void OnNetworkSpawn() {
        fryingTimer.OnValueChanged += FryingTimer_OnValueChanged;
        burningTimer.OnValueChanged += BurningTimer_OnValueChanged;
        state.OnValueChanged += State_OnValueChanged;
    }
    private void FryingTimer_OnValueChanged(float previousValue, float newValue) {
        float fryingTimerMax = fryingRecipeSO != null ? fryingRecipeSO.fryingTimerMax : 1f;
        OnProgressChange?.Invoke(this, new IHasProgress.OnProgressChangeEventArgs {
            progressNormalized = fryingTimer.Value / fryingTimerMax
        });
    }
    private void BurningTimer_OnValueChanged(float previousValue, float newValue) {
        float burningTimerMax = burningRecipeSO != null ? burningRecipeSO.burningTimerMax : 1f;
        OnProgressChange?.Invoke(this, new IHasProgress.OnProgressChangeEventArgs {
            progressNormalized = burningTimer.Value / burningTimerMax
        });
    }
    private void State_OnValueChanged(State previousState, State newState) {
        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs { 
            state = state.Value 
        });
        if(state.Value == State.Burned || state.Value == State.Idle) {
            OnProgressChange?.Invoke(this, new IHasProgress.OnProgressChangeEventArgs {
                progressNormalized = 0f
            });
        }
    }

    private void Update()
    {
        if (IsServer == false) return;
        if (HasKitchenObject() == false) return;

        switch (state.Value)
        {
            case State.Idle:
                break;
            case State.Frying:
                fryingTimer.Value += Time.deltaTime;
                if (fryingTimer.Value > fryingRecipeSO.fryingTimerMax)
                {
                    KitchenObject.DestroyKitchenObject(GetKitchenObject());
                    KitchenObject.SpawnKitchenObject(fryingRecipeSO.output, this);
                    burningRecipeSO = GetBurningRecipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());
                    state.Value = State.Fried;
                    burningTimer.Value = 0;
                    SetBurningRecipeSOClientRpc(KitchenGameMultiplayer.Instance.GetKitchenObjectSOIndex(GetKitchenObject().GetKitchenObjectSO()));
                }
                break;
            case State.Fried:
                burningTimer.Value += Time.deltaTime;
                if (burningTimer.Value > burningRecipeSO.burningTimerMax)
                {
                    KitchenObject.DestroyKitchenObject(GetKitchenObject());
                    KitchenObject.SpawnKitchenObject(burningRecipeSO.output, this);
                    state.Value = State.Burned;
                }
                break;
            case State.Burned:
                break;
        }
    }
    public override void Interact(Player player)
    {
        if (HasKitchenObject() == false)
        {
            if (player.HasKitchenObject() && HasRecipeWithInput(player.GetKitchenObject().GetKitchenObjectSO()))
            {
                KitchenObject kitchenObject = player.GetKitchenObject();
                kitchenObject.SetKitchenObjectParent(this);
                InteractLogicPlaceObjectOnCounterServerRpc(KitchenGameMultiplayer.Instance.GetKitchenObjectSOIndex(kitchenObject.GetKitchenObjectSO()));
            }
        }
        else
        {
            if (player.HasKitchenObject() == false)
            {
                GetKitchenObject().SetKitchenObjectParent(player);
                SetStateIdleServerRpc();
            } else {
                if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject)) {
                    if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO())) {
                        KitchenObject.DestroyKitchenObject(GetKitchenObject());
                        SetStateIdleServerRpc();
                    }
                }
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetStateIdleServerRpc() {
        state.Value = State.Idle;
        fryingTimer.Value = 0;
        burningTimer.Value = 0;
    }
    [ServerRpc(RequireOwnership = false)]
    private void InteractLogicPlaceObjectOnCounterServerRpc(int index) {
        fryingTimer.Value = 0f;
        state.Value = State.Frying;
        SetFryingRecipeSOClientRpc(index);
    }
    [ClientRpc]
    private void SetFryingRecipeSOClientRpc(int index) {
        KitchenObjectSO kitchenObjectSO = KitchenGameMultiplayer.Instance.GetKitchenObjectFromIndex(index);
        fryingRecipeSO = GetFryingRecipeSOWithInput(kitchenObjectSO);
    }
    [ClientRpc]
    private void SetBurningRecipeSOClientRpc(int index) {
        KitchenObjectSO kitchenObjectSO = KitchenGameMultiplayer.Instance.GetKitchenObjectFromIndex(index);
        burningRecipeSO = GetBurningRecipeSOWithInput(kitchenObjectSO);
    }

    public override void InteractAlternate(Player player)
    {
        //base.InteractAlternate(player);
    }
    private bool HasRecipeWithInput(KitchenObjectSO input)
    {
        FryingRecipeSO fryingRecipeSO = GetFryingRecipeSOWithInput(input);
        if (fryingRecipeSO != null) return true;
        else return false;
    }
    private KitchenObjectSO GetOutputForInput(KitchenObjectSO input)
    {
        FryingRecipeSO fryingRecipeSO = GetFryingRecipeSOWithInput(input);
        if (fryingRecipeSO != null) return fryingRecipeSO.output;
        else return null;
    }
    private FryingRecipeSO GetFryingRecipeSOWithInput(KitchenObjectSO input)
    {
        for (int i = 0; i < fryingRecipeSOArray.Length; i++)
        {
            if (fryingRecipeSOArray[i].input == input) return fryingRecipeSOArray[i];
        }
        return null;
    }
    private BurningRecipeSO GetBurningRecipeSOWithInput(KitchenObjectSO input)
    {
        for (int i = 0; i < burningRecipeSOArray.Length; i++)
        {
            if (burningRecipeSOArray[i].input == input) return burningRecipeSOArray[i];
        }
        return null;
    }
    public bool IsFried() => state.Value == State.Fried;
}
