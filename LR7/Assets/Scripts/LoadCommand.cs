using UnityEngine;

public class LoadCommand : ICommand
{
    private readonly ObjectMenuController builder;
    private readonly ObjectTemplate template;
    private readonly SaveData saveData;
    private GameObject newObject;

    public LoadCommand(ObjectMenuController builder, ObjectTemplate template, SaveData saveData)
    {
        this.builder = builder;
        this.template = template;
        this.saveData = saveData;
    }

    public void Execute()
    {
        if (builder == null || template == null || saveData == null)
        {
            return;
        }

        newObject = builder.CreateLoadedSceneObject(template, saveData);
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
