
using System.Data;
using System.Xml.Serialization;
using Newtonsoft.Json;
using YamlDotNet.Serialization;

namespace FileManagerApp
{

    
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }

        public string Role { get; set; }

        public DateTime RegisterDate { get; set; }

        
        public Aktivnost Activity { get; set; }
        public enum Aktivnost { Онлайн, Оффлайн }

       
        public override string ToString()
        {
            return $"{Id} | {Name} | {Age} | {Role} | {RegisterDate:yyyy-MM-dd} | {Activity}";
        }

    }


    // интерфейс для работы с файлами разных форматов
    public interface IFileHandler<T>
    {
        
        List<T> Read(string filename);

        void Write(string filename, List<T> data);
    }


    // =============================================== json 

    // обработчик json для чтения и записи
    public class JsonHandler<T> : IFileHandler<T>
    {
        // читаем данные из json файла
        public List<T> Read(string filename)
        {
            try
            {
                if (!File.Exists(filename))
                    throw new FileNotFoundException("Файл не найден");

                string json = File.ReadAllText(filename);

                return JsonConvert.DeserializeObject<List<T>>(json);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка JSON: " + ex.Message);
                Logger.Log(ex.ToString());
                return new List<T>();
            }
        }

        // записываем данные в json файл
        public void Write(string filename, List<T> data)
        {
            try
            {
                string json = JsonConvert.SerializeObject(data, Newtonsoft.Json.Formatting.Indented);
                File.WriteAllText(filename, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка записи JSON: " + ex.Message);
                Logger.Log(ex.ToString());
            }
        }
    }



    // =================================================== xml 

    // сделали обработчик xml формата
    public class XmlHandler<T> : IFileHandler<T>
    {
        // читаем данные из xml
        public List<T> Read(string filename)
        {
            try
            {
                if (!File.Exists(filename))
                    throw new Exception("Файл не найден");

                XmlSerializer serializer = new XmlSerializer(typeof(List<T>));

                using (FileStream fs = new FileStream(filename, FileMode.Open))
                {
                    return (List<T>)serializer.Deserialize(fs);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка XML: " + ex.Message);
                Logger.Log(ex.ToString());
                return new List<T>();
            }
        }

        // записываем данные в xml файл
        public void Write(string filename, List<T> data)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<T>));

                using (FileStream fs = new FileStream(filename, FileMode.Create))
                {
                    serializer.Serialize(fs, data);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка записи XML: " + ex.Message);
            }
        }
    }


    // =================================================== csv 

    // сделали обработчик csv формата
    public class CsvHandler : IFileHandler<User>
    {
        // читаем строки csv и преобразуем в пользователей
        public List<User> Read(string filename)
        {
            List<User> list = new List<User>();

            try
            {
                if (!File.Exists(filename))
                    throw new Exception("Файл не найден");

                var lines = File.ReadAllLines(filename);

                foreach (var line in lines)
                {
                    var p = line.Split(',');

                    list.Add(new User
                    {
                        Id = int.Parse(p[0]),
                        Name = p[1],
                        Age = int.Parse(p[2]),
                        Role = p[3],
                        RegisterDate = DateTime.Parse(p[4]),
                        Activity = (User.Aktivnost)Enum.Parse(typeof(User.Aktivnost), p[5])
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка CSV: " + ex.Message);
                Logger.Log(ex.ToString());
            }

            return list;
        }

        // записываем список пользователей в csv файл
        public void Write(string filename, List<User> data)
        {
            try
            {
                var lines = data.Select(x => $"{x.Id},{x.Name},{x.Age},{x.Role},{x.RegisterDate},{x.Activity}");

                File.WriteAllLines(filename, lines);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка записи CSV: " + ex.Message);
            }
        }
    }


    // =================================================== yaml 

    // сделали обработчик формата
    public class YamlHandler<T> : IFileHandler<T>
    {
        // читаем файл
        public List<T> Read(string filename)
        {
            try
            {
                if (!File.Exists(filename))
                    throw new Exception("Файл не найден");

                var deserializer = new DeserializerBuilder().Build();
                var text = File.ReadAllText(filename);

                return deserializer.Deserialize<List<T>>(text);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка YAML: " + ex.Message);
                Logger.Log(ex.ToString());
                return new List<T>();
            }

        }

        // записываем данные в yaml
        public void Write(string filename, List<T> data)
        {
            try
            {
                var serializer = new SerializerBuilder().Build();
                var text = serializer.Serialize(data);

                File.WriteAllText(filename, text);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка записи YAML: " + ex.Message);
            }
        }
    }





    class Program
    {

        // список пользователей в памяти программы
        static List<User> users = new List<User>();


        
        static void Main()
        {
            Console.WriteLine(Directory.GetCurrentDirectory());

            while (true)
            {
                Console.WriteLine("\n====== МЕНЮ ======");
                Console.WriteLine("1 Загрузить из файла");
                Console.WriteLine("2 Сохранить в файл");
                Console.WriteLine("3 Показать данные");
                Console.WriteLine("4 Сортировать по имени");
                Console.WriteLine("5 Сортировать по возрасту");
                Console.WriteLine("6 Поиск по имени");
                Console.WriteLine("7 Добавить пользователя");
                Console.WriteLine("8 Удалить пользователя");
                Console.WriteLine("9 Изменить пользователя");
                Console.WriteLine("0 Выход");

                Console.Write("Выберите действие: ");

                
                if (!int.TryParse(Console.ReadLine(), out int choice))
                {
                    Console.WriteLine("Неверный ввод! Нужно ввести число.");
                    continue;
                }

                
                switch (choice)
                {
                    case 1:
                        Load();
                        break;

                    case 2:
                        Save();
                        break;

                    case 3:
                        Show();
                        break;

                    case 4:
                        Sort();
                        break;

                    case 5:
                        Sort1();
                        break;

                    case 6:
                        Search();
                        break;

                    case 7:
                        Add();
                        break;

                    case 8:
                        Delete();
                        break;

                    case 9:
                        Edit();
                        break;

                    case 0:
                        return;

                    default:
                        Console.WriteLine("Мимо");
                        break;
                }
            }
        }



        // загрузка данных из файла
        static void Load()
        {
            Console.WriteLine("Формат: json / xml / csv / yaml");
            string type = Console.ReadLine();

            Console.Write("Имя файла: ");
            string file = Console.ReadLine();

            // выбираем обработчик по типу файла
            switch (type)
            {
                case "json":
                    users = new JsonHandler<User>().Read(file);
                    break;

                case "xml":
                    users = new XmlHandler<User>().Read(file);
                    break;

                case "csv":
                    users = new CsvHandler().Read(file);
                    break;

                case "yaml":
                    users = new YamlHandler<User>().Read(file);
                    break;
            }
        }


        // сохранение данных в файл
        static void Save()
        {
            Console.WriteLine("Формат: json / xml / csv / yaml");
            string type = Console.ReadLine();

            Console.Write("Имя файла: *.формат\n");
            string file = Console.ReadLine();

            switch (type)
            {
                case "json":
                    new JsonHandler<User>().Write(file, users);
                    break;

                case "xml":
                    new XmlHandler<User>().Write(file, users);
                    break;

                case "csv":
                    new CsvHandler().Write(file, users);
                    break;

                case "yaml":
                    new YamlHandler<User>().Write(file, users);
                    break;
            }
        }


        // вывод всех пользователей
        static void Show()
        {
            foreach (var u in users)
                Console.WriteLine(u);
        }


        // сортировка пользователей по имени
        static void Sort()
        {
            users = users.OrderBy(x => x.Name).ToList();
        }

        // сортировка пользователей по возрасту
        static void Sort1()
        {
            users = users.OrderBy(x => x.Age).ToList();
        }

        // поиск пользователей по имени
        static void Search()
        {
            Console.Write("Введите имя: ");
            string name = Console.ReadLine();

            var result = users.Where(x => x.Name.Contains(name)).ToList();

            foreach (var u in result)
                Console.WriteLine(u);
        }


        // добавление нового пользователя
        static void Add()
        {
            User u = new User();

            Console.Write("Id: ");
            u.Id = int.Parse(Console.ReadLine());

            Console.Write("Name: ");
            u.Name = Console.ReadLine();

            Console.Write("Age: ");
            u.Age = int.Parse(Console.ReadLine());

            Console.Write("Роль: ");
            u.Role = Console.ReadLine();


            Console.Write("Дата регистрации (yyyy-mm-dd): ");

            DateTime date;

            // проверяем правильность ввода даты
            while (!DateTime.TryParseExact(
                Console.ReadLine(),
                "yyyy-MM-dd",
                null,
                System.Globalization.DateTimeStyles.None,
                out date))
            {
                Console.WriteLine("Неверный формат даты! Введите в формате yyyy-MM-dd");
            }

            u.RegisterDate = date;

            Console.Write("Активность (0 - Онлайн, 1 - Оффлайн): ");
            u.Activity = (User.Aktivnost)int.Parse(Console.ReadLine());

            users.Add(u);
        }


        // удаление пользователя по айди
        static void Delete()
        {
            Console.Write("Введите Id: ");
            int id = int.Parse(Console.ReadLine());

            users.RemoveAll(x => x.Id == id);
        }


        // изменение данных пользователя
        static void Edit()
        {
            Console.Write("Id пользователя: ");
            int id = int.Parse(Console.ReadLine());

            var user = users.FirstOrDefault(x => x.Id == id);

            // проверяем существует ли пользователь
            if (user == null)
            {
                Console.WriteLine("Не найден");
                return;
            }

            Console.Write("Новое имя: ");
            user.Name = Console.ReadLine();

            Console.Write("Новый возраст: ");
            user.Age = int.Parse(Console.ReadLine());

            Console.Write("Новая роль: ");
            user.Role = Console.ReadLine();

            Console.Write("Новая дата регистрации (yyyy-MM-dd): ");

            DateTime date;

            while (!DateTime.TryParseExact(
                Console.ReadLine(),
                "yyyy-MM-dd",
                null,
                System.Globalization.DateTimeStyles.None,
                out date))
            {
                Console.WriteLine("Неверный формат даты! Введите в формате yyyy-MM-dd");
            }

            user.RegisterDate = date;

            Console.Write("Активность (0 - Онлайн, 1 - Оффлайн): ");
            user.Activity = (User.Aktivnost)int.Parse(Console.ReadLine());
        }
    }
}
