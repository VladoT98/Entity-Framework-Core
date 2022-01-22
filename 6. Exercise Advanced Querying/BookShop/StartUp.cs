using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using BookShop.Models;
using BookShop.Models.Enums;

namespace BookShop
{
    using Data;
    using Initializer;

    public class StartUp
    {
        public static void Main()
        {
            using var context = new BookShopContext();
            //DbInitializer.ResetDatabase(context);

            Console.WriteLine(RemoveBooks(context));
        }

        //1. Age Restriction
        public static string GetBooksByAgeRestriction(BookShopContext context, string command)
        {
            //Parse string to enum
            AgeRestriction ageRestriction = Enum.Parse<AgeRestriction>(command, true);

            string[] bookTitles = context
                .Books
                .ToArray()
                .Where(b => b.AgeRestriction == ageRestriction)
                .OrderBy(b => b.Title)
                .Select(b => b.Title)
                .ToArray();

            StringBuilder sb = new StringBuilder();

            foreach (var title in bookTitles)
            {
                sb.AppendLine(title);
            }

            return sb.ToString().TrimEnd();
        }

        //2. Golden Books
        public static string GetGoldenBooks(BookShopContext context)
        {
            string[] goldenEditionBooks = context
                .Books
                .Where(b => b.EditionType == EditionType.Gold && b.Copies < 5000)
                .OrderBy(b => b.BookId)
                .Select(b => b.Title)
                .ToArray();

            StringBuilder sb = new StringBuilder();

            foreach (var goldenEditionBook in goldenEditionBooks)
            {
                sb.AppendLine(goldenEditionBook);
            }

            return sb.ToString().TrimEnd();
        }

        //3. Books by Price
        public static string GetBooksByPrice(BookShopContext context)
        {
            var books = context
                .Books
                .Where(b => b.Price > 40)
                .OrderByDescending(b => b.Price)
                .Select(b => new
                {
                    Title = b.Title,
                    Price = b.Price
                })
                .ToArray();

            StringBuilder sb = new StringBuilder();

            foreach (var b in books)
            {
                sb.AppendLine($"{b.Title} - ${b.Price}");
            }

            return sb.ToString().TrimEnd();
        }

        //4. Not Released In
        public static string GetBooksNotReleasedIn(BookShopContext context, int year)
        {
            string[] bookTitles = context
                .Books
                .Where(b => b.ReleaseDate.Value.Year != year)
                .OrderBy(b => b.BookId)
                .Select(b => b.Title)
                .ToArray();

            StringBuilder sb = new StringBuilder();

            foreach (var bookTitle in bookTitles)
            {
                sb.AppendLine(bookTitle);
            }

            return sb.ToString().TrimEnd();
        }

        //5. Book Titles by Category NOT DONE
        public static string GetBooksByCategory(BookShopContext context, string input)
        {
            string[] categories = input.Split();

            List<string> bookTitles = new List<string>();

            //horror
            foreach (var category in categories)
            {
                BookCategory currBookCategory = context
                    .BooksCategories
                    .FirstOrDefault(bc => bc.Category.Name == category);

                List<string> currBookTitles = context
                    .Books
                    .ToArray()
                    .Where(b => b.BookCategories.Contains(currBookCategory))
                    .Select(b => b.Title)
                    .ToList();

                foreach (var currBookTitle in currBookTitles)
                {
                    bookTitles.Add(currBookTitle);
                }
            }

            StringBuilder sb = new StringBuilder();

            foreach (var bookTitle in bookTitles)
            {
                sb.AppendLine(bookTitle);
            }

            return sb.ToString().TrimEnd();
        }

        //6. Released Before Date
        public static string GetBooksReleasedBefore(BookShopContext context, string date)
        {
            var books = context
                .Books
                .Where(b => b.ReleaseDate < Convert.ToDateTime(date))
                .OrderByDescending(b => b.ReleaseDate)
                .Select(b => new
                {
                    b.Title,
                    b.EditionType,
                    b.Price
                })
                .ToArray();

            StringBuilder sb = new StringBuilder();

            foreach (var b in books)
            {
                sb.AppendLine($"{b.Title} - {b.EditionType} - ${b.Price:F2}");
            }

            return sb.ToString().TrimEnd();
        }

        //7. Author Search
        public static string GetAuthorNamesEndingIn(BookShopContext context, string input)
        {
            string[] authorNames = context
                .Authors
                .Where(a => a.FirstName.ToLower().EndsWith(input.ToLower()))
                .Select(a => $"{a.FirstName} {a.LastName}")
                .OrderBy(a => a)
                .ToArray();

            StringBuilder sb = new StringBuilder();

            foreach (var authorName in authorNames)
            {
                sb.AppendLine(authorName);
            }

            return sb.ToString().TrimEnd();
        }

        //8. Book Search
        public static string GetBookTitlesContaining(BookShopContext context, string input)
        {
            string[] bookTitles = context
                .Books
                .Where(b => b.Title.Contains(input))
                .OrderBy(b => b.Title)
                .Select(b => b.Title)
                .ToArray();

            StringBuilder sb = new StringBuilder();

            foreach (var bookTitle in bookTitles)
            {
                sb.AppendLine(bookTitle);
            }

            return sb.ToString().TrimEnd();
        }

        //9. Book Search by Author
        public static string GetBooksByAuthor(BookShopContext context, string input)
        {
            var booksInfo = context
                .Books
                .Where(b => b.Author.LastName.StartsWith(input))
                .OrderBy(b => b.BookId)
                .Select(b => new
                {
                    b.Title,
                    AuthorFullName = $"{b.Author.FirstName} {b.Author.LastName}"
                })
                .ToArray();

            StringBuilder sb = new StringBuilder();

            foreach (var bookInfo in booksInfo)
            {
                sb.AppendLine($"{bookInfo.Title} ({bookInfo.AuthorFullName})");
            }

            return sb.ToString().TrimEnd();
        }

        //10. Count Books
        public static int CountBooks(BookShopContext context, int lengthCheck)
        {
            int numOfBooks = context
                .Books
                .Where(b => b.Title.Length > lengthCheck)
                .ToArray()
                .Length;

            return numOfBooks;
        }

        //11. Total Book Copies
        public static string CountCopiesByAuthor(BookShopContext context)
        {
            var authorsWithBooksCopies = context
                .Authors
                .Select(a => new
                {
                    Author = $"{a.FirstName} {a.LastName}",
                    Copies = a.Books.Sum(b => b.Copies)
                })
                .OrderByDescending(a => a.Copies)
                .ToArray();

            StringBuilder sb = new StringBuilder();

            foreach (var book in authorsWithBooksCopies)
            {
                sb.AppendLine($"{book.Author} - {book.Copies}");
            }

            return sb.ToString().TrimEnd();
        }

        //12. Profit by Category
        public static string GetTotalProfitByCategory(BookShopContext context)
        {
            var categories = context
                .Categories
                .Select(c => new
                {
                    CategoryName = c.Name,
                    Profit = c.CategoryBooks
                        .Sum(cb => cb.Book.Copies * cb.Book.Price)
                })
                .OrderByDescending(cb => cb.Profit)
                .ToArray();

            StringBuilder sb = new StringBuilder();

            foreach (var category in categories)
            {
                sb.AppendLine($"{category.CategoryName} ${category.Profit:F2}");
            }

            return sb.ToString().TrimEnd();
        }

        //13. Most Recent Books
        public static string GetMostRecentBooks(BookShopContext context)
        {
            var categories = context
                .Categories
                .Select(c => new
                {
                    CategoryName = c.Name,
                    BookTitles = c.CategoryBooks
                        .Select(cb => cb.Book)
                        .OrderByDescending(b => b.ReleaseDate)
                        .Select(b => new
                        {
                            BookTitle = b.Title,
                            BookReleaseYear = b.ReleaseDate.Value.Year
                        })
                        .Take(3)
                        .ToArray()
                })
                .OrderBy(c => c.CategoryName)
                .ToArray();

            StringBuilder sb = new StringBuilder();

            foreach (var category in categories)
            {
                sb.AppendLine($"--{category.CategoryName}");

                foreach (var bookTitle in category.BookTitles)
                {
                    sb.AppendLine($"{bookTitle.BookTitle} ({bookTitle.BookReleaseYear})");
                }
            }

            return sb.ToString().TrimEnd();
        }

        //14. Increase Prices
        public static void IncreasePrices(BookShopContext context)
        {
            var books = context
                .Books
                .Where(b => b.ReleaseDate.Value.Year < 2010)
                .ToArray();

            foreach (var book in books)
            {
                book.Price += 5;
            }

            context.SaveChanges();
        }

        //15. Remove Books
        public static int RemoveBooks(BookShopContext context)
        {
            var books = context
                .Books
                .Where(b => b.Copies < 4200)
                .ToArray();

            int result = books.Length;

            foreach (var book in books)
            {
                context.BulkDelete(book, null);
            }

            context.SaveChanges();

            return result;
        }
    }
}