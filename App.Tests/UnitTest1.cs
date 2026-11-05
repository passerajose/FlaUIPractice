using System;
using System.IO;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Definitions;
using FlaUI.Core.Tools;
using FlaUI.UIA3;
using NUnit.Framework;

using Application = FlaUI.Core.Application;

namespace TaskManager.Tests;

[TestFixture]
[NonParallelizable]
public class TaskTests
{
    private const string AppPath = @"C:\Users\espin\git\App.Tests\TaskManager.App\bin\Debug\net10.0-windows\TaskManager.App.exe";

    private Application? _app;
    private UIA3Automation? _automation;
    private Window _window = null!;

    [SetUp]
    public void SetUp()
    {
        Assert.That(File.Exists(AppPath), Is.True, $"No se encontró la aplicación: {AppPath}");

        _automation = new UIA3Automation();
        _app = Application.Launch(AppPath);

        _window = _app.GetMainWindow(_automation, TimeSpan.FromSeconds(10)) ?? throw new InvalidOperationException("No apareció la ventana principal.");
    }

    [Test]
    public void AddTask_WithValidTitle_ShowsTaskInList()
    {
        var titleInput = FindById("TaskTitleInput").AsTextBox();
        var addButton = FindById("AddTaskButton").AsButton();
        var taskList = FindById("TaskList");

        titleInput.Text = "Practicar FlaUI";
        addButton.Invoke();

        // Esperar a que aparezca la tarea.
        var addedTask = Retry.WhileNull(() => taskList.FindFirstDescendant(cf => cf.ByControlType(ControlType.ListItem).And(cf.ByName("Practicar FlaUI"))), timeout: TimeSpan.FromSeconds(5), 
            interval: TimeSpan.FromMilliseconds(200), throwOnTimeout: true).Result;

        // Verificar el resultado.
        Assert.That(addedTask, Is.Not.Null);
        Assert.That(titleInput.Text, Is.Empty);
    }

    private AutomationElement FindById(string automationId)
    {
        return Retry.WhileNull(() => _window.FindFirstDescendant(cf => cf.ByAutomationId(automationId)), timeout: TimeSpan.FromSeconds(5), interval: TimeSpan.FromMilliseconds(200), throwOnTimeout: true)
            .Result ?? throw new InvalidOperationException($"No se encontró el control: {automationId}");
    }

    [TearDown]
    public void TearDown()
    {
        try
        {
            if (_app is not null && !_app.HasExited)
            {
                //_app.Close();
            }
        }
        finally
        {
            _app?.Dispose();
            _automation?.Dispose();

            _app = null;
            _automation = null;
        }
    }
}