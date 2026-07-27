using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class FishingMinigameController : MonoBehaviour
{
    public static FishingMinigameController Instance;

    [Header("Bar")]
    public float barSpeed = 120f;
    public bool alternateDirection = false;
    public Transform barVisual;
    private float barAngle;
    private int direction = 1;

    [Header("Progress")]
    public float progress = 0.5f;
    public float riseAmount = 0.12f;
    public float fallAmount = 0.06f;
    public float fallPerSecond = 0.04f;

    [Header("Ring")]
    public Transform ringCenter;
    public float ringRadius = 1.5f;
    public float targetArcWidth = 30f;
    public int maxActiveTargets = 2;

    [Header("Debug")]
    public FishData debugFish;

    private FishData currentFish;
    private List<FishTarget> activeTargets = new();
    private Action<bool> onFinish;
    private bool running;

    void Awake()
    {
        Instance = this;
        alternateDirection = false;
    }

    public void StartMinigame(FishData fish, Action<bool> callback)
    {
        currentFish = fish;
        onFinish = callback;
        progress = 0.5f;
        barAngle = 0f;
        direction = 1;
        running = true;
        activeTargets.Clear();

        for (int i = 0; i < maxActiveTargets; i++)
            SpawnTarget();
    }

    void Update()
    {
        if (!running) return;

        barAngle += barSpeed * direction * Time.deltaTime;
        if (barAngle >= 360f || barAngle < 0f)
        {
            barAngle = Mathf.Repeat(barAngle, 360f);
            if (alternateDirection) direction *= -1;
        }

        if (barVisual != null)
            barVisual.localRotation = Quaternion.Euler(0f, 0f, barAngle);

        foreach (var t in activeTargets)
            t.TickModifiers(Time.deltaTime);

        progress -= fallPerSecond * Time.deltaTime;

        if (Input.GetMouseButtonDown(1))
            TryHit();

        CheckEndConditions();
    }

    void TryHit()
    {
        FishTarget hit = activeTargets.Find(t => t.IsAngleInside(barAngle));

        if (hit != null)
        {
            progress += riseAmount;
            RemoveAndRespawn(hit);
        }
        else
        {
            progress -= fallAmount;
        }
    }

    void RemoveAndRespawn(FishTarget t)
    {
        activeTargets.Remove(t);
        Destroy(t.gameObject);
        SpawnTarget();
    }

    void SpawnTarget()
    {
        FishTarget prefab = PickTargetPrefabForFish();
        if (prefab == null) return;

        float angle = GetNonOverlappingAngle();
        FishTarget t = Instantiate(prefab, ringCenter);
        t.Setup(angle, targetArcWidth, ringRadius);
        activeTargets.Add(t);
    }

    FishTarget PickTargetPrefabForFish()
    {
        if (currentFish == null || currentFish.possibleTargetPrefabs == null || currentFish.possibleTargetPrefabs.Length == 0)
        {
            return null;
        }

        int index = Random.Range(0, currentFish.possibleTargetPrefabs.Length);
        return currentFish.possibleTargetPrefabs[index];
    }

    float GetNonOverlappingAngle()
    {
        float angle;
        int attempts = 0;
        do
        {
            angle = Random.Range(0f, 360f);
            attempts++;
        } while (activeTargets.Exists(t => Mathf.Abs(Mathf.DeltaAngle(t.AngleCenter, angle)) < targetArcWidth) && attempts < 20);
        return angle;
    }

    void CheckEndConditions()
    {
        if (progress >= 1f) Finish(true);
        else if (progress <= 0f) Finish(false);
    }

    void Finish(bool success)
    {
        running = false;
        foreach (var t in activeTargets) Destroy(t.gameObject);
        activeTargets.Clear();
        onFinish?.Invoke(success);
    }

    void OnDrawGizmos()
    {
        if (ringCenter == null) return;

        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(ringCenter.position, ringRadius);

        float rad = barAngle * Mathf.Deg2Rad;
        Vector3 barPos = ringCenter.position + new Vector3(Mathf.Cos(rad), Mathf.Sin(rad)) * ringRadius;
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(barPos, 0.1f);
    }
}