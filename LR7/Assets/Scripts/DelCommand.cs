using UnityEngine;

public class DelCommand : ICommand
{
    private readonly GameObject selectedObject;

    public DelCommand(GameObject selectedObject)
    {
        this.selectedObject = selectedObject;
    }

    public void Execute()
    {
        if (selectedObject == null)
        {
            return;
        }

        selectedObject.SetActive(false);

        ObjectDescription description = selectedObject.GetComponent<ObjectDescription>();
        if (description != null && description.referenceButton != null)
        {
            description.referenceButton.SetActive(false);
        }
    }

    public void Undo()
    {
        if (selectedObject == null)
        {
            return;
        }

        selectedObject.SetActive(true);

        ObjectDescription description = selectedObject.GetComponent<ObjectDescription>();
        if (description != null && description.referenceButton != null)
        {
            description.referenceButton.SetActive(true);
        }
    }
}
