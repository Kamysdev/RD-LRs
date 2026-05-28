using System.Collections.Generic;
using UnityEngine;

public class HighlightScript : MonoBehaviour
{
    [SerializeField] private Renderer[] renderers;
    [SerializeField] private Color selectionColor = Color.white;
    [SerializeField] private Color collisionColor = Color.red;

    private readonly List<Material> materials = new();
    private bool isHovered;
    private bool isSelected;
    private bool isColliding;
    private Collider cachedCollider;

    private void Awake()
    {
        if (renderers == null || renderers.Length == 0)
        {
            renderers = GetComponentsInChildren<Renderer>();
        }

        cachedCollider = GetComponent<Collider>();
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
        isSelected = value;
        ApplyHighlight();
    }

    private void Update()
    {
        isColliding = CheckIntersection();
        ApplyHighlight();
    }

    private bool CheckIntersection()
    {
        if (cachedCollider == null || !gameObject.activeInHierarchy)
        {
            return false;
        }

        Bounds bounds = cachedCollider.bounds;
        Collider[] overlaps = Physics.OverlapBox(bounds.center, bounds.extents * 0.95f, transform.rotation);

        foreach (Collider overlap in overlaps)
        {
            if (overlap == null || overlap == cachedCollider)
            {
                continue;
            }

            if (overlap.transform.root == transform.root)
            {
                continue;
            }

            ObjectDescription otherObject = overlap.GetComponentInParent<ObjectDescription>();
            if (otherObject == null)
            {
                continue;
            }

            return true;
        }

        return false;
    }

    private void ApplyHighlight()
    {
        bool shouldHighlight = isHovered || isSelected || isColliding;
        Color targetColor = isColliding ? collisionColor : selectionColor;

        if (shouldHighlight)
        {
            foreach (Material material in materials)
            {
                if (material == null)
                {
                    continue;
                }

                material.EnableKeyword("_EMISSION");
                material.SetColor("_EmissionColor", targetColor);
            }
        }
        else
        {
            foreach (Material material in materials)
            {
                if (material == null)
                {
                    continue;
                }

                material.DisableKeyword("_EMISSION");
            }
        }
    }

    private void OnMouseEnter()
    {
        isHovered = true;
        ApplyHighlight();
    }

    private void OnMouseExit()
    {
        isHovered = false;
        ApplyHighlight();
    }
}
