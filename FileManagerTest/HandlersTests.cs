using System;
using System.Collections.Generic;
using System.IO;
using FileManagerLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FileManagerTest
{
    [TestClass]
    public class HandlersTests
    {

        List<User> users;

        [TestInitialize]
        public void Init()
        {
            // тестовые данные пользователей, для проверки всех методов
            users = new List<User>
            {
                new User{Id=1,Name="Никитос",Age=25,Role="Админ",RegisterDate=DateTime.Now,Activity=User.Aktivnost.Онлайн},
                new User{Id=2,Name="Владос",Age=30,Role="Пользователь",RegisterDate=DateTime.Now,Activity=User.Aktivnost.Оффлайн}
            };
        }

        // ================================================================= все для json

        [TestMethod]
        public void JsonWriteReadTest()
        {
            // сделали обработчик json, для теста чтения и записи файла
            var handler = new JsonHandler<User>();

            // записали данные в файл
            handler.Write("test.json", users);

            // прочитали данные обратно
            var result = handler.Read("test.json");

            // проверили что количество пользователей совпадает
            Assert.AreEqual(2, result.Count);
        }

        [TestMethod]
        public void JsonFileNotExistTest()
        {
            // сделали проверку чтения несуществующего файла
            var handler = new JsonHandler<User>();

            var result = handler.Read("no_file.json");

            // ожидаем пустой список
            Assert.AreEqual(0, result.Count);
        }

        // ========================================================================= все для xml

        [TestMethod]
        public void XmlWriteReadTest()
        {
            // сделали обработчик xml, для теста чтения и записи
            var handler = new XmlHandler<User>();

            handler.Write("test.xml", users);

            var result = handler.Read("test.xml");

            // проверяем что данные загрузились
            Assert.AreEqual(2, result.Count);
        }

        [TestMethod]
        public void XmlFileNotExistTest()
        {
            // сделали проверку xml если файл отсутствует
            var handler = new XmlHandler<User>();

            var result = handler.Read("no_file.xml");

            Assert.AreEqual(0, result.Count);
        }

        // ================================================= все для csv

        [TestMethod]
        public void CsvWriteReadTest()
        {
            // обработчик для тестирования записи и чтения
            var handler = new CsvHandler();

            handler.Write("test.csv", users);

            var result = handler.Read("test.csv");

            Assert.AreEqual(2, result.Count);
        }

        [TestMethod]
        public void CsvFileNotExistTest()
        {
            // сделали проверку csv при отсутствии файла
            var handler = new CsvHandler();

            var result = handler.Read("no_file.csv");

            Assert.AreEqual(0, result.Count);
        }

        // ========================================= все для yaml

        [TestMethod]
        public void YamlWriteReadTest()
        {
            // обработчик для теста сохранения и загрузки
            var handler = new YamlHandler<User>();

            handler.Write("test.yaml", users);

            var result = handler.Read("test.yaml");

            Assert.AreEqual(2, result.Count);
        }

        [TestMethod]
        public void YamlFileNotExistTest()
        {
            // сделали проверку yaml если файл не найден
            var handler = new YamlHandler<User>();

            var result = handler.Read("no_file.yaml");

            Assert.AreEqual(0, result.Count);
        }

        // ================================= для пользователей

        [TestMethod]
        public void AddUserTest()
        {
            // сделали менеджер пользователей для проверки добавления
            var manager = new UserManager();

            manager.Add(users[0]);

            // проверили что пользователь добавился
            Assert.AreEqual(1, manager.Users.Count);
        }

        [TestMethod]
        public void DeleteUserTest()
        {
            // сделали проверку удаления пользователя
            var manager = new UserManager();

            manager.Add(users[0]);

            manager.Delete(1);

            // проверили что список пустой
            Assert.AreEqual(0, manager.Users.Count);
        }

        [TestMethod]
        public void SortByNameTest()
        {
            // сделали проверку сортировки по имени
            var manager = new UserManager();

            manager.Add(users[1]);
            manager.Add(users[0]);

            manager.SortByName();

            // проверяем что первый элемент Alice
            Assert.AreEqual("Владос", manager.Users[0].Name);
        }

        [TestMethod]
        public void SortByAgeTest()
        {
            // сделали проверку сортировки по возрасту
            var manager = new UserManager();

            manager.Add(users[0]);
            manager.Add(users[1]);

            manager.SortByAge();

            Assert.AreEqual(25, manager.Users[0].Age);
        }

        [TestMethod]
        public void SearchUserTest()
        {
            // сделали проверку поиска по имени
            var manager = new UserManager();

            manager.Add(users[0]);
            manager.Add(users[1]);

            var result = manager.Search("Вла");

            // ожидаем что найдется один пользователь
            Assert.AreEqual(1, result.Count);
        }

        [TestMethod]
        public void EditUserTest()
        {
            // сделали проверку редактирования пользователя
            var manager = new UserManager();

            manager.Add(users[0]);

            manager.Edit(1, "NewName");

            // проверяем что имя изменилось
            Assert.AreEqual("NewName", manager.Users[0].Name);
        }

        [TestMethod]
        public void DeleteNonExistingUserTest()
        {
            // сделали проверку удаления несуществующего пользователя
            var manager = new UserManager();

            manager.Add(users[0]);

            manager.Delete(99);

            // список должен остаться без изменений
            Assert.AreEqual(1, manager.Users.Count);
        }
    }
}
