using System;
using System.Diagnostics;
using System.Security.Authentication;
using System.Text;
using System.Windows;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using static System.Net.Mime.MediaTypeNames;

namespace PR3;

/// <summary>
/// InterbaseAction logic for MainWindow.xaml
/// </summary> 
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    List<string> inputHistory = new List<string>() {};
    List<string> userInput = new List<string>() { };
    string currentNum = "0";
    string sign = "";
    string prevNum = "0";
    bool basePressed = false;
    bool numPressed = false;
    bool beforePressed = false;
    bool equalPressed = false;
    bool isError = false;
    int Brackets = 0;
    string[] nums = { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "." };
    string[] constNums = { "3.1415926535898", "2.7182818284590" };
    string[] specSign = { "=", "(", ")", "CE", "del" };
    string[] baseSigns = { " + ", " - ", " × ", " ÷ ", " ^ " };
    string[] beforeSigns = { "sin", "cos", "tg", "log", "ln", "1/", "10 ^ ", "√", "sqr", "fact", "abs", "negate"};

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        var button = sender as Button;
        var value = button.Tag.ToString();

        if (nums.Contains(value) || constNums.Contains(value) || specSign.Contains(value) || isError)
        {
            if (isError)
            {
                numPressed = false;
                basePressed = false;
                currentNum = "0";
                ResultBox.Text = currentNum;
                HistoryBox.Text = "";
                userInput = new List<string>() { };
                inputHistory = new List<string>() { };
                isError = false;
                return;
            }
            if (value == "(")
            {
                Brackets ++;
                if (!baseSigns.Contains(currentNum) & currentNum != "0")
                {
                    userInput.Add(currentNum);
                    inputHistory.Add(currentNum);
                    basePressed = true;
                    sign = " × ";
                    userInput.Add(sign);
                    inputHistory.Add(sign);
                }
                userInput.Add(value);
                inputHistory.Add(value);
                prevNum = currentNum;
                PrintHistory();
            }
            else if (value == ")" & Brackets>0)
            {
                Brackets --;
                userInput.Add(value);
                inputHistory.Add(value);
                PrintHistory();
            }
            if (value == "=")
            {
                if (equalPressed & baseSigns.Contains(sign))
                {
                    userInput.Add(currentNum);
                    userInput.Add(sign);
                    userInput.Add(prevNum);
                    inputHistory = userInput;
                    currentNum = Act();
                    ResultBox.Text = currentNum;
                    PrintHistory();
                    HistoryBox.Text += " =";
                }
                else
                {
                    if ((basePressed || inputHistory.Count == 0) & !beforePressed)
                    {
                        inputHistory.Add(currentNum);
                        userInput.Add(currentNum);
                        prevNum = currentNum;
                    }
                    numPressed = false;
                    basePressed = false;
                    equalPressed = true;
                    currentNum = Act();
                    ResultBox.Text = currentNum;
                    PrintHistory();
                    HistoryBox.Text += " =";
                }
                userInput = new List<string>() { };
                inputHistory = new List<string>() { };
                return;

            }
            if (basePressed & !numPressed)
            {
                inputHistory.Add(sign);
                userInput.Add(sign);
                
            }
            if (nums.Contains(value) || constNums.Contains(value))
            {
                numPressed = true;
                if (constNums.Contains(value) & !basePressed)
                {
                    currentNum = value;
                    userInput = new List<string>() { };
                    inputHistory = new List<string>() { };
                }
                if (((currentNum == prevNum || constNums.Contains(currentNum) || constNums.Contains(value)) & value != ".") || equalPressed)
                    currentNum = value;
                else if (currentNum.Length < 15 & !constNums.Contains(value))
                {
                    if ((currentNum.EndsWith(".") || currentNum.Contains(".")) & value == ".")
                        return;
                    currentNum += value;
                }
                prevNum = "0";
            }
            else if(value=="del" & !constNums.Contains(currentNum))
            {
                if (basePressed || !numPressed)
                    return;
                if (currentNum.Length > 1 & !isError & !equalPressed)
                    currentNum = currentNum.Substring(0, currentNum.Length - 1);
                else if (equalPressed)
                {
                    HistoryBox.Text = "";
                    userInput = new List<string>() { };
                    inputHistory = new List<string>() { };
                    numPressed = false;
                    basePressed = false;
                    sign = "";
                    ResultBox.Text = "0";
                    currentNum = "0";
                    prevNum = "0";
                }
                else
                    currentNum = "0";
            }
            else if(value=="CE")
            {
                if (!basePressed)
                    inputHistory = new List<string>() { };
                else
                    inputHistory.Remove(inputHistory.Last());
                if (currentNum == "0" || equalPressed || isError)
                {
                    HistoryBox.Text = "";
                    userInput = new List<string>() { };
                    inputHistory = new List<string>() { };
                    numPressed = false;
                    basePressed = false;
                    sign = "";
                }
                ResultBox.Text = "0";
                currentNum = "0";
                prevNum = "0";
            }
                ResultBox.Text = currentNum;
            if (equalPressed)
                equalPressed = false;
        }

        //==================================================================================================

        else
        {
            sign = value;
            if (isError)
                return;
            if (numPressed || inputHistory.Count == 0)
            {
                inputHistory.Add(currentNum);
                userInput.Add(currentNum);
                numPressed = false;
            }

            if (baseSigns.Contains(sign))
            {
                if (basePressed & !numPressed)
                {
                    sign = beforeAct();
                    sign = value;
                    currentNum = Act();
                    ResultBox.Text = currentNum;
                }
                else
                {
                    basePressed = true;
                }
            }
            else
            {
                beforePressed = true;
                inputHistory.Add(sign);
                userInput.Add(sign);
                currentNum = beforeAct();

                if (currentNum != "0" & !basePressed)
                    ResultBox.Text = currentNum;
            }
            PrintHistory();
            if (basePressed & !beforeSigns.Contains(sign))
            {
                HistoryBox.Text += " " + sign + " ";
            }
            prevNum = currentNum;
        }
    }

    public string Act()
    {
        var result = 0.0;
        var sign = "";
        var pNum = 0.0;
        var num = 0.0;
        var idx = 0;
        var bracketsNum = 0;
        var tempInput = userInput;

        foreach (var input in userInput)
        {
            idx++;
            if (input == "(")
            {
                bracketsAct(idx);
            }
            else if (double.TryParse(input, out double res))
            {
                if (sign != "")
                {
                    num = res;
                    result = baseAct(pNum, num, sign);
                    pNum = result;
                    sign = "";
                }
                else
                    pNum = res;

            }
            else
                sign = input;
        }
        result = pNum;
        return result.ToString();
    }

    public string bracketsAct(int idx)
    {
        var result = "";
        
        userInput.RemoveRange(0, idx);
        result = Act();


        return result;
    }

    public string beforeAct()
    {
        var result = 0.0;
        var item = "";
        var prevNum = 0.0;
        var cycles = userInput.Count;

        for (var idx = 0; idx < cycles; idx++)
        {
            item = userInput[idx];
            if (double.TryParse(item, out double res))
            {
                prevNum = res;
            }
            else
            {
                if (beforeSigns.Contains(item))
                {
                    result = soloAct(prevNum, item);
                    userInput[idx] = result.ToString();
                    userInput.RemoveAt(idx - 1);
                }
            }
        }
        return result.ToString();
    }

    public double baseAct(double a, double b, string sign)
    {
        var result = 0.0;

        if (sign == "÷" & b == 0.0)
        {
            ResultBox.Text = "Огузок,неправильно";
            isError = true;
            return 0;
        }

        switch (sign)
        {
            case " + ":
                result = a + b;
                break;
            case " - ":
                result = a - b;
                break;
            case " ÷ ":
                result = a / b;
                break;
            case " × ":
                result = a * b;
                break;
            case " ^ ":
                result = Math.Pow(a, b);
                break;
        }

        return result;
    }

    public double soloAct(double a, string sign)
    {
        double result = a;

        if (sign == "1/" & a == 0.0)
        {
            ResultBox.Text = "Огузок,неправильно";
            isError = true;
            return 0;
        }

        switch (sign)
        {
            case "sin":
                result = Math.Sin(a);
                if (a == 3.1415926535898)
                    result = 0;
                break;
            case "cos":
                result = Math.Cos(a);
                break;
            case "tg":
                result = Math.Tan(a);
                if (a == 3.1415926535898)
                    result = 0;
                break;
            case "log":
                if (a==0)
                {
                    ResultBox.Text = "Огузок,неправильно";
                    isError = true;
                    return 0;
                }
                result = Math.Log10(a);
                break;
            case "ln":
                if (a == 0)
                {
                    ResultBox.Text = "Огузок,неправильно";
                    isError = true;
                    return 0;
                }
                result = Math.Log(a);
                break;
            case "1/":
                result = 1 / a;
                break;
            case "√":
                result = Math.Sqrt(a);
                break;
            case "10 ^ ":
                result = Math.Pow(10, a);
                break;
            case "fact":
                if (int.TryParse(a.ToString(), out int res))
                    result = Factorial(Convert.ToInt32(a));
                else
                    result = double.PositiveInfinity;
                break;
            case "sqr":
                result = Math.Pow(a, 2);
                break;
            case "abs":
                result = Math.Abs(a);
                break;
            case "negate":
                if (a != 0)
                    result = a * -1;
                break;

        }

        return result;
    }
    public long Factorial(long number)
    {
        if (number == 1 || number == 0)
            return 1;
        else
            return number * Factorial(number - 1);
    }

    public void PrintHistory()
    {
        var history = new List<string>() {};
        var num = "";
        int idx = -1;
        List<int> idxToDel = new List<int>() { };
        foreach (string input in inputHistory)
        {
            idx += 1;
            if (double.TryParse(input, out double res))
            {
                num = input;
                history.Add(num);
            }
            else if (input == "(" || input == ")")
                history.Add(input);
            else
            {
                if (beforeSigns.Contains(input))
                {
                    num = input + "(" + num + ")";
                    history[history.Count - 1] = num;
                }
                else if (baseSigns.Contains(input))
                {
                    num = num + " " + input + " ";
                    history[history.Count - 1] = num;
                }

            }
        }
        HistoryBox.Text = String.Join("", history);
    }
}
