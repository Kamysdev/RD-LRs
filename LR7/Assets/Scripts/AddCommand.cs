using UnityEngine;

public class AddCommand : ICommand
{
    private readonly ObjectMenuController builder;
    private readonly ObjectTemplate template;
    private GameObject newObject;

    public AddCommand(ObjectMenuController builder, ObjectTemplate template)
    {
        this.builder = builder;
        this.template = template;
    }

    public void Execute()
    {
        if (builder == null || template == null)
        {
            return;
        }

        newObject = builder.CreateSceneObject(template);
    }

    public void Undo()
    {
        if (builder == null || newObject == null)
        {
            return;
        }

        builder.DeselectCurrentObject();
        builder.DestroySceneObject(newObject);
        newObject = null;
    }
}
