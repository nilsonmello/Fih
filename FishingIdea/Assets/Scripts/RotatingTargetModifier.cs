using UnityEngine;

public class RotatingTargetModifier : MonoBehaviour, ITargetModifier
{
    public float rotateSpeed = 40f;
    public void OnSpawn(FishTarget target) { }

    public void Tick(FishTarget target, float deltaTime)
    {
        target.SetAngle(target.AngleCenter + rotateSpeed * deltaTime);
    }
}
