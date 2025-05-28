using System;
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

    private State state;
    private FryingRecipeSO fryingRecipeSO;
    private BurningRecipeSO burningRecipeSO;
    private float fryingTimer = 0;
    private float burningTimer = 0;

    private void Start()
    {
        state = State.Idle;
    }
    private void Update()
    {
        if (HasKitchenObject() == false) return;

        switch (state)
        {
            case State.Idle:
                break;
            case State.Frying:
                fryingTimer += Time.deltaTime;
                OnProgressChange?.Invoke(this, new IHasProgress.OnProgressChangeEventArgs { progressNormalized = fryingTimer / fryingRecipeSO.fryingTimerMax });
                if (fryingTimer > fryingRecipeSO.fryingTimerMax)
                {
                    GetKitchenObject().DestroySelf();
                    KitchenObject.SpawnKitchenObject(fryingRecipeSO.output, this);
                    burningRecipeSO = GetBurningRecipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());
                    state = State.Fried;
                    burningTimer = 0;
                    OnStateChanged?.Invoke(this, new OnStateChangedEventArgs { state = state});
                }
                break;
            case State.Fried:
                burningTimer += Time.deltaTime;
                OnProgressChange?.Invoke(this, new IHasProgress.OnProgressChangeEventArgs { progressNormalized = burningTimer / burningRecipeSO.burningTimerMax });
                if (burningTimer > burningRecipeSO.burningTimerMax)
                {
                    GetKitchenObject().DestroySelf();
                    KitchenObject.SpawnKitchenObject(burningRecipeSO.output, this);
                    state = State.Burned;
                    OnStateChanged?.Invoke(this, new OnStateChangedEventArgs { state = state });
                }
                break;
            case State.Burned:
                OnProgressChange?.Invoke(this, new IHasProgress.OnProgressChangeEventArgs { progressNormalized = 0f });
                break;
        }
    }
    public override void Interact(Player player)
    {
        if (HasKitchenObject() == false)
        {
            if (player.HasKitchenObject() && HasRecipeWithInput(player.GetKitchenObject().GetKitchenObjectSO()))
            {
                player.GetKitchenObject().SetKitchenObjectParent(this);
                fryingRecipeSO = GetFryingRecipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());
                state = State.Frying;
                fryingTimer = 0f;
                OnStateChanged?.Invoke(this, new OnStateChangedEventArgs { state = state });
                OnProgressChange?.Invoke(this, new IHasProgress.OnProgressChangeEventArgs { progressNormalized = fryingTimer / fryingRecipeSO.fryingTimerMax });
            }
        }
        else
        {
            if (player.HasKitchenObject() == false)
            {
                GetKitchenObject().SetKitchenObjectParent(player);
                state = State.Idle;
                OnStateChanged?.Invoke(this, new OnStateChangedEventArgs { state = state });
                OnProgressChange?.Invoke(this, new IHasProgress.OnProgressChangeEventArgs { progressNormalized = 0f });
            }else {
                if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject)) {
                    if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO())) {
                        GetKitchenObject().DestroySelf();
                        state = State.Idle;
                        OnStateChanged?.Invoke(this,new OnStateChangedEventArgs {
                            state = state
                        });
                        OnProgressChange?.Invoke(this, new IHasProgress.OnProgressChangeEventArgs { progressNormalized = 0 });
                    }
                }
            }
        }
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
}
