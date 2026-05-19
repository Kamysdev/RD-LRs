using UnityEngine;
using UnityEngine.UI;

public class ObjectMenuController : MonoBehaviour
{
    [SerializeField] private GameObject objectButtonPrefab;
    [SerializeField] private Transform objectsContainer;
    [SerializeField] private float spawnHeight;
    [SerializeField] private Vector2 xSpawnRange = new(-15f, 15f);
    [SerializeField] private Vector2 zSpawnRange = new(-15f, 15f);

    private ObjectTemplate[] objectTemplates;

    private void Start()
    {
        BuildMenu();
    }

    private void BuildMenu()
    {
        if (objectButtonPrefab == null || objectsContainer == null)
        {
            Debug.LogError("ObjectMenuController is not configured.");
            return;
        }

        objectTemplates = Resources.LoadAll<ObjectTemplate>("Objects");

        foreach (Transform child in objectsContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (ObjectTemplate template in objectTemplates)
        {
            if (template == null || template.objectPrefab == null)
            {
                continue;
            }

            GameObject buttonObject = Instantiate(objectButtonPrefab, objectsContainer);
            ObjectButtonScript buttonScript = buttonObject.GetComponent<ObjectButtonScript>();
            Button button = buttonObject.GetComponent<Button>();

            if (buttonScript != null)
            {
                buttonScript.SetText(template.objectType);
                buttonScript.SetSprite(template.objectIcon);
            }

            if (button != null)
            {
                ObjectTemplate cachedTemplate = template;
                button.onClick.AddListener(() => CreateObject(cachedTemplate.objectPrefab));
            }
        }
    }

    public void CreateObject(GameObject objectPrefab)
    {
        if (objectPrefab == null)
        {
            return;
        }

        Vector3 position = new(
            Random.Range(xSpawnRange.x, xSpawnRange.y),
            spawnHeight,
            Random.Range(zSpawnRange.x, zSpawnRange.y));

        GameObject newObject = Instantiate(objectPrefab, position, Quaternion.identity);
        EnsureInteractiveSetup(newObject);
    }

    private static void EnsureInteractiveSetup(GameObject targetObject)
    {
        if (targetObject == null)
        {
            return;
        }

        HighlightScript highlight = targetObject.GetComponent<HighlightScript>();
        if (highlight == null)
        {
            highlight = targetObject.AddComponent<HighlightScript>();
        }

        if (targetObject.GetComponent<Collider>() == null)
        {
            Renderer[] renderers = targetObject.GetComponentsInChildren<Renderer>();
            Bounds bounds = new(targetObject.transform.position, Vector3.one);

            if (renderers.Length > 0)
            {
                bounds = renderers[0].bounds;
                for (int i = 1; i < renderers.Length; i++)
                {
                    bounds.Encapsulate(renderers[i].bounds);
                }
            }

            BoxCollider collider = targetObject.AddComponent<BoxCollider>();
            collider.center = targetObject.transform.InverseTransformPoint(bounds.center);
            collider.size = bounds.size;
        }
    }
}
