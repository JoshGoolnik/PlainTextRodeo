using Rodeo.PlainText.Core.Entities;
using Rodeo.PlainText.Core.Interfaces;
using Rodeo.PlainText.Core.Json;
using Rodeo.PlainText.Core.Xml;
using Rodeo.PlainText.Core.Yaml;
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

    private void ParseAndShowInput(string fileType, string input)
    {
        IPlainTextDataParser parser;
        switch (fileType.ToLower())
        {
            case "json":
                parser = new JsonParser();
                break;
            case "yaml":
                parser = new YamlParser();
                break;
            case "xml":
                parser = new XmlParser();
                break;
            // Add handling for other file types here
            default:
                statusTextBox.Text = $"Unsupported file type: {fileType}";
                outputTreeView.ItemsSource = null;
                return;
        }
        UpdateTreeViewAndStatusText(parser.Parse(input));
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