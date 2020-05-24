using System.Globalization;

using SimpleCalculator.Helpers;

namespace SimpleCalculator.ViewModels
{
  internal class MainWindowViewModel : ViewModelBase
  {
    private const string PROGRAM_CAPTION_ENGLISH = "Calculator";
    private const string PROGRAM_CAPTION_RUSSIAN = "Калькулятор";
    internal const string HISTORY_TITLE_ENGLISH = "History";
    internal const string HISTORY_TITLE_RUSSIAN = "История операций";
    internal const int LCID_RUSSIAN = 1049;

    public MainWindowViewModel()
    {
      CultureInfo ci = CultureInfo.CurrentUICulture;
      mWindowText = (ci.LCID == LCID_RUSSIAN) ? PROGRAM_CAPTION_RUSSIAN : PROGRAM_CAPTION_ENGLISH;
    }
    
    #region WindowText

    private string mWindowText;

    public string WindowText
    {
      get { return mWindowText; }
      set
      {
        if (mWindowText != value)
        {
          mWindowText = value;
          RaisePropertyChanged();
        }
      }
    }

    #endregion

  }
}
