using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FountainSummonWardrobe : MonoBehaviour
{
    [SerializeField] private MoveProps moveProps;
    [SerializeField] private Transform wardrobe;
    [SerializeField] private Transform wardrobeTargetTransform;
    [SerializeField] private Transform[] benches;
    [SerializeField] private Transform fountain;
    private Vector3 wardrobeTarget;
    private Vector3[] originalBenchPositions;
    private Vector3[] secondaryBenchPositions;
    private Vector3 originalFountainPosition;
    private bool summoning;
    private bool touchedWaterPrev;

    void Start()
    {
        wardrobeTarget = wardrobeTargetTransform.position;
        originalFountainPosition = fountain.position;
        originalBenchPositions = new Vector3[benches.Length];
        secondaryBenchPositions = new Vector3[benches.Length];
        for (int i = 0; i < benches.Length; i++)
        {
            originalBenchPositions[i] = benches[i].position;
            secondaryBenchPositions[i] = benches[i].position + benches[i].right;
        }
    }

    void Update()
    {
        if (Physics.CheckSphere(transform.position, 0.25f) && Vector3.Distance(fountain.position, originalFountainPosition) < 0.01f && !touchedWaterPrev)
        {
            touchedWaterPrev = true;
            moveProps.Toggle();
        }
        else if (!Physics.CheckSphere(transform.position, 0.25f))
            touchedWaterPrev = false;
    }
}
