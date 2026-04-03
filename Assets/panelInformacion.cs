using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class panelInformacion : MonoBehaviour
{
    [Header("Configuración de RectTransform")]
    public RectTransform panelRect;

    [Header("Valores de Desplazamiento (Left / Right)")]
    [Tooltip("Valores cuando el panel está CERRADO (fuera de pantalla)")]
    public Vector2 closedOffsets; // X = Left, Y = Right

    [Tooltip("Valores cuando el panel está ABIERTO (visible)")]
    public Vector2 openOffsets;   // X = Left, Y = Right

    [Header("Ajustes de Animación")]
    public float duration = 0.3f;
    private bool isOpen = false;

    public void TogglePanel()
    {
        StopAllCoroutines();
        if (isOpen)
        {
            StartCoroutine(MovePanelOffsets(closedOffsets));
        }
        else
        {
            StartCoroutine(MovePanelOffsets(openOffsets));
        }
        isOpen = !isOpen;
    }

    IEnumerator MovePanelOffsets(Vector2 targetOffsets)
    {
        float time = 0;

        // El 'Left' es offsetMin.x y el 'Right' es -offsetMax.x
        Vector2 startOffsets = new Vector2(panelRect.offsetMin.x, -panelRect.offsetMax.x);

        while (time < duration)
        {
            float t = time / duration;
            // Suavizado (SmoothStep)
            t = t * t * (3f - 2f * t);

            float currentLeft = Mathf.Lerp(startOffsets.x, targetOffsets.x, t);
            float currentRight = Mathf.Lerp(startOffsets.y, targetOffsets.y, t);

            // Aplicamos los valores al RectTransform
            panelRect.offsetMin = new Vector2(currentLeft, panelRect.offsetMin.y);
            panelRect.offsetMax = new Vector2(-currentRight, panelRect.offsetMax.y);

            time += Time.deltaTime;
            yield return null;
        }

        // Asegurar posición final exacta
        panelRect.offsetMin = new Vector2(targetOffsets.x, panelRect.offsetMin.y);
        panelRect.offsetMax = new Vector2(-targetOffsets.y, panelRect.offsetMax.y);
    }
}
