using System.Collections.Generic;

public static class CommandInvoker
{
    private const int MaxUndoCommands = 10;

    private static readonly LinkedList<ICommand> UndoCommands = new();
    private static readonly LinkedList<ICommand> RedoCommands = new();

    public static void ExecuteCommand(ICommand command)
    {
        if (command == null)
        {
            return;
        }

        command.Execute();
        PushUndo(command);
        RedoCommands.Clear();
    }

    public static void UndoCommand()
    {
        if (UndoCommands.Count == 0)
        {
            return;
        }

        ICommand lastCommand = UndoCommands.Last.Value;
        UndoCommands.RemoveLast();
        lastCommand.Undo();
        RedoCommands.AddLast(lastCommand);
    }

    public static void RedoCommand()
    {
        if (RedoCommands.Count == 0)
        {
            return;
        }

        ICommand lastUndoneCommand = RedoCommands.Last.Value;
        RedoCommands.RemoveLast();
        lastUndoneCommand.Execute();
        PushUndo(lastUndoneCommand);
    }

    public static void ClearStack()
    {
        UndoCommands.Clear();
        RedoCommands.Clear();
    }

    private static void PushUndo(ICommand command)
    {
        UndoCommands.AddLast(command);

        if (UndoCommands.Count > MaxUndoCommands)
        {
            UndoCommands.RemoveFirst();
        }
    }
}
