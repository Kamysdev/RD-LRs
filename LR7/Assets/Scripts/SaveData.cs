using UnityEngine;

[System.Serializable]
public class SaveData
{
    public string objectName;
    public string objectType;
    public Vector3 position;
    public Vector3 rotation;

    public SaveData(ObjectDescription objectDescription)
    {
        objectName = objectDescription.objectName;
        objectType = objectDescription.template != null ? objectDescription.template.objectType : string.Empty;
        position = objectDescription.transform.position;
        rotation = objectDescription.transform.eulerAngles;
    }

    public void ApplyTo(ObjectDescription objectDescription)
    {
        objectDescription.objectName = objectName;
        objectDescription.transform.position = position;
        objectDescription.UpdateRotation(rotation);
        objectDescription.gameObject.name = objectName;

        if (objectDescription.referenceButton != null)
        {
            ObjectButtonScript buttonScript = objectDescription.referenceButton.GetComponent<ObjectButtonScript>();
            if (buttonScript != null)
            {
                buttonScript.SetText(objectName);
            }
        }
    }
}
