using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PR3Calc;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    List<string> inputHistory = new List<string>() { };
    List<string> userInput = new List<string>() {};
    string currentNum = "0";
    string sign = "";
    string actPressed = "";
    bool equalPressed = false;
    bool isError = false;
    bool isBrackets = false;
    string[] nums = { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", ".", "(", ")" };
    string[] constNums = { "3.1415926535898", "2.7182818284590" };
    string[] baseSigns = { " + ", " - ", " × ", " ÷ ", " ^ ", "" };
    string[] beforeSigns = { "sin", "cos", "tg", "log", "ln", "1/", "10 ^ ", "√", "sqr", "fact", "abs", "+/-", "!" };

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        var button = sender as Button;
        var value = button.Tag.ToString();

        if (nums.Contains(value) || constNums.Contains(value) || value == "del")
        {
            if (value == "del" & actPressed == "" & !constNums.Contains(currentNum))
            {
                if (isError)
                {
                    currentNum = "0";
                    ResultBox.Text = "0";
                    HistoryBox.Text = "";
                    userInput = new List<string>() {};
                    inputHistory = new List<string>() { };
                    return;
                }
                if (currentNum.Length == 1)
                {
                    currentNum = "0";
                    ResultBox.Text = "0";
                    return;
                }
                if (currentNum == "0")
                    return;
                if (equalPressed)
                {
                    HistoryBox.Text = "";
                    currentNum = ResultBox.Text;
                    userInput = new List<string>() { };
                    inputHistory = new List<string>() { };
                    return;
                }
                currentNum = currentNum.Substring(0, currentNum.Length - 1);
                ResultBox.Text = currentNum;
                return;
            }
            if (constNums.Contains(value) || actPressed != "")
            {
                currentNum = value;
                ResultBox.Text = currentNum;
                actPressed = "";
                return;
            }
            if (baseSigns.Contains(actPressed))
            {
                if (currentNum == "0" || actPressed != "")
                    currentNum = value;
                else
                {
                    if (currentNum.Length < 15)
                        currentNum += value;
                }
                ResultBox.Text = currentNum;
                actPressed = "";
                return;
            }
        }
        else
        {
            if (value == "CE")
            {
                if (ResultBox.Text == "0" || equalPressed)
                {
                    HistoryBox.Text = "";
                    userInput = new List<string>() { };
                    inputHistory = new List<string>() { };
                }
                ResultBox.Text = "0";
                currentNum = "0";
                actPressed = "";
                return;
            }

            if (isError)
            {
                actPressed = "";
                return;
            }

            if (!beforeSigns.Contains(actPressed))
            {
                userInput.Add(currentNum);
                inputHistory.Add(currentNum);
            }
            if (value == "=")
            {
                equalPressed = true;
                ResultBox.Text = Act();
                PrintHistory();
                HistoryBox.Text += " =";
                actPressed = "";
                return;
            }
            else if (baseSigns.Contains(value))
            {
                if (currentNum != "0" || beforeSigns.Contains(actPressed))
                {
                    if (beforeSigns.Contains(actPressed))
                    {
                        userInput.Add(value);
                        inputHistory.Add(value);
                        PrintHistory();
                        actPressed = value;
                        return;
                    }
                    ResultBox.Text = currentNum;
                    if (userInput.Count % 2 != 1)
                    {
                        ResultBox.Text = Act();
                        currentNum = "0";
                    }
                    userInput.Add(value);
                    inputHistory.Add(value);
                    PrintHistory();
                    actPressed = value;

                }
                return;
            }
            else
            {
                actPressed = value;
                userInput.RemoveAt(userInput.Count-1);
                userInput.Add(value);
                userInput.Add(currentNum);
                inputHistory.RemoveAt(inputHistory.Count - 1);
                inputHistory.Add(value);
                inputHistory.Add(currentNum);
                PrintHistory();
                ResultBox.Text = Act();
                currentNum = ResultBox.Text;
                return;
            }
        }
    }

    public string Act()
    {
        if (userInput.Count == 1)
            return userInput[0].ToString();
        double result = 0;
        Boolean isBase = false;
        Boolean isBefore = false;
        double a = 0;
        double b = 0;
        var idx = -1;
        string sign = "sign";

        foreach (string input in userInput)
        {
            idx += 1;
            if (isError)
                return "Нельзя делить на 0";
            if (double.TryParse(input, out double res))
            {
                if (baseSigns.Contains(sign))
                    isBase = true;
                else if (beforeSigns.Contains(sign))
                    isBefore = true;
                if (isBase)
                {
                    a = res;
                    result = baseAct(b, a, sign);
                    b = result;
                    isBase = false;
                    continue;
                }
                else if (isBefore)
                {
                    b = res;
                    result = soloAct(b, sign);
                    break;
                }
                else
                    b = res;
            }
            else
                sign = input;
        }
        if (isBefore)
        {
            userInput[idx] = result.ToString();
            userInput.RemoveAt(idx - 1);
            result = Convert.ToDouble(Act());
        }

        return result.ToString();
    }

    public double baseAct(double a, double b, string sign)
    {
        var result = 0.0;

        if (sign == "÷" & b == 0.0)
        {
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
        double result = 0.0;

        if (sign == "1/" & a == 0.0)
        {
            isError = true;
            return 0;
        }

        switch (sign)
        {
            case "sin":
                result = Math.Sin(a);
                break;
            case "cos":
                result = Math.Cos(a);
                break;
            case "tg":
                result = Math.Tan(a);
                break;
            case "log":
                result = Math.Log10(a);
                break;
            case "ln":
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
                result = Factorial(Convert.ToInt32(a));
                break;
            case "sqr":
                result = Math.Pow(a, 2);
                break;
            case "abs":
                result = Math.Abs(a);
                break;
            case "+/-":
                result = a * -1;
                break;

        }

        return result;
    }
    public int Factorial(int n)
    {
        if (n == 1 || n == 0)
            return 1;

        return n * Factorial(n - 1);
    }

    public void PrintHistory()
    {
        var history = "";
        var isBefore = false;
        foreach(string input in inputHistory)
        {
            if (isBefore)
            {
                isBefore = false;
                history += "(" + input + ")";
                continue;
            }
            if (beforeSigns.Contains(input))
                isBefore = true;
            history += input;
        }  
        HistoryBox.Text = history;
    }
}
