using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Globalization;

namespace HotelManagement
{
    // Перечисление типов номеров
    public enum RoomType
    {
        Single,
        Double,
        Triple
    }

    // Класс для представления номера в гостинице
    public class Room
    {
        public int Number { get; set; }
        public int Floor { get; set; }
        public RoomType Type { get; set; }
        public decimal DailyCost { get; set; }
        public List<Client> Clients { get; set; } = new();

        public int GetMaxPlaces()
        {
            return Type switch
            {
                RoomType.Single => 1,
                RoomType.Double => 2,
                RoomType.Triple => 3,
                _ => 0
            };
        }
    }

    // Класс для представления клиента
    public class Client
    {
        public string Key { get; set; } = string.Empty; // уникальный идентификатор
        public string FullName { get; set; } = string.Empty;
        public string PassportNumber { get; set; } = string.Empty;
        public string CityFrom { get; set; } = string.Empty;
        public int RoomNumber { get; set; }
        public int Place { get; set; } // 1..3
        public DateTime ArrivalDate { get; set; }
        public int PaidDays { get; set; }
    }

    // Класс для представления служащего
    public class Employee
    {
        public string Key { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public List<int> Floors { get; set; } = new(); // этажи, за которые отвечает
        public List<DayOfWeek> WorkDays { get; set; } = new();
    }

    class Program
    {
        private static List<Room> rooms = new();
        private static List<Client> clients = new();
        private static List<Employee> employees = new();

        static void Main(string[] args)
        {
            LoadData();
            
            while (true)
            {
                ShowMainMenu();
                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        HandleRequests();
                        break;
                    case "2":
                        HandleEmployeeManagement();
                        break;
                    case "3":
                        ShowInformation();
                        break;
                    case "4":
                        Console.WriteLine("Выход из программы...");
                        return;
                    default:
                        Console.WriteLine("Неверный выбор. Пожалуйста, попробуйте снова.");
                        break;
                }
            }
        }

        static void LoadData()
        {
            // Загрузка данных из файлов
            LoadRooms();
            LoadClients();
            LoadEmployees();
        }

        static void LoadRooms()
        {
            if (!File.Exists("rooms.txt"))
            {
                // Создаем тестовые данные если файл не существует
                rooms.Add(new Room { Number = 101, Floor = 1, Type = RoomType.Single, DailyCost = 2000 });
                rooms.Add(new Room { Number = 102, Floor = 1, Type = RoomType.Double, DailyCost = 3000 });
                rooms.Add(new Room { Number = 201, Floor = 2, Type = RoomType.Triple, DailyCost = 4500 });
                SaveRooms();
            }
            else
            {
                var lines = File.ReadAllLines("rooms.txt");
                foreach (var line in lines)
                {
                    var parts = line.Split('|');
                    if (parts.Length >= 4)
                    {
                        var room = new Room
                        {
                            Number = int.Parse(parts[0]),
                            Floor = int.Parse(parts[1]),
                            Type = Enum.Parse<RoomType>(parts[2]),
                            DailyCost = decimal.Parse(parts[3])
                        };
                        rooms.Add(room);
                    }
                }
            }
        }

        static void LoadClients()
        {
            if (!File.Exists("clients.txt"))
            {
                // Создаем тестовые данные если файл не существует
                clients.Add(new Client { 
                    Key = "C001", 
                    FullName = "Иванов И.И.", 
                    PassportNumber = "123456", 
                    CityFrom = "Москва", 
                    RoomNumber = 101, 
                    Place = 1, 
                    ArrivalDate = DateTime.Now.AddDays(-5), 
                    PaidDays = 5 
                });
                SaveClients();
            }
            else
            {
                var lines = File.ReadAllLines("clients.txt");
                foreach (var line in lines)
                {
                    var parts = line.Split('|');
                    if (parts.Length >= 8)
                    {
                        var client = new Client
                        {
                            Key = parts[0],
                            FullName = parts[1],
                            PassportNumber = parts[2],
                            CityFrom = parts[3],
                            RoomNumber = int.Parse(parts[4]),
                            Place = int.Parse(parts[5]),
                            ArrivalDate = DateTime.ParseExact(parts[6], "yyyy-MM-dd", CultureInfo.InvariantCulture),
                            PaidDays = int.Parse(parts[7])
                        };
                        clients.Add(client);
                    }
                }
            }
            
            // Привязываем клиентов к комнатам
            foreach (var client in clients)
            {
                var room = rooms.FirstOrDefault(r => r.Number == client.RoomNumber);
                if (room != null && !room.Clients.Any(c => c.Key == client.Key))
                {
                    room.Clients.Add(client);
                }
            }
        }

        static void LoadEmployees()
        {
            if (!File.Exists("employees.txt"))
            {
                // Создаем тестовые данные если файл не существует
                var emp1 = new Employee { 
                    Key = "E001", 
                    FullName = "Петров П.П." 
                };
                emp1.Floors.AddRange(new[] { 1, 2 });
                emp1.WorkDays.AddRange(new[] { DayOfWeek.Monday, DayOfWeek.Wednesday, DayOfWeek.Friday });
                
                employees.Add(emp1);
                SaveEmployees();
            }
            else
            {
                var lines = File.ReadAllLines("employees.txt");
                foreach (var line in lines)
                {
                    var parts = line.Split('|');
                    if (parts.Length >= 4)
                    {
                        var employee = new Employee
                        {
                            Key = parts[0],
                            FullName = parts[1]
                        };
                        
                        // Парсим этажи
                        var floorParts = parts[2].Split(',');
                        foreach (var floorStr in floorParts)
                        {
                            if (int.TryParse(floorStr.Trim(), out int floor))
                            {
                                employee.Floors.Add(floor);
                            }
                        }
                        
                        // Парсим дни работы
                        var dayParts = parts[3].Split(',');
                        foreach (var dayStr in dayParts)
                        {
                            if (Enum.TryParse<DayOfWeek>(dayStr.Trim(), out DayOfWeek day))
                            {
                                employee.WorkDays.Add(day);
                            }
                        }
                        
                        employees.Add(employee);
                    }
                }
            }
        }

        static void SaveRooms()
        {
            var lines = rooms.Select(r => $"{r.Number}|{r.Floor}|{r.Type}|{r.DailyCost}");
            File.WriteAllLines("rooms.txt", lines);
        }

        static void SaveClients()
        {
            var lines = clients.Select(c => 
                $"{c.Key}|{c.FullName}|{c.PassportNumber}|{c.CityFrom}|" +
                $"{c.RoomNumber}|{c.Place}|{c.ArrivalDate:yyyy-MM-dd}|{c.PaidDays}");
            File.WriteAllLines("clients.txt", lines);
        }

        static void SaveEmployees()
        {
            var lines = employees.Select(e => 
                $"{e.Key}|{e.FullName}|{string.Join(",", e.Floors)}|{string.Join(",", e.WorkDays)}");
            File.WriteAllLines("employees.txt", lines);
        }

        static void ShowMainMenu()
        {
            Console.WriteLine("\n=== Главное меню ===");
            Console.WriteLine("1. Запросы");
            Console.WriteLine("2. Управление персоналом");
            Console.WriteLine("3. Вывод информации");
            Console.WriteLine("4. Выход");
            Console.Write("Выберите действие (1-4): ");
        }

        static void HandleRequests()
        {
            while (true)
            {
                Console.WriteLine("\n=== Запросы ===");
                Console.WriteLine("1. Рассчитать стоимость места в указанном номере");
                Console.WriteLine("2. Вывести список клиентов, прибывших из заданного города");
                Console.WriteLine("3. Определить, какой служащий убирал номер указанного клиента в заданный день недели");
                Console.WriteLine("4. Показать количество свободных мест и свободных номеров в гостинице");
                Console.WriteLine("5. Вывести всех клиентов, проживающих в одноместных номерах");
                Console.WriteLine("6. Рассчитать общую сумму, выплаченную всеми клиентами");
                Console.WriteLine("7. Назад");
                Console.Write("Выберите действие (1-7): ");

                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        CalculateRoomCost();
                        break;
                    case "2":
                        ShowClientsByCity();
                        break;
                    case "3":
                        FindEmployeeForClientOnDay();
                        break;
                    case "4":
                        ShowFreeRoomsAndPlaces();
                        break;
                    case "5":
                        ShowClientsInSingleRooms();
                        break;
                    case "6":
                        CalculateTotalPayment();
                        break;
                    case "7":
                        return;
                    default:
                        Console.WriteLine("Неверный выбор. Пожалуйста, попробуйте снова.");
                        break;
                }
            }
        }

        static void CalculateRoomCost()
        {
            Console.Write("Введите номер комнаты: ");
            if (int.TryParse(Console.ReadLine(), out int roomNum))
            {
                var room = rooms.FirstOrDefault(r => r.Number == roomNum);
                if (room != null)
                {
                    var costPerPlace = room.DailyCost / room.GetMaxPlaces();
                    Console.WriteLine($"Стоимость одного места в номере {roomNum}: {costPerPlace:C}");
                }
                else
                {
                    Console.WriteLine("Номер не найден.");
                }
            }
            else
            {
                Console.WriteLine("Неверный формат номера комнаты.");
            }
        }

        static void ShowClientsByCity()
        {
            Console.Write("Введите город: ");
            var city = Console.ReadLine();
            
            if (!string.IsNullOrEmpty(city))
            {
                var clientsFromCity = clients.Where(c => c.CityFrom.Equals(city, StringComparison.OrdinalIgnoreCase)).ToList();
                
                if (clientsFromCity.Any())
                {
                    Console.WriteLine($"\nКлиенты из города {city}:");
                    foreach (var client in clientsFromCity)
                    {
                        Console.WriteLine($"- {client.FullName}, паспорт: {client.PassportNumber}");
                    }
                }
                else
                {
                    Console.WriteLine($"Клиенты из города {city} не найдены.");
                }
            }
            else
            {
                Console.WriteLine("Город не может быть пустым.");
            }
        }

        static void FindEmployeeForClientOnDay()
        {
            Console.Write("Введите имя клиента: ");
            var clientName = Console.ReadLine();
            
            Console.Write("Введите день недели (например, Monday): ");
            var dayInput = Console.ReadLine();
            
            if (!string.IsNullOrEmpty(clientName) && 
                Enum.TryParse<DayOfWeek>(dayInput, true, out DayOfWeek targetDay))
            {
                var client = clients.FirstOrDefault(c => 
                    c.FullName.Equals(clientName, StringComparison.OrdinalIgnoreCase));
                    
                if (client != null)
                {
                    var room = rooms.FirstOrDefault(r => r.Number == client.RoomNumber);
                    if (room != null)
                    {
                        var employee = employees.FirstOrDefault(e => 
                            e.Floors.Contains(room.Floor) && e.WorkDays.Contains(targetDay));
                            
                        if (employee != null)
                        {
                            Console.WriteLine($"Служащий, убиравший номер клиента {clientName} в {targetDay}: {employee.FullName}");
                        }
                        else
                        {
                            Console.WriteLine($"Не найдено служащего, убиравшего номер клиента {clientName} в {targetDay}.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Номер клиента не найден.");
                    }
                }
                else
                {
                    Console.WriteLine("Клиент не найден.");
                }
            }
            else
            {
                Console.WriteLine("Неверный ввод имени клиента или дня недели.");
            }
        }

        static void ShowFreeRoomsAndPlaces()
        {
            int totalRooms = rooms.Count;
            int occupiedRooms = rooms.Count(r => r.Clients.Any());
            int freeRooms = totalRooms - occupiedRooms;
            
            int totalPlaces = rooms.Sum(r => r.GetMaxPlaces());
            int occupiedPlaces = clients.Count;
            int freePlaces = totalPlaces - occupiedPlaces;
            
            Console.WriteLine($"\nВсего номеров: {totalRooms}");
            Console.WriteLine($"Занятых номеров: {occupiedRooms}");
            Console.WriteLine($"Свободных номеров: {freeRooms}");
            Console.WriteLine($"Всего мест: {totalPlaces}");
            Console.WriteLine($"Занятых мест: {occupiedPlaces}");
            Console.WriteLine($"Свободных мест: {freePlaces}");
        }

        static void ShowClientsInSingleRooms()
        {
            var singleRoomClients = clients
                .Where(c => {
                    var room = rooms.FirstOrDefault(r => r.Number == c.RoomNumber);
                    return room != null && room.Type == RoomType.Single;
                })
                .ToList();
                
            if (singleRoomClients.Any())
            {
                Console.WriteLine("\nКлиенты, проживающие в одноместных номерах:");
                foreach (var client in singleRoomClients)
                {
                    Console.WriteLine($"- {client.FullName}, номер {client.RoomNumber}");
                }
            }
            else
            {
                Console.WriteLine("Клиенты в одноместных номерах не найдены.");
            }
        }

        static void CalculateTotalPayment()
        {
            var totalPayment = clients.Sum(c => c.PaidDays * (rooms.First(r => r.Number == c.RoomNumber).DailyCost / rooms.First(r => r.Number == c.RoomNumber).GetMaxPlaces()));
            Console.WriteLine($"Общая сумма, выплаченная всеми клиентами: {totalPayment:C}");
        }

        static void HandleEmployeeManagement()
        {
            while (true)
            {
                Console.WriteLine("\n=== Управление персоналом ===");
                Console.WriteLine("1. Принять на работу нового служащего");
                Console.WriteLine("2. Уволить служащего");
                Console.WriteLine("3. Назад");
                Console.Write("Выберите действие (1-3): ");

                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        HireEmployee();
                        break;
                    case "2":
                        FireEmployee();
                        break;
                    case "3":
                        return;
                    default:
                        Console.WriteLine("Неверный выбор. Пожалуйста, попробуйте снова.");
                        break;
                }
            }
        }

        static void HireEmployee()
        {
            Console.Write("Введите ключ служащего (например, E001): ");
            var key = Console.ReadLine();
            
            if (string.IsNullOrEmpty(key))
            {
                Console.WriteLine("Ключ не может быть пустым.");
                return;
            }
            
            if (employees.Any(e => e.Key == key))
            {
                Console.WriteLine("Служащий с таким ключом уже существует.");
                return;
            }
            
            Console.Write("Введите ФИО служащего: ");
            var fullName = Console.ReadLine();
            
            var employee = new Employee { Key = key, FullName = fullName ?? string.Empty };
            
            Console.Write("Введите этажи через запятую (например, 1,2,3): ");
            var floorsInput = Console.ReadLine();
            if (!string.IsNullOrEmpty(floorsInput))
            {
                var floorParts = floorsInput.Split(',');
                foreach (var floorStr in floorParts)
                {
                    if (int.TryParse(floorStr.Trim(), out int floor))
                    {
                        employee.Floors.Add(floor);
                    }
                }
            }
            
            Console.Write("Введите дни работы через запятую (например, Monday,Tuesday): ");
            var daysInput = Console.ReadLine();
            if (!string.IsNullOrEmpty(daysInput))
            {
                var dayParts = daysInput.Split(',');
                foreach (var dayStr in dayParts)
                {
                    if (Enum.TryParse<DayOfWeek>(dayStr.Trim(), out DayOfWeek day))
                    {
                        employee.WorkDays.Add(day);
                    }
                }
            }
            
            employees.Add(employee);
            SaveEmployees();
            Console.WriteLine($"Служащий {fullName} успешно принят на работу.");
        }

        static void FireEmployee()
        {
            Console.Write("Введите ключ служащего для увольнения: ");
            var key = Console.ReadLine();
            
            if (string.IsNullOrEmpty(key))
            {
                Console.WriteLine("Ключ не может быть пустым.");
                return;
            }
            
            var employeeToRemove = employees.FirstOrDefault(e => e.Key == key);
            if (employeeToRemove != null)
            {
                employees.Remove(employeeToRemove);
                SaveEmployees();
                Console.WriteLine($"Служащий {employeeToRemove.FullName} уволен.");
            }
            else
            {
                Console.WriteLine("Служащий с таким ключом не найден.");
            }
        }

        static void ShowInformation()
        {
            while (true)
            {
                Console.WriteLine("\n=== Вывод информации ===");
                Console.WriteLine("1. Показать сведения о гостинице");
                Console.WriteLine("2. Показать список всех клиентов");
                Console.WriteLine("3. Показать список всех служащих");
                Console.WriteLine("4. Назад");
                Console.Write("Выберите действие (1-4): ");

                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        ShowHotelInfo();
                        break;
                    case "2":
                        ShowAllClients();
                        break;
                    case "3":
                        ShowAllEmployees();
                        break;
                    case "4":
                        return;
                    default:
                        Console.WriteLine("Неверный выбор. Пожалуйста, попробуйте снова.");
                        break;
                }
            }
        }

        static void ShowHotelInfo()
        {
            Console.WriteLine("\n=== Информация о гостинице ===");
            foreach (var room in rooms.OrderBy(r => r.Number))
            {
                Console.WriteLine($"Номер: {room.Number}, Этаж: {room.Floor}, Тип: {room.Type}, Стоимость: {room.DailyCost:C}");
                if (room.Clients.Any())
                {
                    Console.WriteLine("  Клиенты:");
                    foreach (var client in room.Clients)
                    {
                        Console.WriteLine($"    - {client.FullName} (место {client.Place})");
                    }
                }
                else
                {
                    Console.WriteLine("  Клиентов нет");
                }
            }
        }

        static void ShowAllClients()
        {
            Console.WriteLine("\n=== Все клиенты ===");
            foreach (var client in clients.OrderBy(c => c.FullName))
            {
                Console.WriteLine($"{client.Key}: {client.FullName}, паспорт: {client.PassportNumber}, " +
                                $"из города {client.CityFrom}, номер {client.RoomNumber}, " +
                                $"проживание с {client.ArrivalDate:dd.MM.yyyy}, дней оплачено: {client.PaidDays}");
            }
        }

        static void ShowAllEmployees()
        {
            Console.WriteLine("\n=== Все служащие ===");
            foreach (var employee in employees.OrderBy(e => e.FullName))
            {
                Console.WriteLine($"{employee.Key}: {employee.FullName}");
                Console.WriteLine($"  Этажи: {string.Join(", ", employee.Floors)}");
                Console.WriteLine($"  Дни работы: {string.Join(", ", employee.WorkDays)}");
            }
        }
    }
}