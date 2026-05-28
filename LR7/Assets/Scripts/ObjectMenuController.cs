using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ObjectMenuController : MonoBehaviour
{
    [SerializeField] private GameObject objectButtonPrefab;
    [SerializeField] private Transform objectsContainer;
    [SerializeField] private GameObject objectReferenceButtonPrefab;
    [SerializeField] private Transform referenceButtonsContainer;
    [SerializeField] private CursorScript cursor;
    [SerializeField] private float spawnHeight;
    [SerializeField] private Vector2 xSpawnRange = new(-15f, 15f);
    [SerializeField] private Vector2 zSpawnRange = new(-15f, 15f);

    private ObjectTemplate[] objectTemplates;
    private readonly List<ObjectDescription> objectsList = new();

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
                button.onClick.AddListener(() => CreateObject(cachedTemplate));
            }
        }
    }

    public void CreateObject(ObjectTemplate template)
    {
        ICommand command = new AddCommand(this, template);
        CommandInvoker.ExecuteCommand(command);
    }

    public GameObject CreateSceneObject(ObjectTemplate template)
    {
        if (template == null || template.objectPrefab == null)
        {
            return null;
        }

        Vector3 position = new(
            Random.Range(xSpawnRange.x, xSpawnRange.y),
            spawnHeight,
            Random.Range(zSpawnRange.x, zSpawnRange.y));

        GameObject newObject = Instantiate(template.objectPrefab, position, Quaternion.Euler(-90, 0, 0));
        EnsureInteractiveSetup(newObject);

        ObjectDescription description = PrepareObjectDescription(newObject, template);
        CreateReferenceButton(newObject, description);
        AddObject(description);

        if (cursor != null)
        {
            cursor.Select(newObject);
        }

        return newObject;
    }

    public GameObject CreateLoadedSceneObject(ObjectTemplate template, SaveData saveData)
    {
        if (template == null || template.objectPrefab == null || saveData == null)
        {
            return null;
        }

        GameObject newObject = Instantiate(template.objectPrefab, saveData.position, Quaternion.Euler(saveData.rotation));
        EnsureInteractiveSetup(newObject);

        ObjectDescription description = PrepareObjectDescription(newObject, template);
        CreateReferenceButton(newObject, description);
        saveData.ApplyTo(description);
        AddObject(description);

        return newObject;
    }

    public void DestroySceneObject(GameObject sceneObject)
    {
        if (sceneObject == null)
        {
            return;
        }

        ObjectDescription description = sceneObject.GetComponent<ObjectDescription>();
        if (description != null)
        {
            objectsList.Remove(description);

            if (description.referenceButton != null)
            {
                Destroy(description.referenceButton);
            }
        }

        Destroy(sceneObject);
    }

    public void DeselectCurrentObject()
    {
        if (cursor != null)
        {
            cursor.Deselect();
        }
    }

    public void UndoCommand()
    {
        CommandInvoker.UndoCommand();
    }

    public void RedoCommand()
    {
        CommandInvoker.RedoCommand();
    }

    public void AddObject(ObjectDescription objectDescription)
    {
        if (objectDescription != null && !objectsList.Contains(objectDescription))
        {
            objectsList.Add(objectDescription);
        }
    }

    public List<SaveData> GetSaveData()
    {
        List<SaveData> saveList = new();

        foreach (ObjectDescription item in objectsList)
        {
            if (item != null && item.gameObject.activeSelf)
            {
                saveList.Add(item.GetData());
            }
        }

        return saveList;
    }

    public ObjectTemplate FindTemplateByType(string type)
    {
        foreach (ObjectTemplate template in objectTemplates)
        {
            if (template != null && template.objectType == type)
            {
                return template;
            }
        }

        return null;
    }

    public void SceneClear()
    {
        DeselectCurrentObject();

        for (int i = objectsList.Count - 1; i >= 0; i--)
        {
            ObjectDescription description = objectsList[i];
            if (description == null)
            {
                continue;
            }

            if (description.referenceButton != null)
            {
                Destroy(description.referenceButton);
            }

            Destroy(description.gameObject);
        }

        objectsList.Clear();
    }

    public void LoadFromFile()
    {
#if UNITY_EDITOR
        string fileName = EditorUtility.OpenFilePanel("Load scene from json", string.Empty, "json");
        if (string.IsNullOrEmpty(fileName))
        {
            return;
        }

        List<SaveData> saveList = SaveLoadSystem.LoadFromFile(fileName);
        SceneClear();

        foreach (SaveData data in saveList)
        {
            ObjectTemplate template = FindTemplateByType(data.objectType);
            if (template == null)
            {
                continue;
            }

            ICommand command = new LoadCommand(this, template, data);
            CommandInvoker.ExecuteCommand(command);
        }

        CommandInvoker.ClearStack();
#else
        Debug.LogWarning("LoadFromFile is available only in the Unity Editor in this lab implementation.");
#endif
    }

    public void SaveToFile()
    {
#if UNITY_EDITOR
        string fileName = EditorUtility.SaveFilePanel("Save scene as json", string.Empty, "scene", "json");
        if (string.IsNullOrEmpty(fileName))
        {
            return;
        }

        SaveLoadSystem.SaveToFile(fileName, GetSaveData());
#else
        Debug.LogWarning("SaveToFile is available only in the Unity Editor in this lab implementation.");
#endif
    }

    private ObjectDescription PrepareObjectDescription(GameObject sceneObject, ObjectTemplate template)
    {
        ObjectDescription objectDescription = sceneObject.GetComponent<ObjectDescription>();
        if (objectDescription == null)
        {
            objectDescription = sceneObject.AddComponent<ObjectDescription>();
        }

        string objectName = sceneObject.name;
        if (objectName.EndsWith("(Clone)"))
        {
            objectName = objectName.Replace("(Clone)", string.Empty).TrimEnd();
        }

        objectDescription.objectName = objectName;
        objectDescription.template = template;
        sceneObject.name = objectName;

        return objectDescription;
    }

    private void CreateReferenceButton(GameObject sceneObject, ObjectDescription objectDescription)
    {
        if (sceneObject == null || objectReferenceButtonPrefab == null || referenceButtonsContainer == null || objectDescription == null)
        {
            return;
        }

        GameObject buttonObject = Instantiate(objectReferenceButtonPrefab, referenceButtonsContainer);
        ObjectButtonScript buttonScript = buttonObject.GetComponent<ObjectButtonScript>();
        Button button = buttonObject.GetComponent<Button>();

        objectDescription.referenceButton = buttonObject;

        if (buttonScript != null)
        {
            buttonScript.SetText(objectDescription.objectName);
        }

        if (button != null)
        {
            button.onClick.AddListener(() =>
            {
                if (cursor != null)
                {
                    cursor.Select(sceneObject);
                }
            });
        }
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
