using UnityEngine;

public class RenameCommand : ICommand
{
    private readonly ObjectDescription selectedObject;
    private readonly string oldName;
    private readonly string newName;

    public RenameCommand(ObjectDescription selectedObject, string oldName, string newName)
    {
        this.selectedObject = selectedObject;
        this.oldName = oldName;
        this.newName = newName;
    }

    public void Execute()
    {
        if (selectedObject != null)
        {
            selectedObject.UpdateName(newName);
        }
    }

    public void Undo()
    {
        if (selectedObject != null)
        {
            selectedObject.UpdateName(oldName);
        }
    }
}
