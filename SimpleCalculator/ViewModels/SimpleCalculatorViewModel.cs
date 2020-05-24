using System;
using System.Text;
using System.Globalization;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Linq;

using SimpleCalculator.Helpers;

namespace SimpleCalculator.ViewModels
{
  internal class SimpleCalculatorViewModel : ViewModelBase
  {
    private const int MAX_NUMBER_LENGTH = 32;
    private static readonly char[] OPERATION_SYMBOLS = { '+', '-', '×', '/' };

    private readonly StringBuilder mNumber = new StringBuilder("0");
    private readonly StringBuilder mCurrentHistoryItem = new StringBuilder();
    private double mOperand1;
    private double mOperand2;
    private int mCurrentOperand = 1;
    private int mOperation;
    private bool mStartNewOperand;

    public SimpleCalculatorViewModel()
    {
      mNumberDecimalSeparator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
      mText = "0";
      CultureInfo ci = CultureInfo.CurrentUICulture;
      mHistoryTitle = (ci.LCID == MainWindowViewModel.LCID_RUSSIAN) ? MainWindowViewModel.HISTORY_TITLE_RUSSIAN : MainWindowViewModel.HISTORY_TITLE_ENGLISH;
    }

    #region NumberDecimalSeparator

    private string mNumberDecimalSeparator;

    public string NumberDecimalSeparator
    {
      get { return mNumberDecimalSeparator; }
      set
      {
        if (mNumberDecimalSeparator != value)
        {
          mNumberDecimalSeparator = value;
          RaisePropertyChanged();
        }
      }
    }

    #endregion
    
    #region HistoryTitle

    private string mHistoryTitle;

    public string HistoryTitle
    {
      get { return mHistoryTitle; }
      set
      {
        if (mHistoryTitle != value)
        {
          mHistoryTitle = value;
          RaisePropertyChanged();
        }
      }
    }

    #endregion

    #region Text

    private string mText;

    public string Text
    {
      get { return mText; }
      set
      {
        if (mText != value)
        {
          mText = value;
          RaisePropertyChanged();
        }
      }
    }

    #endregion

    #region HistoryListItemsSource

    private readonly ObservableCollection<string> mHistoryListItemsSource = new ObservableCollection<string>();
    public ObservableCollection<string> HistoryListItemsSource
    {
      get { return mHistoryListItemsSource; }
    }

    #endregion

    #region SelectedHistoryListItem

    private string mSelectedHistoryListItem;

    public string SelectedHistoryListItem
    {
      get { return mSelectedHistoryListItem; }
      set
      {
        if (mSelectedHistoryListItem != value)
        {
          mSelectedHistoryListItem = value;
          RaisePropertyChanged();
          ParseSelectedHistoryItem();
        }
      }
    }

    #endregion

    public void PressNumberButton(string aButtonText)
    {
      char aOperand = aButtonText[7];
      bool aDecimalSeparator = (aOperand == 'I');
      bool aParseOperand = true;
      bool aUpdateText = true;

      if (mStartNewOperand)
      {
        mNumber.Clear();
        mNumber.Append('0');
        mStartNewOperand = false;
      }

      if (mHistoryListItemsSource.Count == 0)
        mHistoryListItemsSource.Add(string.Empty);

      if ((aOperand >= '0') && (aOperand <= '9') || aDecimalSeparator)
      {
        if (mNumber.Length < MAX_NUMBER_LENGTH)
        {
          if (aDecimalSeparator)
            aOperand = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator[0];
          else if ((mNumber.Length == 1) && (mNumber.ToString()[0] == '0'))
            mNumber.Clear();

          mNumber.Append(aOperand);
        }
      }
      else if (aOperand == 'F')
      {
        if (mNumber.Length > 1)
          mNumber.Remove(mNumber.Length - 1, 1);
        else
        {
          mNumber.Clear();
          mNumber.Append('0');
        }
      }
      else if (aOperand == 'Z')
      {
        bool aNegative = (mNumber.Length > 0) && (mNumber.ToString()[0] == '-');
        if (aNegative)
        {
          mNumber.Remove(0, 1);
          if (mNumber.Length == 0)
            mNumber.Append('0');
        }
        else
        {
          if (mNumber.Length == 0)
            mNumber.Append("-0");
          else
            mNumber.Insert(0, '-');
        }
      }
      else
      {
        double aOp2;
        switch (aOperand)
        {
          case 'A':
          case 'B':
          case 'C':
          case 'D':
          case 'E':
            mCurrentHistoryItem.Append((mCurrentOperand < 2) ? mOperand1 : mOperand2);
            bool aOpSuccess = PerformOperation();
            if (aOpSuccess)
            {
              mNumber.Clear();
              mNumber.Append(mOperand1);
            }
            else
              aUpdateText = false;
            mOperation = aOperand - 'A';
            aParseOperand = false;
            mStartNewOperand = true;
            if (mOperation > 0)
            {
              if (mCurrentOperand < 2)
                mCurrentOperand++;
              mCurrentHistoryItem.Append(" ");
              mCurrentHistoryItem.Append(OPERATION_SYMBOLS[mOperation - 1]);
              mCurrentHistoryItem.Append(" ");
              mHistoryListItemsSource[0] = mCurrentHistoryItem.ToString();
            }
            else if (aOpSuccess)
            {
              mCurrentHistoryItem.Append(" = ");
              mCurrentHistoryItem.Append(mOperand1);
              mHistoryListItemsSource[0] = mCurrentHistoryItem.ToString();
              mCurrentHistoryItem.Clear();
              mHistoryListItemsSource.Insert(0, string.Empty);
            }
            break;
          case 'K':
            mNumber.Clear();
            mNumber.Append('0');
            mCurrentHistoryItem.Clear();
            mOperand1 = 0.0;
            mOperand2 = 0.0;
            mCurrentOperand = 1;
            mOperation = 0;
            aParseOperand = false;
            mStartNewOperand = true;
            break;
          case 'P':
            if (mCurrentOperand >= 2)
            {
              mOperand2 *= mOperand1 / 100.0;
              mNumber.Clear();
              mNumber.Append(mOperand2);
              aParseOperand = false;
              mStartNewOperand = true;
            }
            break;
          case 'Q':
            if (mCurrentOperand >= 2)
            {
              mOperand2 = Math.Sqrt(mOperand2);
              aOp2 = mOperand2;              
            }
            else
            {
              mOperand1 = Math.Sqrt(mOperand1);
              aOp2 = mOperand1;
            }
            mNumber.Clear();
            mNumber.Append(aOp2);
            aParseOperand = false;
            mStartNewOperand = true;
            break;
          case 'R':
            if (mCurrentOperand >= 2)
            {
              mOperand2 = 1.0 / mOperand2;
              aOp2 = mOperand2;
            }
            else
            {
              mOperand1 = 1.0 / mOperand1;
              aOp2 = mOperand1;
            }
            mNumber.Clear();
            mNumber.Append(aOp2);
            aParseOperand = false;
            mStartNewOperand = true;
            break;
          case 'S':
            if (mCurrentOperand >= 2)
            {
              mOperand2 *= mOperand2;
              aOp2 = mOperand2;
            }
            else
            {
              mOperand1 *= mOperand1;
              aOp2 = mOperand1;
            }
            mNumber.Clear();
            mNumber.Append(aOp2);
            aParseOperand = false;
            mStartNewOperand = true;
            break;
          case 'T':
            if (mCurrentOperand >= 2)
            {
              mOperand2 *= mOperand2 * mOperand2;
              aOp2 = mOperand2;
            }
            else
            {
              mOperand1 *= mOperand1 * mOperand1;
              aOp2 = mOperand1;
            }
            mNumber.Clear();
            mNumber.Append(aOp2);
            aParseOperand = false;
            mStartNewOperand = true;
            break;
        }
      }

      if (aUpdateText)
      {
        Text = mNumber.ToString();
        if (aParseOperand && double.TryParse(Text, out double aOp1))
        {
          if (mCurrentOperand == 1)
            mOperand1 = aOp1;
          else if (mCurrentOperand == 2)
            mOperand2 = aOp1;
        }
      }      
    }

    private bool PerformOperation()
    {
      bool res = (mCurrentOperand >= 2) && (mOperation > 0);
      if (res)
      {
        switch (mOperation)
        {
          case 1:
            mOperand1 += mOperand2;
            break;
          case 2:
            mOperand1 -= mOperand2;
            break;
          case 3:
            mOperand1 *= mOperand2;
            break;
          case 4:
            mOperand1 /= mOperand2;
            break;
        }
        mCurrentOperand--;
      }
      return res;
    }

    private void ParseSelectedHistoryItem()
    {
      if (!string.IsNullOrEmpty(mSelectedHistoryListItem))
      {
        int aPos = mSelectedHistoryListItem.LastIndexOf('=');
        if ((aPos > 0) && double.TryParse(mSelectedHistoryListItem.Substring(aPos + 1).Trim(), out double d))
        {
          mNumber.Clear();
          mNumber.Append(d);
          Text = mNumber.ToString();
          if (mCurrentOperand == 1)
            mOperand1 = d;
          else if (mCurrentOperand == 2)
            mOperand2 = d;
        }
      }
    }

  }
}
