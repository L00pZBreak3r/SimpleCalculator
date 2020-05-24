using System.Windows;

using SimpleCalculator.ViewModels;

namespace SimpleCalculator
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    public MainWindow()
    {
      DataContext = new MainWindowViewModel();
      InitializeComponent();
    }
  }
}
