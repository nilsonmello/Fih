using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public enum FishingState
{
    Idle,
    Aiming,
    Casting,
    Waiting,
    BiteWindow,
    Minigame,
    Success,
    Failure
}

public class FishingController : MonoBehaviour
{
    [Header("Aim/Cast")]
    public float maxCastDistance = 6f;
    public Transform lurePreview;
    public LineRenderer aimLine;

    [Header("Waiting")]
    public float minWaitTime = 2f;
    public float maxWaitTime = 6f;
    public float biteWindowDuration = 0.8f;

    [Header("Peixes disponíveis")]
    public FishData[] availableFish;

    private FishingState state = FishingState.Idle;
    public FishingState State => state;

    private Vector2 castPoint;
    private Coroutine waitRoutine;
    private FishData hookedFish;

    void Update()
    {
        switch (state)
        {
            case FishingState.Idle:
                if (Input.GetMouseButtonDown(0)) EnterAiming();
                break;

            case FishingState.Aiming:
                UpdateAimPreview();
                if (Input.GetMouseButtonDown(0)) ThrowLure();
                break;

            case FishingState.BiteWindow:
                if (Input.GetMouseButtonDown(1)) StartMinigame();
                break;
        }
    }

    void SetState(FishingState newState)
    {
        state = newState;
        Debug.Log("Estado da pesca: " + state);
    }

    void EnterAiming() => SetState(FishingState.Aiming);

    void UpdateAimPreview()
    {
        Vector2 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 dir = (mouseWorld - (Vector2)transform.position);
        float dist = Mathf.Min(dir.magnitude, maxCastDistance);
        castPoint = (Vector2)transform.position + dir.normalized * dist;

        if (lurePreview != null)
            lurePreview.position = castPoint;
    }

    void ThrowLure()
    {
        SetState(FishingState.Casting);
        EnterWaiting();
    }

    void EnterWaiting()
    {
        SetState(FishingState.Waiting);
        waitRoutine = StartCoroutine(WaitForBite());
    }

    IEnumerator WaitForBite()
    {
        yield return new WaitForSeconds(Random.Range(minWaitTime, maxWaitTime));

        hookedFish = PickRandomFish();
        if (hookedFish == null)
        {
            SetState(FishingState.Idle);
            yield break;
        }

        SetState(FishingState.BiteWindow);
        yield return new WaitForSeconds(biteWindowDuration);

        if (state == FishingState.BiteWindow)
        {
            hookedFish = null;
            SetState(FishingState.Idle);
        }
    }

    FishData PickRandomFish()
    {
        if (availableFish == null || availableFish.Length == 0) return null;
        return availableFish[Random.Range(0, availableFish.Length)];
    }

    void StartMinigame()
    {
        if (FishingMinigameController.Instance == null)
        {
            SetState(FishingState.Idle);
            return;
        }

        SetState(FishingState.Minigame);
        FishingMinigameController.Instance.StartMinigame(hookedFish, OnMinigameResult);
    }

    void OnMinigameResult(bool success)
    {
        SetState(success ? FishingState.Success : FishingState.Failure);
        hookedFish = null;
        SetState(FishingState.Idle);
    }
}