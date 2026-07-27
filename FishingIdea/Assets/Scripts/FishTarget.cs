using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITargetModifier
{
    void OnSpawn(FishTarget target);
    void Tick(FishTarget target, float deltaTime);
}

public class FishTarget : MonoBehaviour
{
    public float AngleCenter { get; private set; }
    public float ArcWidth { get; private set; }
    private float ringRadius;
    private Transform ringCenter;

    [Header("Rotação visual")]
    public float rotationOffset = 0f;

    private ITargetModifier[] modifiers;
    public SpriteRenderer visual;

    public void Setup(float angle, float arcWidth, float radius)
    {
        AngleCenter = angle;
        ArcWidth = arcWidth;
        ringRadius = radius;
        ringCenter = transform.parent;

        modifiers = GetComponents<ITargetModifier>();
        UpdatePosition();

        foreach (var m in modifiers) m.OnSpawn(this);
    }

    public void TickModifiers(float deltaTime)
    {
        foreach (var m in modifiers) m.Tick(this, deltaTime);
        UpdatePosition();
    }

    public void SetAngle(float angle) => AngleCenter = angle;

    void UpdatePosition()
    {
        float rad = AngleCenter * Mathf.Deg2Rad;
        Vector3 pos = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad)) * ringRadius;
        transform.localPosition = pos;
        transform.localRotation = Quaternion.Euler(0f, 0f, AngleCenter + rotationOffset);
    }

    public bool IsAngleInside(float barAngle)
    {
        float delta = Mathf.Abs(Mathf.DeltaAngle(AngleCenter, barAngle));
        return delta <= ArcWidth / 2f;
    }




}