using System.Collections.Generic;
using UnityEngine;

public class HighlightScript : MonoBehaviour
{
    [SerializeField] private Renderer[] renderers;
    [SerializeField] private Color color = Color.white;

    private readonly List<Material> materials = new();

    private void Awake()
    {
        if (renderers == null || renderers.Length == 0)
        {
            renderers = GetComponentsInChildren<Renderer>();
        }

        materials.Clear();

        foreach (Renderer currentRenderer in renderers)
        {
            if (currentRenderer == null)
            {
                continue;
            }

            materials.AddRange(currentRenderer.materials);
        }
    }

    public void ToggleHighlight(bool value)
    {
        foreach (Material material in materials)
        {
            if (material == null)
            {
                continue;
            }

            if (value)
            {
                material.EnableKeyword("_EMISSION");
                material.SetColor("_EmissionColor", color);
            }
            else
            {
                material.DisableKeyword("_EMISSION");
            }
        }
    }

    private void OnMouseEnter()
    {
        ToggleHighlight(true);
    }

    private void OnMouseExit()
    {
        ToggleHighlight(false);
    }
}
