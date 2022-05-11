double a;

int next_step = 0;
int steps = 0;

//значения для перехода на шаг RR
double f1 = 0;
double f2 = 0;
double f1_step2 = 0;
double f2_step2 = 0;

//оптимальные значения выиграшей для оппонентов
int nesh_men = 0;
int nesh_prof = 0;

//цели оппонентов 
double goal_men = 50;
double goal_prof = 100;

//границы интервала
double min = 40;
double max = 60;

//матрицы выиграшей
double[,] matrix_men = new double[2, 2];
double[,] matrix_prof = new double[2, 2];

bool show_step0 = false;

//коэффициент дисконтирования
double q = 0.9;

Console.WriteLine("Хотите задать свои цели менеджера и профсоюза? Да/Нет");
string answ = Console.ReadLine();
if (answ == "да" || answ == "Да")
{
    Console.WriteLine("Введите цель менеджера");
    goal_men = Convert.ToDouble(Console.ReadLine());
    Console.WriteLine("Введите цель профсоюза");
    goal_prof = Convert.ToDouble(Console.ReadLine());
    Console.WriteLine("Данные изменены \n");
    min = goal_men;
    max = goal_prof;
}
else
{
    Console.WriteLine("Выбраны значения по умолчанию");
    Console.WriteLine("Цель менеджера = " + goal_men);
    Console.WriteLine("Цель профсоюза = " + goal_prof + "\n");
}

Console.WriteLine("Хотите задать свой интервал? Да/Нет");
answ = Console.ReadLine();
if (answ == "да" || answ == "Да")
{
    Console.WriteLine("Рекомендуемый интервал для выбраных значений не менее [" + goal_men + ", " + goal_prof + "] \n");
    Console.WriteLine("Введите нижнюю границу интервала");
    min = Convert.ToDouble(Console.ReadLine());
    Console.WriteLine("Введите цель профсоюза");
    max = Convert.ToDouble(Console.ReadLine());
    Console.WriteLine("Данные изменены \n");
}
else
{
    Console.WriteLine("Выбраны значения по умолчанию");
    Console.WriteLine("Нижняя граница интервала = " + min);
    Console.WriteLine("Верхняя граница интервала = " + max + "\n");
}

Console.WriteLine("Хотите задать свой коэффициент дисконтирования? Да/Нет");
answ = Console.ReadLine();
if (answ == "да" || answ == "Да")
{
    Console.WriteLine("Введите коэффициент дисконтирования (в формате 0,XXXX)");
    q = Convert.ToDouble(Console.ReadLine());
    Console.WriteLine("Данные изменены \n");
}
else
{
    Console.WriteLine("Выбрано значение по умолчанию");
    Console.WriteLine("Коэффициент дисконтирования = " + q + "\n");
}

double av_val = Math.Abs(goal_prof - goal_men) * 0.5 + Math.Min(goal_prof, goal_men);
Console.WriteLine("Подсчёт функций оптимальности без учёта шага RR");

Console.WriteLine("Вывести подробные расчёты?");
answ = Console.ReadLine();
if (answ == "да" || answ == "Да")
{
    show_step0 = true;
    Console.WriteLine("\nПодробные расчёты значений равновесия Нэша с учётом отсутствия возможности перехода на второй шаг: \n");
}

double exp = max - min + 1;
for (a = min; a <= max; a++)
{
    //AA
    matrix_prof[0, 0] = av_val - Math.Abs(a - goal_prof);
    matrix_men[0, 0] = av_val - Math.Abs(a - goal_men);

    //AR
    matrix_prof[0, 1] = av_val - Math.Abs(a + 10 - goal_prof);
    matrix_men[0, 1] = av_val - Math.Abs(a + 10 - goal_men);

    //RA
    matrix_prof[1, 0] = av_val - Math.Abs(a - 10 - goal_prof);
    matrix_men[1, 0] = av_val - Math.Abs(a - 10 - goal_men);

    //RR
    //при подсчете равновесий Нэша для 1 шага игра представляется с невозможностью перехода на второй
    matrix_prof[1, 1] = 0;
    matrix_men[1, 1] = 0;

    double min_A = 999999;
    double min_R = 999999;

    double max_prof;
    double max_men;

    // оптимальность для менеджера
    for (int i = 0; i < 2; i++)
    {
        //выбирается минимум 1 строки (согласие)
        if (i == 0)
        {
            min_A = Math.Min(matrix_men[i, 0], matrix_men[i, 1]);
        }
        //выбирается минимум 2 строки (отказ)
        if (i == 1)
        {
            //всегда на предварительном шаге выбирается ненулевой элемент
            min_R = matrix_men[i, 0];
        }
        max_men = Math.Max(min_A, min_R);

        if (max_men == min_A)
        {
            nesh_men = 0;
        }
        else
        {
            nesh_men = 1;
        }
    }

    min_A = 999999;
    min_R = 999999;
    // оптимальность для профсоюза
    for (int i = 0; i < 2; i++)
    {
        //выбирается минимум 1 столбца (согласие)
        if (i == 0)
        {
            min_A = Math.Min(matrix_prof[0, i], matrix_prof[1, i]);
        }
        //выбирается минимум 2 столбца (отказ)
        if (i == 1)
        {
            //всегда на предварительном шаге выбирается ненулевой элемент
            min_R = matrix_men[i, 0];
        }
        max_prof = Math.Max(min_A, min_R);

        if (max_prof == min_A)
        {
            nesh_prof = 0;
        }
        else
        {
            nesh_prof = 1;
        }
    }

    if (nesh_men == 1 && nesh_prof == 1)
    {
        nesh_men = 0;
        nesh_prof = 0;
    }

    f1 += matrix_prof[nesh_men, nesh_prof] / exp;
    f2 += matrix_men[nesh_men, nesh_prof] / exp;

    steps++;
    if (show_step0)
    {
        Console.WriteLine("при a = " + a);
        Console.WriteLine("(" + matrix_prof[0, 0] + ", " + matrix_men[0, 0] + ")" + "  " + "(" + matrix_prof[0, 1] + ", " + matrix_men[0, 1] + ")");
        Console.WriteLine("(" + matrix_prof[1, 0] + ", " + matrix_men[1, 0] + ")" + "  " + "(" + matrix_prof[1, 1] + ", " + matrix_men[1, 1] + ")");
        Console.WriteLine("Равновесие Нэша достигается на (" + matrix_prof[nesh_men, nesh_prof] + ", " + matrix_men[nesh_men, nesh_prof] + ") \n");
    }
}

f1 *= q;
f2 *= q;

Console.WriteLine("f1 = " + f1 + " и f2 = " + f2 + " на этапе подсчёта функций оптимальности без учёта шага RR");

Console.WriteLine("\n");
Console.WriteLine("Переход на основную часть программы:");
Console.WriteLine("\n");

//переходим на основную часть программы, где есть возможность на переход на 2 шаг
steps = 0;
for (a = min; a <= max; a++)
{

    //AA
    matrix_prof[0, 0] = av_val - Math.Abs(a - goal_prof);
    matrix_men[0, 0] = av_val - Math.Abs(a - goal_men);

    //AR
    matrix_prof[0, 1] = av_val - Math.Abs(a + 10 - goal_prof);
    matrix_men[0, 1] = av_val - Math.Abs(a + 10 - goal_men);

    //RA
    matrix_prof[1, 0] = av_val - Math.Abs(a - 10 - goal_prof);
    matrix_men[1, 0] = av_val - Math.Abs(a - 10 - goal_men);

    //RR
    matrix_prof[1, 1] = f1;
    matrix_men[1, 1] = f2;

    double min_A = 999999;
    double min_R = 999999;

    double max_prof;
    double max_men;

    // оптимальность для менеджера
    for (int i = 0; i < 2; i++)
    {
        //выбирается минимум 1 строки (согласие)
        if (i == 0)
        {
            min_A = Math.Min(matrix_men[i, 0], matrix_men[i, 1]);
        }
        //выбирается минимум 2 строки (отказ)
        if (i == 1)
        {
            min_R = Math.Min(matrix_men[i, 0], matrix_men[i, 1]);
        }
        max_men = Math.Max(min_A, min_R);

        if (max_men == min_A)
        {
            nesh_men = 0;
        }
        else
        {
            nesh_men = 1;
        }
    }

    min_A = 999999;
    min_R = 999999;
    // оптимальность для профсоюза
    for (int i = 0; i < 2; i++)
    {
        //выбирается минимум 1 столбца (согласие)
        if (i == 0)
        {
            min_A = Math.Min(matrix_prof[0, i], matrix_prof[1, i]);
        }
        //выбирается минимум 2 столбца (отказ)
        if (i == 1)
        {
            min_R = Math.Min(matrix_prof[0, i], matrix_prof[1, i]);
        }
        max_prof = Math.Max(min_A, min_R);

        if (max_prof == min_A)
        {
            nesh_prof = 0;
        }
        else
        {
            nesh_prof = 1;
        }
    }

    steps++;

    Console.WriteLine("при a = " + a);
    Console.WriteLine("(" + matrix_prof[0, 0] + ", " + matrix_men[0, 0] + ")" + "  " + "(" + matrix_prof[0, 1] + ", " + matrix_men[0, 1] + ")");
    Console.WriteLine("(" + matrix_prof[1, 0] + ", " + matrix_men[1, 0] + ")" + "  " + "(" + matrix_prof[1, 1] + ", " + matrix_men[1, 1] + ")");
    Console.WriteLine("Равновесие Нэша достигается на (" + matrix_prof[nesh_men, nesh_prof] + ", " + matrix_men[nesh_men, nesh_prof] + ")");

    if (matrix_prof[nesh_men, nesh_prof] == matrix_prof[1, 1] && matrix_men[nesh_men, nesh_prof] == matrix_men[1, 1])
    {
        next_step++;
        Console.WriteLine("Игра переходит на следующий шаг");
    }

    Console.WriteLine("\n");

    f1_step2 += matrix_prof[nesh_men, nesh_prof] / exp;
    f2_step2 += matrix_men[nesh_men, nesh_prof] / exp;
}

Console.WriteLine("f1 = " + f1_step2 + " и f2 = " + f2_step2 + " при переходе игры на второй шаг");
Console.WriteLine("Игра переходит на следующий шаг в " + next_step + " из " + steps + " случаев");

