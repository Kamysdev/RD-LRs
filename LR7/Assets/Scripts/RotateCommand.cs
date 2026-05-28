using UnityEngine;

public class RotateCommand : ICommand
{
    private readonly ObjectDescription selectedObject;
    private readonly Vector3 oldRotation;
    private readonly Vector3 newRotation;

    public RotateCommand(ObjectDescription selectedObject, Vector3 oldRotation, Vector3 newRotation)
    {
        this.selectedObject = selectedObject;
        this.oldRotation = oldRotation;
        this.newRotation = newRotation;
    }

    public void Execute()
    {
        if (selectedObject != null)
        {
            selectedObject.UpdateRotation(newRotation);
        }
    }

    public void Undo()
    {
        if (selectedObject != null)
        {
            selectedObject.UpdateRotation(oldRotation);
        }
    }
}
