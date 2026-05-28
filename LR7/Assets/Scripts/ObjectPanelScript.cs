using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ObjectPanelScript : MonoBehaviour
{
    [SerializeField] private TMP_InputField objectName;
    [SerializeField] private Slider rotationSlider;
    [SerializeField] private CursorScript cursor;

    private ObjectDescription selectedObject;

    public void SetObject(ObjectDescription targetObject)
    {
        selectedObject = targetObject;

        if (objectName != null)
        {
            objectName.text = selectedObject != null ? selectedObject.objectName : string.Empty;
        }

        if (rotationSlider != null)
        {
            float rotation = selectedObject != null ? selectedObject.GetYRotation() : 0f;
            rotationSlider.SetValueWithoutNotify(rotation);
        }
    }

    public void ClearPanel()
    {
        if (objectName != null)
        {
            objectName.text = string.Empty;
        }

        if (rotationSlider != null)
        {
            rotationSlider.SetValueWithoutNotify(0f);
        }

        selectedObject = null;
    }

    public void UpdateName()
    {
        if (selectedObject == null || objectName == null)
        {
            return;
        }

        ICommand command = new RenameCommand(selectedObject, selectedObject.objectName, objectName.text);
        CommandInvoker.ExecuteCommand(command);
    }

    public void UpdateRotation(float yRotation)
    {
        ApplyRotation(yRotation);
    }

    public void UpdateRotationFromSlider()
    {
        if (rotationSlider == null)
        {
            return;
        }

        ApplyRotation(rotationSlider.value);
    }

    private void ApplyRotation(float yRotation)
    {
        if (selectedObject == null)
        {
            return;
        }

        Vector3 oldRotation = selectedObject.transform.eulerAngles;
        Vector3 newRotation = new(oldRotation.x, yRotation, oldRotation.z);

        if (oldRotation == newRotation)
        {
            return;
        }

        ICommand command = new RotateCommand(selectedObject, oldRotation, newRotation);
        CommandInvoker.ExecuteCommand(command);

        if (rotationSlider != null)
        {
            rotationSlider.SetValueWithoutNotify(selectedObject.GetYRotation());
        }
    }

    public void DeleteObject()
    {
        if (selectedObject == null)
        {
            return;
        }

        ICommand command = new DelCommand(selectedObject.gameObject);
        CommandInvoker.ExecuteCommand(command);

        if (cursor != null)
        {
            cursor.Deselect();
        }

        ClearPanel();
    }
}
