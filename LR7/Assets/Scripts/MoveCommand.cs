using UnityEngine;

public class MoveCommand : ICommand
{
    private readonly GameObject selectedObject;
    private readonly Vector3 oldPosition;
    private readonly Vector3 newPosition;

    public MoveCommand(GameObject selectedObject, Vector3 oldPosition, Vector3 newPosition)
    {
        this.selectedObject = selectedObject;
        this.oldPosition = oldPosition;
        this.newPosition = newPosition;
    }

    public void Execute()
    {
        if (selectedObject != null)
        {
            selectedObject.transform.position = newPosition;
        }
    }

    public void Undo()
    {
        if (selectedObject != null)
        {
            selectedObject.transform.position = oldPosition;
        }
    }
}
