namespace Parallel23._03
{
    internal class Program
    {
        private static readonly object filelock = new object();
        static void Main(string[] args)
        {
            List<int> numbers = File.ReadAllLines(MyFile())
                .AsParallel()
                .Where(line => int.TryParse(line, out _))
                .Select(int.Parse)
                .ToList();

            int unique = numbers.AsParallel().Distinct().Count();
            Console.WriteLine($"Кол-во уникальных значений - ", unique);

            List<int> max_line = InRow(numbers);
            Console.WriteLine("Сама длинная цепочка " + string.Join(", ", max_line));

            List<int> max_line_plus = InRowPlus(numbers);
            Console.WriteLine("Сама длинная цепочка plus " + string.Join(", ", max_line_plus));

            int start = 5;
            int end = 8;
            string filePath = MyFile();
            Parallel.For(start, end, i =>
            {
                string res = Generator(i);
                WriteFile(filePath, res);
            });

            Console.WriteLine("Таблица создана ;)");
        
        }
        public static void WriteFile(string filepath, string content)
        {
            try
            {
                lock (filelock)
                {
                    using (StreamWriter writer = new StreamWriter(filepath, append: true))
                    {
                        writer.Write(content);
                    }
                }
                
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public static string MyFile()
        {
            List<int> list = new List<int>{1 ,2, 3, 4, -9 ,-3, 7,0, 1, 2 };
            string desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string filePath = Path.Combine(desktop, "NumberFile.txt");

            File.WriteAllLines(filePath, list.Select(n => n.ToString()));
            return filePath;
        }
        public static List<int> InRow(List<int> list)
        {
            List<int> arr = new List<int>();    
            List<int> maxArr = new List<int>(); 
            int? prev = null;
            int count = 0;

            foreach (int i in list)
            {
                if (prev.HasValue && i > prev.Value) 
                {
                    count++;
                    arr.Add(i);

                    if (count > 2 && arr.Count > maxArr.Count)
                    {
                        maxArr = new List<int>(arr); 
                    }
                }
                else
                {
                    count = 1;
                    arr.Clear();
                    arr.Add(i); 
                }

                prev = i;
            }

            return maxArr;
        }
        public static List<int> InRowPlus(List<int> list)
        {
            List<int> arr = new List<int>();
            List<int> maxArr = new List<int>();
            int? prev = null;
            int count = 0;

            foreach (int i in list)
            {
                if(i > 0)
                {
                    if (prev.HasValue && i > prev.Value)
                    {
                        count++;
                        arr.Add(i);

                        if (count > 2 && arr.Count > maxArr.Count)
                        {
                            maxArr = new List<int>(arr);
                        }
                    }
                    else
                    {
                        count = 1;
                        arr.Clear();
                        arr.Add(i);
                    }

                    prev = i;
                }
            }
            return maxArr;
        }
        public static string Generator(int num)
        {
            string result = "";
            for(int i = 0; i < 10; i++)
            {
                result += $"{num} * {i} = {num * i}\n";
            }
            return result;
        }
    }
}
