using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Newtonsoft.Json;
using YamlDotNet.Serialization;

namespace FileManagerLib
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public string Role { get; set; }
        public DateTime RegisterDate { get; set; }

        public Aktivnost Activity { get; set; }

        public enum Aktivnost
        {
            Онлайн,
            Оффлайн
        }

        public override string ToString()
        {
            return $"{Id} | {Name} | {Age} | {Role} | {RegisterDate:yyyy-MM-dd} | {Activity}";
        }
    }

    public interface IFileHandler<T>
    {
        List<T> Read(string filename);
        void Write(string filename, List<T> data);
    }

    // json
    public class JsonHandler<T> : IFileHandler<T>
    {
        public List<T> Read(string filename)
        {
            if (!File.Exists(filename))
                return new List<T>();

            string json = File.ReadAllText(filename);
            return JsonConvert.DeserializeObject<List<T>>(json);
        }

        public void Write(string filename, List<T> data)
        {
            string json = JsonConvert.SerializeObject(data, Formatting.Indented);
            File.WriteAllText(filename, json);
        }
    }

    // xml
    public class XmlHandler<T> : IFileHandler<T>
    {
        public List<T> Read(string filename)
        {
            if (!File.Exists(filename))
                return new List<T>();

            XmlSerializer serializer = new XmlSerializer(typeof(List<T>));

            using FileStream fs = new FileStream(filename, FileMode.Open);

            return (List<T>)serializer.Deserialize(fs);
        }

        public void Write(string filename, List<T> data)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<T>));

            using FileStream fs = new FileStream(filename, FileMode.Create);

            serializer.Serialize(fs, data);
        }
    }

    // csv
    public class CsvHandler : IFileHandler<User>
    {
        public List<User> Read(string filename)
        {
            List<User> list = new List<User>();

            if (!File.Exists(filename))
                return list;

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
                    Activity = Enum.Parse<User.Aktivnost>(p[5])
                });
            }

            return list;
        }

        public void Write(string filename, List<User> data)
        {
            var lines = data.Select(x =>
                $"{x.Id},{x.Name},{x.Age},{x.Role},{x.RegisterDate},{x.Activity}");

            File.WriteAllLines(filename, lines);
        }
    }

    // yaml
    public class YamlHandler<T> : IFileHandler<T>
    {
        public List<T> Read(string filename)
        {
            if (!File.Exists(filename))
                return new List<T>();

            var deserializer = new DeserializerBuilder().Build();

            var text = File.ReadAllText(filename);

            return deserializer.Deserialize<List<T>>(text);
        }

        public void Write(string filename, List<T> data)
        {
            var serializer = new SerializerBuilder().Build();

            var text = serializer.Serialize(data);

            File.WriteAllText(filename, text);
        }
    }

    // логгер
    public static class Logger
    {
        private static string logFile = "errors.log";

        public static void Log(string message)
        {
            File.AppendAllText(logFile, $"{DateTime.Now}: {message}\n");
        }
    }

    // менеджер пользователей
    public class UserManager
    {
        public List<User> Users = new List<User>();

        public void Add(User user)
        {
            Users.Add(user);
        }

        public void Delete(int id)
        {
            Users.RemoveAll(x => x.Id == id);
        }

        public void SortByName()
        {
            Users = Users.OrderBy(x => x.Name).ToList();
        }

        public void SortByAge()
        {
            Users = Users.OrderBy(x => x.Age).ToList();
        }

        public List<User> Search(string name)
        {
            return Users.Where(x => x.Name.Contains(name)).ToList();
        }

        public void Edit(int id, string name)
        {
            var user = Users.FirstOrDefault(x => x.Id == id);

            if (user != null)
                user.Name = name;
        }
    }
}
