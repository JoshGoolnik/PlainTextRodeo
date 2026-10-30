using Rodeo.PlainText.Core.Entities;
using System.Windows;
using System.Windows.Controls;

namespace Rodeo.PlainText.Wpf;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        inputTextBox.TextChanged += inputTextBox_TextChanged;
        var input = inputTextBox.Text;
        ParseAndShowInput(input);

    }
    private void inputTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        var input = ((TextBox)sender).Text;
        ParseAndShowInput(input);
    }

    // At the moment this is redundant, but later we will add formats beyond JSON.
    private void ParseAndShowInput(string input) => ParseAndShowJson(input);

    private void ParseAndShowJson(string json)
    {
        var result = Core.Json.JsonParser.ParseJson(json);
        UpdateTreeViewAndStatusText(result);
    }

    private void UpdateTreeViewAndStatusText(DataNodeResult result)
    {
        if (result.Node != null)
        {
            outputTreeView.ItemsSource = new[] { result.Node };
            statusTextBox.Text = "Valid Input :)";
        }
        else if (result.Message != null)
        {
            statusTextBox.Text = $"Line {result.LineNumber}: {result.Message}";
        }
        else
        {   
            outputTreeView.ItemsSource = null;
            statusTextBox.Text = "Nothing to display.";
        }
    }
}