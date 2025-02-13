// логин библиотекаря - lib1
// логин читателя - usr1

using System.Globalization;
using System.Reflection;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace P323
{
    public abstract class Role
    {
        protected string Name { get; set; }

        public Role() { }

        public abstract string GetName();

        public abstract void ViewBooks(Dictionary<int, Book> allBooks);
    }

    public class Librarian : Role
    {
        public Librarian(string name)
        {
            Name = name;
        }

        public override string GetName()
        {
            return Name;
        }

        public override void ViewBooks(Dictionary<int, Book> allBooks)
        {
            Console.WriteLine("Все книги:");
            Console.WriteLine("");
            foreach (var book in allBooks.Values)
            {
                book.SelfPrint("lib");
            }
            Console.WriteLine("");
            Console.WriteLine("Нажмите Enter, чтобы продолжить");
            Console.ReadLine();
            Console.WriteLine("------------------------------------------");
        }
        public Book AddBook(int id)
        {
            while (true)
            {
                Console.WriteLine("Впишите название книги: ");
                string title = Console.ReadLine().Trim();
                Console.WriteLine("Впишите автора книги: ");
                string author = Console.ReadLine().Trim();

                if (!string.IsNullOrEmpty(title) & !string.IsNullOrEmpty(author))
                {
                    title = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(title.ToLower());
                    author = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(author.ToLower());
                    return new Book(id, title, author);
                }

                Console.WriteLine("Название книги или имя автора отсутствуют");
                Console.WriteLine("------------------------------------------");
            }
        }

        public Dictionary<int, Book> DeleteBook(Dictionary<int, Book> allBooks)
        {
            while(true)
            {
                Console.WriteLine("Введите id книги, которую вы хотите удалить. ");
                Console.WriteLine("");
                var availableIDs = new List<int>{};
                foreach (var book in allBooks.Values)
                {
                    if (book.Status == "Не выдана")
                    {
                        book.SelfPrint("lib");
                        availableIDs.Add(book.ID);
                    }

                }
                
                var id = Console.ReadLine();
                if (int.TryParse(id, out int res))
                {
                    if (availableIDs.Contains(res))
                    {
                        if (allBooks.Remove(res))
                        {   
                            Console.WriteLine($"Книга удалена");
                            Console.WriteLine("");
                            Console.WriteLine("Нажмите Enter, чтобы продолжить");
                            Console.ReadLine();
                            Console.WriteLine("------------------------------------------");
                            return allBooks;
                        }
                        else Console.WriteLine("Индекс не найден");
                        Console.WriteLine("");
                        Console.WriteLine("Нажмите Enter, чтобы продолжить");
                        Console.ReadLine();
                        Console.WriteLine("------------------------------------------");
                    }
                    Console.WriteLine("Данная книга выдана");
                    Console.WriteLine("");
                    Console.WriteLine("Нажмите Enter, чтобы продолжить");
                    Console.ReadLine();
                    Console.WriteLine("------------------------------------------");
                }
                else Console.WriteLine("Введенного индекса нет");
            }
        }

        public Dictionary<string, Reader> RegReader(Dictionary<string, Reader> readLogins)
        {
            while(true)
            {
                Console.WriteLine("Напишите логин нового читателя: ");
                var login = Console.ReadLine();
                foreach (var log in readLogins.Keys)
                {
                    if (login == log)
                    {
                        Console.WriteLine("Данный логин уже занят");
                        Console.WriteLine("");
                        continue;
                    }
                }
                Console.WriteLine("Напишите имя нового читателя: ");
                var name = Console.ReadLine();

                readLogins.Add(login, new Reader(name, []));

                return readLogins;
            }
        }

        public void ViewUsers(Dictionary<string, Reader> readLogins, Dictionary<string, Librarian> librLogins)
        {
            Console.WriteLine("Все читатели:");
            Console.WriteLine("");
            foreach (Reader reader in readLogins.Values)
            {
                Console.WriteLine(reader.GetName());
            }
            Console.WriteLine("");
            Console.WriteLine("Все библиотекари:");
            Console.WriteLine("");
            foreach (Librarian librarian in librLogins.Values)
            {
                Console.WriteLine(librarian.Name);
            }
            Console.WriteLine("");
            Console.WriteLine("Нажмите Enter, чтобы продолжить");
            Console.ReadLine();
        }
    }

    public class Reader : Role
    {
        public List<int> TakenBooks { get; set; }

        public Reader (string name, List<int> takenBooks)
        {
            Name = name;
            TakenBooks = takenBooks;
        }

        public override string GetName()
        {
            return Name;
        }

        public override void ViewBooks(Dictionary<int, Book> allBooks)
        {
            Console.WriteLine("Все доступные книги:");
            Console.WriteLine("");
            foreach (var book in allBooks.Values)
            {
                if (book.Status == "Не выдана")
                    book.SelfPrint("usr");
            }
            Console.WriteLine("");
            Console.WriteLine("Нажмите Enter, чтобы продолжить");
            Console.ReadLine();
            Console.WriteLine("------------------------------------------");
        }

        public Dictionary<int, Book> TakeBook(Dictionary<int, Book> allBooks)
        {
            while (true)
            {
                Console.WriteLine("Введите id книги, которую хотите взять: ");
                Console.WriteLine("");
                var availableIDs = new List<int>{};
                foreach (var book in allBooks.Values)
                {
                    book.SelfPrint("usr");
                    availableIDs.Add(book.ID);
                }

                var bookID = Console.ReadLine();
                if (int.TryParse(bookID, out int res) & availableIDs.Contains(res))
                    if (allBooks[res].Status == "Не выдана")
                    {
                        Console.WriteLine($"Вы взяли книгу '{allBooks[res].Title}', не забудьте ее вернуть.");
                        Console.WriteLine("");
                        Console.WriteLine("Нажмите Enter, чтобы продолжить");
                        Console.ReadLine();
                        Console.WriteLine("------------------------------------------");

                        TakenBooks.Add(res);
                        allBooks[res].Status = "Выдана";
                        return allBooks;
                    }
                    else
                    {
                        Console.WriteLine("Данная книги уже выдана");
                        Console.WriteLine("");
                        Console.WriteLine("Нажмите Enter, чтобы продолжить");
                        Console.ReadLine();
                        Console.WriteLine("------------------------------------------");
                    }
                else 
                {
                    Console.WriteLine("Данной книги нет в каталоге");
                    Console.WriteLine("");
                    Console.WriteLine("Нажмите Enter, чтобы продолжить");
                    Console.ReadLine();
                    Console.WriteLine("------------------------------------------");
                }
            }
        }

        public Dictionary<int, Book> ReturnBook(Dictionary<int, Book> allBooks)
        {
            while (true)
            {
                Console.WriteLine("Введите id книги, которую хотите вернуть:");
                Console.WriteLine("");
                var availableIDs = new List<int>{};

                if (TakenBooks.Count == 0)
                {
                    Console.WriteLine("У вас нет взятых книг");
                    Console.WriteLine("");
                    Console.WriteLine("Нажмите Enter, чтобы продолжить");
                    Console.ReadLine();
                    Console.WriteLine("------------------------------------------");
                    return allBooks;
                }

                foreach (var book in TakenBooks)
                {
                    allBooks[book].SelfPrint("usr");
                    availableIDs.Add(allBooks[book].ID);
                }
                
                var bookID = Console.ReadLine();
                if (int.TryParse(bookID, out int res) & availableIDs.Contains(res))
                {
                    Console.WriteLine($"Вы вернули книгу '{allBooks[res].Title}'.");
                    Console.WriteLine("");
                    Console.WriteLine("Нажмите Enter, чтобы продолжить");
                    Console.ReadLine();
                    Console.WriteLine("------------------------------------------");

                    TakenBooks.Remove(res);
                    allBooks[res].Status = "Не выдана";
                    return allBooks;
                }
                else 
                {
                    Console.WriteLine("Данной книги у вас нет");
                    Console.WriteLine("");
                    Console.WriteLine("Нажмите Enter, чтобы продолжить");
                    Console.ReadLine();
                    Console.WriteLine("------------------------------------------");
                }
            }
        }

        public void ViewTakenBooks(Dictionary<int, Book> allBooks)
        {
            Console.WriteLine("Все взятые книги: ");
            Console.WriteLine("");
            foreach (var book in TakenBooks)
            {
                allBooks[book].SelfPrint("usr");
            }
            Console.WriteLine("");
            Console.WriteLine("Нажмите Enter, чтобы продолжить");
            Console.ReadLine();
            Console.WriteLine("------------------------------------------");
        }
    }

    public class Book
    {

        public int ID { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Status { get; set; } = "Не выдана";

        public Book(int id, string title, string author)
        {
            ID = id;
            Title = title;
            Author = author;
        }

        public void SelfPrint(string role)
        {
            if (role == "lib")
                Console.WriteLine($"{ID}: Книга '{Title}' автора {Author}. {Status}");
            else 
                Console.WriteLine($"{ID}: Книга '{Title}' автора {Author}.");
        }
    }

    internal class Program
    {
        static void Main()
        {
            var allBooks = ReadBooks();
            var id = allBooks.Last().Key + 1;

            var librLogins = ReadLibrarian();
            var readLogins = ReadReaders();

            string login = "";


            while (true)
            {
                Console.WriteLine("Введите логин или 0, чтобы выйти: ");
                login = Console.ReadLine();
                bool go = true;

                if (readLogins.ContainsKey(login))
                {
                    var usr = readLogins.GetValueOrDefault(login);
                    while (go)
                    {
                        Console.WriteLine($"Здравствуйте, читатель {usr.GetName()}.");
                        Console.WriteLine("Выберете действие: ");
                        Console.WriteLine("1 - Взять кингу");
                        Console.WriteLine("2 - Вернуть книгу");
                        Console.WriteLine("3 - Посмотреть все доступные книги");
                        Console.WriteLine("4 - Посмотреть все взятые книги");
                        Console.WriteLine("0 - Выход");

                        var choice = Console.ReadLine();
                        switch (choice)
                        {
                            default:
                                Console.WriteLine("Ошибка");
                                break;
                            case "0": 
                                go = false;
                                Console.WriteLine("Выход");
                                break;
                            case "1": // взять книгу
                                allBooks = usr.TakeBook(allBooks);
                                break;
                            case "2": // вернуть книгу
                                allBooks = usr.ReturnBook(allBooks);
                                break;
                            case "3": // доступные книги
                                usr.ViewBooks(allBooks);
                                break;
                            case "4": // взятые книги
                                usr.ViewTakenBooks(allBooks);
                                break;
                        }
                    }
                }   
                else if (librLogins.ContainsKey(login))
                {
                    while (go)
                    {
                        var usr = librLogins.GetValueOrDefault(login);
                        Console.WriteLine($"Здравствуйте, библиотекарь {usr.GetName()}.");
                        Console.WriteLine("Выберете действие: ");
                        Console.WriteLine("1 - Добавить книгу");
                        Console.WriteLine("2 - Удалить книгу");
                        Console.WriteLine("3 - Посмотреть все книги");
                        Console.WriteLine("4 - Посмотреть всех пользователей");
                        Console.WriteLine("5 - Зарегистрировать нового читателя");
                        Console.WriteLine("0 - Выход");

                        var choice = Console.ReadLine();
                        switch (choice)
                        {
                            default:
                                Console.WriteLine("Ошибка");
                                break;
                            case "0":
                                go = false;
                                Console.WriteLine("Выход");
                                break;
                            case "1": // добавить книгу
                                var tempBook = usr.AddBook(id);
                                allBooks[id] = tempBook;
                                id += 1;
                                break;
                            case "2": // удалить книгу
                                allBooks = usr.DeleteBook(allBooks);
                                break;
                            case "3": // все книги
                                usr.ViewBooks(allBooks);
                                break;
                            case "4": // все пользователи
                                usr.ViewUsers(readLogins, librLogins);
                                break;
                            case "5": // регистрация нового
                                usr.RegReader(readLogins);
                                break;
                        }
                    }
                }
                else if (login == "0")
                {
                    Console.WriteLine("Выход...");
                    SaveData(allBooks, readLogins, librLogins);
                    break;                
                }
                else
                {
                    Console.WriteLine("Ошибка");
                }
            }
        }

        static void SaveData(Dictionary<int, Book> allBooks, Dictionary<string, Reader> readLogins, Dictionary<string, Librarian> librLogins)
        {
            var fileName = "data.txt";

            using (StreamWriter writer = new StreamWriter(fileName))
            {
                writer.WriteLine("allBooks:");
                foreach (var book in allBooks)
                {
                    writer.WriteLine($"(B): {book.Key}: {book.Value.Title}: {book.Value.Author}: {book.Value.Status}");
                }

                writer.WriteLine("readLogins:");
                
                foreach (var read in readLogins)
                {
                    writer.WriteLine($"(R): {read.Key}: {read.Value.GetName()}: {string.Join(",", read.Value.TakenBooks)}");
                }

                writer.WriteLine("librLogins:");
                foreach (var libr in librLogins)
                {
                    writer.WriteLine($"(L): {libr.Key}: {libr.Value.GetName()}");
                }
            }
        }

        static Dictionary<string, Reader> ReadReaders()
        {
            string fileName = "data.txt";
            var readLogins = new Dictionary<string, Reader>{};
            
            List<string> readLines = File.ReadLines(fileName).Where(line => line.StartsWith("(R)")).ToList();
            var temp = new List<int>{};

            foreach (var line in readLines)
            {
                string[] data = line.Split(": ");

                if (data[3] != "")
                    temp = data[3].Split(", ").Select(int.Parse).ToList();
                else 
                    temp = [];

                readLogins[data[1]] = new Reader(data[2], temp);
            }

            return readLogins;
        }

        static Dictionary<string, Librarian> ReadLibrarian()
        {
            string fileName = "data.txt";
            var librLogins = new Dictionary<string, Librarian>{};
            
            List<string> readLines = File.ReadLines(fileName).Where(line => line.StartsWith("(L)")).ToList();

            foreach (var line in readLines)
            {
                string[] data = line.Split(": ");

                librLogins[data[1]] = new Librarian(data[2]);
            }

            return librLogins;
        }

        static Dictionary<int, Book> ReadBooks()
        {
            string fileName = "data.txt";
            var allBooks = new Dictionary<int, Book>{};
            
            List<string> readLines = File.ReadLines(fileName).Where(line => line.StartsWith("(B)")).ToList();

            foreach (var line in readLines)
            {
                string[] data = line.Split(": ");

                var temp = int.Parse(data[1]);

                allBooks[temp] = new Book(temp, data[2], data[3]);
                allBooks[temp].Status = data[4];
            }

            return allBooks;
        }
    }
}