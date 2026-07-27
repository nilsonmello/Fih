using UnityEngine;

[CreateAssetMenu(fileName = "NewFish", menuName = "Fishing/Fish Data")]
public class FishData : ScriptableObject
{
    [Header("Identidade")]
    public string fishName;
    public string fishId;
    public Sprite sprite;

    [Header("Atributos físicos")]
    public float weight; // kg
    public float size;   // cm

    [Header("Targets que esse peixe pode spawnar")]
    public FishTarget[] possibleTargetPrefabs;
}