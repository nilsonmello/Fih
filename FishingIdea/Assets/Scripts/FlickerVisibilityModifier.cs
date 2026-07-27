using UnityEngine;

public class FlickerVisibilityModifier : MonoBehaviour, ITargetModifier
{
    public float visibleDuration = 2f;
    public float hiddenDuration = 1f;
    private float timer;
    private bool visible = true;

    public void OnSpawn(FishTarget target) => timer = visibleDuration;

    public void Tick(FishTarget target, float deltaTime)
    {
        timer -= deltaTime;
        if (timer <= 0f)
        {
            visible = !visible;
            target.visual.enabled = visible;
            timer = visible ? visibleDuration : hiddenDuration;
        }
    }
}