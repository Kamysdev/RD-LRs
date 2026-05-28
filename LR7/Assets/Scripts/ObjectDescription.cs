using UnityEngine;

public class ObjectDescription : MonoBehaviour
{
    public string objectName;
    public ObjectTemplate template;
    public GameObject referenceButton;

    public void UpdateName(string newName)
    {
        objectName = newName;

        if (referenceButton != null)
        {
            ObjectButtonScript buttonScript = referenceButton.GetComponent<ObjectButtonScript>();
            if (buttonScript != null)
            {
                buttonScript.SetText(newName);
            }
        }

        gameObject.name = newName;
    }

    public void UpdateRotation(Vector3 newRotation)
    {
        transform.eulerAngles = newRotation;
    }

    public float GetYRotation()
    {
        return transform.eulerAngles.y;
    }

    public SaveData GetData()
    {
        return new SaveData(this);
    }
}
