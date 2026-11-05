using System.Windows;

namespace TaskManager.App;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void AddTask_Click(object sender, RoutedEventArgs e)
    {
        string title = TaskTitleInput.Text.Trim();

        if (string.IsNullOrWhiteSpace(title))
        {
            StatusMessage.Text = "Escribe un título.";
            return;
        }

        TaskList.Items.Add(title);
        TaskTitleInput.Clear();

        StatusMessage.Text = "Tarea agregada.";
    }
}