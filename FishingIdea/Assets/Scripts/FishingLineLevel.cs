using UnityEngine;

public class FishingLineVisual : MonoBehaviour
{
    [Header("Referências")]
    public LineRenderer lineRenderer;
    public Transform rodTip;       // ponto fixo na ponta da vara
    public Transform fishVisual;   // objeto com o sprite do peixe, vai se mover
    public SpriteRenderer fishSpriteRenderer;

    [Header("Limites de profundidade")]
    public Transform topPoint;    // posição do peixe quando progress = 1 (perto da superfície)
    public Transform bottomPoint; // posição do peixe quando progress = 0 (fundo, prestes a escapar)

    void Update()
    {
        var minigame = FishingMinigameController.Instance;
        if (minigame == null) return;

        bool active = minigame.IsRunning;

        if (lineRenderer != null) lineRenderer.enabled = active;
        if (fishVisual != null) fishVisual.gameObject.SetActive(active);

        if (!active) return;

        // posiciona o peixe entre bottomPoint (progress 0) e topPoint (progress 1)
        Vector3 pos = Vector3.Lerp(bottomPoint.position, topPoint.position, minigame.progress);
        fishVisual.position = pos;

        // atualiza o sprite conforme o peixe fisgado
        if (fishSpriteRenderer != null && minigame.CurrentFish != null)
            fishSpriteRenderer.sprite = minigame.CurrentFish.sprite;

        // desenha a linha da vara até o peixe
        if (lineRenderer != null)
        {
            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, rodTip.position);
            lineRenderer.SetPosition(1, fishVisual.position);
        }
    }
}