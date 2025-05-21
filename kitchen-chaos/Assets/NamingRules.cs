using System;
using UnityEngine;

public class NamingRules : MonoBehaviour
{
    //Constants UpperCase SnakeCase
    public const int CONSTANT_FIELD = 69; // nice

    //Properties: PascalCase
    public static NamingRules Instance { get; private set; }

    //Events: PascalCase
    public event EventHandler OnSomethingHappened;

    //Fields: camelCase
    private float nameVariable;

    //Functions: PascalCase
    private void Awake(){
        Instance = this;
        DoSomething(10f);
    }

    private void DoSomething(float time){
        nameVariable = time + Time.deltaTime;
        if (nameVariable > 0)
        {
            //Do something else
        }
    }
}
