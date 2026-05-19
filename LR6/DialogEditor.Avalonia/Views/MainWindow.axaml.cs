using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using DialogEditor.Avalonia.ViewModels;

namespace DialogEditor.Avalonia.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private MainWindowViewModel ViewModel => (MainWindowViewModel)DataContext!;

    private void TreeSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (sender is TreeView treeView)
        {
            ViewModel.SelectTreeItem(treeView.SelectedItem);
        }
    }

    private void AddMessageClick(object? sender, RoutedEventArgs e) => ViewModel.AddMessage();

    private void AddAnswerClick(object? sender, RoutedEventArgs e) => ViewModel.AddAnswer();

    private void UpdateClick(object? sender, RoutedEventArgs e) => ViewModel.UpdateSelectedItem();

    private void DeleteClick(object? sender, RoutedEventArgs e) => ViewModel.DeleteSelectedItem();

    private void LinkClick(object? sender, RoutedEventArgs e) => ViewModel.LinkSelectedAnswerToSelectedMessage();

    private void ClearClick(object? sender, RoutedEventArgs e) => ViewModel.Clear();

    private void AddIncreaseClauseClick(object? sender, RoutedEventArgs e) => ViewModel.AddIncreaseClause();

    private void AddCheckClauseClick(object? sender, RoutedEventArgs e) => ViewModel.AddCheckClause();

    private void AddOpenDoorClauseClick(object? sender, RoutedEventArgs e) => ViewModel.AddOpenDoorClause();

    private void AddEndDialogClauseClick(object? sender, RoutedEventArgs e) => ViewModel.AddEndDialogClause();

    private void AddCustomClauseClick(object? sender, RoutedEventArgs e) => ViewModel.AddCustomClause();

    private void ParseRawActionClick(object? sender, RoutedEventArgs e) => ViewModel.ParseRawAction();

    private void ResetActionsClick(object? sender, RoutedEventArgs e) => ViewModel.ResetActions();

    private void RemoveClauseClick(object? sender, RoutedEventArgs e)
    {
        if (sender is Control { DataContext: ActionClauseViewModel clause })
        {
            ViewModel.RemoveClause(clause);
        }
    }

    private async void SaveClick(object? sender, RoutedEventArgs e)
    {
        var file = await StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Сохранить диалог в XML",
            SuggestedFileName = "dialog.xml",
            DefaultExtension = "xml",
            FileTypeChoices =
            [
                new FilePickerFileType("XML")
                {
                    Patterns = ["*.xml"],
                },
            ],
        });

        if (file is null)
        {
            return;
        }

        await using var stream = await file.OpenWriteAsync();
        ViewModel.Save(stream);
    }

    private async void LoadClick(object? sender, RoutedEventArgs e)
    {
        var files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Загрузить диалог из XML",
            AllowMultiple = false,
            FileTypeFilter =
            [
                new FilePickerFileType("XML")
                {
                    Patterns = ["*.xml"],
                },
            ],
        });

        if (files.Count == 0)
        {
            return;
        }

        await using var stream = await files[0].OpenReadAsync();
        ViewModel.Load(stream);
    }
}
