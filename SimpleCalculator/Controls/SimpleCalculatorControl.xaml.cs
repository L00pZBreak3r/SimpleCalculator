using System.Windows;
using System.Windows.Controls;

using SimpleCalculator.ViewModels;

namespace SimpleCalculator.Controls
{
  /// <summary>
  /// Interaction logic for SimpleCalculatorControl.xaml
  /// </summary>
  public partial class SimpleCalculatorControl : UserControl
  {
    private readonly SimpleCalculatorViewModel mModel;

    public SimpleCalculatorControl()
    {
      mModel = new SimpleCalculatorViewModel();
      DataContext = mModel;

      InitializeComponent();
    }

    private void NumberButton_Click(object sender, RoutedEventArgs e)
    {
      mModel.PressNumberButton((sender as Button).Name);
    }
  }
}
