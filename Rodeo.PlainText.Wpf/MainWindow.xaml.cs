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
        fileTypeComboBox.SelectionChanged += fileTypeComboBox_SelectionChanged;
        inputTextBox.TextChanged += inputTextBox_TextChanged;
        InputUpdate();
    }

    private void fileTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) => InputUpdate();

    private void inputTextBox_TextChanged(object sender, TextChangedEventArgs e) => InputUpdate();

    private void InputUpdate()
    {
        var selectedFileType = ((ComboBoxItem)fileTypeComboBox.SelectedItem).Content.ToString();
        var input = inputTextBox.Text;
        ParseAndShowInput(selectedFileType, input);
    }

    // At the moment this is redundant, but later we will add formats beyond JSON.
    private void ParseAndShowInput(string fileType, string input)
    {
        switch (fileType.ToLower())
        {
            case "json":
                ParseAndShowJson(input);
                break;
            case "yaml":
                ParseAndShowYaml(input);
                break;
            // Add handling for other file types here
            default:
                statusTextBox.Text = $"Unsupported file type: {fileType}";
                outputTreeView.ItemsSource = null;
                break;
        }
    }

    private void ParseAndShowJson(string json)
    {
        var jsonParser = new Core.Json.JsonParser();
        var result = jsonParser.Parse(json);
        UpdateTreeViewAndStatusText(result);
    }

    private void ParseAndShowYaml(string yaml)
    {
        var yamlParser = new Core.Yaml.YamlParser();
        var result = yamlParser.Parse(yaml);
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