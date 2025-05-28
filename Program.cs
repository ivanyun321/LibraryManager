using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

public class Book
{
    public string Title { get; set; }
    public string Author { get; set; }

    public Book(string title, string author)
    {
        Title = title;
        Author = author;
    }

    public void Display()
    {
        Console.WriteLine($"Title: {Title}, Author: {Author}");
    }
}

public class Library
{
    private List<Book> books = new List<Book>();
    public void AddBook(string title, string author)
    {
        books.Add(new Book(title, author));
        Console.WriteLine("Book Added!");
    }

    public void ViewBooks()
    {
        if (books.Count == 0)
        {
            Console.WriteLine("Library is empty.");
            return;
        }
        Console.WriteLine("Books in Library");
        for (int i = 0; i < books.Count; i++)
        {
            Console.Write($"{i + 1}.");
            books[i].Display();
        }
    }

    public void RemoveBook(int index)
    {
        if (index < 1 || index > books.Count)
        {
            Console.WriteLine("Invalid Index");
            return;
        }

        books.RemoveAt(index - 1);
        Console.WriteLine("Book removed.");
    }

    public async Task SearchAndAddBookAsync(string query)
{
    using HttpClient client = new HttpClient();
    string url = $"https://openlibrary.org/search.json?q={Uri.EscapeDataString(query)}";

    try
    {
        string json = await client.GetStringAsync(url);
        using JsonDocument doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        if (root.TryGetProperty("docs", out JsonElement docs) && docs.GetArrayLength() > 0)
        {
            var first = docs[0];
            string title = first.GetProperty("title").GetString();

            string author = "Unknown";
            if (first.TryGetProperty("author_name", out JsonElement authors) && authors.GetArrayLength() > 0)
            {
                author = authors[0].GetString();
            }

            books.Add(new Book(title, author));
            Console.WriteLine($"✅ Fetched and added: {title} by {author}");
        }
        else
        {
            Console.WriteLine("❌ No results found.");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine("❌ Error fetching book: " + ex.Message);
    }
}

}

public class Program
{
    public static async Task Main(string[] args)
    {
        Library myLibrary = new Library();
        bool running = true;
        while (running)
        {
            Console.WriteLine("Library Book Manager");
            Console.WriteLine("1. Add book");
            Console.WriteLine("2. View Books");
            Console.WriteLine("3. Remove Book");
            Console.WriteLine("4. Exit");
            Console.Write("Enter your choice (1-4):");

            string input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    Console.Write("Enter book title: ");
                    string title = Console.ReadLine();
                    await myLibrary.SearchAndAddBookAsync(title);
                    break;
                case "2":
                    myLibrary.ViewBooks();
                    break;
                case "3":
                    Console.Write("Enter the index of the book to remove:");
                    if (int.TryParse(Console.ReadLine(), out int index))
                    {
                        myLibrary.RemoveBook(index);
                    }
                    else
                    {
                        Console.WriteLine("Invalid input");
                    }
                    break;
                case "4":
                    running = false;
                    Console.WriteLine("Goodbye");
                    break;
                default:
                    Console.WriteLine("Invalid option");
                    break;
            }
        }
    }
}