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
        UpdateStatus(inputTextBox.Text);
    }
    private void inputTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        var json = ((TextBox)sender).Text;
        UpdateTreeView(json);
        UpdateStatus(json);
    }

    private void UpdateTreeView(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            outputTreeView.ItemsSource = null;
            return;
        }
        var result = Core.Json.JsonParser.ParseJson(json);
        if (result.Node == null)
        {
            return;
        }
        else
        {
            outputTreeView.ItemsSource = result.Node.Children;
        }
    }

    private void UpdateStatus(string json)
    {
        var result = Core.Json.JsonParser.ParseJson(json);
        statusTextBox.Text = result.Message + " | Line: " + result.LineNumber;
    }
}