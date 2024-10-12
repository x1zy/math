using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using System;

namespace MyApp;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    public void MainButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        try
        {
            // Получаем значения из TextBox'ов
            double a = Convert.ToDouble(Coefficient_3.Text);
            double b = Convert.ToDouble(First_Coefficient.Text);
            double c = Convert.ToDouble(Coefficient_x.Text);
            double d = Convert.ToDouble(Constant.Text);
            double A = Convert.ToDouble(ValueA.Text);
            double B = Convert.ToDouble(ValueB.Text);
            double E = Convert.ToDouble(Precision.Text);

            // Создаем экземпляр EquationSolver
            EquationSolver solver = new EquationSolver(a, b, c, d, A, B, E);

            // Очищаем StackPanel для новых шагов
            Steps_Solve.Children.Clear();

            // Запускаем решение уравнения с обновлением шагов
            solver.MainSolution(UpdateStepsUI);
        }
        catch (FormatException)
        {
            AppendStep("Ошибка: Неверный формат данных. Убедитесь, что все поля заполнены корректно.");
        }
        catch (Exception ex)
        {
            AppendStep($"Произошла ошибка: {ex.Message}");
        }
    }

    // Метод для обновления шагов на UI
    private void UpdateStepsUI(int stepNumber, double A, double B, double xi, double f_xi, double E)
    {
        // Округляем значения до тысячных
        xi = Math.Round(xi, 3);
        f_xi = Math.Round(f_xi, 3);
        A = Math.Round(A, 3);
        B = Math.Round(B, 3);
    

        // Создаем TextBlock для каждого шага
        var stepHeader = new TextBlock
        {
            Text = $"Шаг {stepNumber}: ",
            FontSize = 32,
            FontWeight = FontWeight.Bold,
            Margin = new Thickness(0, 30, 0, 0)
        };

        var intervalText = new TextBlock
        {
            Text = $"Новый интервал ξ ∊ [{A}; {B}]",
            FontSize = 24
        };

        var approximationText = new TextBlock
        {
            Text = $"Текущее приближение ξ = {xi}",
            FontSize = 24
        };

        var functionValueText = new TextBlock
        {
            Text = $"f(ξ) = {f_xi}",
            FontSize = 24
        };

        // Добавляем шаги в StackPanel
        Steps_Solve.Children.Add(stepHeader);
        Steps_Solve.Children.Add(intervalText);
        Steps_Solve.Children.Add(approximationText);
        Steps_Solve.Children.Add(functionValueText);

        // Если это последний шаг, выводим результат в TextBlock Answer
        if (Math.Abs(B - A) < E)
        {
            Answer.Text = $"Ответ:  {xi}";
        }
    }

    // Метод для добавления сообщений об ошибках
    private void AppendStep(string message)
    {
        var errorText = new TextBlock
        {
            Text = message,
            FontSize = 24,
            Foreground = Brushes.Red,
            Margin = new Thickness(0, 10, 0, 0)
        };

        Steps_Solve.Children.Add(errorText);
    }

    // Класс для решения уравнения методом бисекции
    public class EquationSolver
    {
        private double a, b, c, d, A, B, E;

        public EquationSolver(double a, double b, double c, double d, double A, double B, double E)
        {
            this.a = a;
            this.b = b;
            this.c = c;
            this.d = d;
            this.A = A;
            this.B = B;
            this.E = E;
        }

        // Функция для вычисления значения кубического уравнения
        public double Function(double x)
        {
            return a * Math.Pow(x, 3) + b * Math.Pow(x, 2) + c * x + d;
        }

        // Метод для вычисления средней точки отрезка [A, B]
        public double Xi_counting()
        {
            return (A + B) / 2.0;
        }

        // Основной метод для решения уравнения методом бисекции
        // Основной метод для решения уравнения методом бисекции
        public void MainSolution(Action<int, double, double, double, double, double> updateSteps)
        {
            double xi;
            int step = 1;

            while (Math.Abs(B - A) > E) // Пока длина отрезка больше заданной точности
            {
                xi = Xi_counting(); // Вычисляем среднюю точку

                // Обновляем UI для текущего шага, передавая E
                updateSteps(step, A, B, xi, Function(xi), E);

                if (Function(xi) == 0.0) // Если значение функции в средней точке равно 0, то корень найден
                {
                    return;
                }
                else if (Function(A) * Function(xi) < 0) // Корень лежит в левой половине
                {
                    B = xi;
                }
                else // Корень лежит в правой половине
                {
                    A = xi;
                }

                step++; // Переход к следующему шагу
            }

            // Последний шаг после завершения цикла
            xi = Xi_counting();
            updateSteps(step, A, B, xi, Function(xi), E);
        }
    }
}