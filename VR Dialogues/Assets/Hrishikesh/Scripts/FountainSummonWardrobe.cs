using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FountainSummonWardrobe : MonoBehaviour
{
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

    public void ToggleWardrobe()
    {
        summoning = !summoning;
        if (summoning)
        {
            wardrobe.position = wardrobeTarget + Vector3.up * 400;
            wardrobe.rotation = wardrobeTargetTransform.rotation;
        }
    }

    void Update()
    {
        if (Physics.CheckSphere(transform.position, 0.25f) && Vector3.Distance(fountain.position, originalFountainPosition) < 0.01f && !touchedWaterPrev)
        {
            touchedWaterPrev = true;
            ToggleWardrobe();
        }
        else if (!Physics.CheckSphere(transform.position, 0.25f))
            touchedWaterPrev = false;

        if (summoning)
        {
            wardrobe.position = Vector3.Lerp(wardrobe.position, wardrobeTarget, 0.05f);
            for (int i = 0; i < benches.Length; i++)
                benches[i].position = Vector3.Lerp(benches[i].position, secondaryBenchPositions[i], 0.1f);
            fountain.position = Vector3.Lerp(fountain.position, originalFountainPosition + Vector3.right * 3, 0.05f);
        }
        else
        {
            wardrobe.position = Vector3.Lerp(wardrobe.position, wardrobeTarget + Vector3.up * 400, 0.0005f);
            for (int i = 0; i < benches.Length; i++)
                benches[i].position = Vector3.Lerp(benches[i].position, originalBenchPositions[i], 0.1f);
            fountain.position = Vector3.Lerp(fountain.position, originalFountainPosition, 0.05f);
        }
    }
}
