using System.Collections.Generic;
using UnityEngine;

public class PlatesCounterVisual : MonoBehaviour
{
    [SerializeField] private Transform counterTopPoint;
    [SerializeField] private GameObject plateVisualPrefab;
    [SerializeField] private PlatesCounter platesCounter;
    [SerializeField] private float plateOffsetY = 0.1f;

    private List<GameObject> plateVisuals = new();
    private void Start() {
        platesCounter.OnPlateSpawned += PlatesCounter_OnPlateSpawned;
        platesCounter.OnPlateRemoved += PlatesCounter_OnPlateRemoved;
    }

    private void PlatesCounter_OnPlateRemoved(object sender, System.EventArgs e) {
        Destroy(plateVisuals[plateVisuals.Count - 1]);
        plateVisuals.RemoveAt(plateVisuals.Count - 1);
    }

    private void PlatesCounter_OnPlateSpawned(object sender, System.EventArgs e) {
        GameObject plateVisualObject = Instantiate(plateVisualPrefab, counterTopPoint);
        plateVisualObject.transform.localPosition = new Vector3(0, plateOffsetY * plateVisuals.Count, 0);
        plateVisuals.Add(plateVisualObject);
    }
}
