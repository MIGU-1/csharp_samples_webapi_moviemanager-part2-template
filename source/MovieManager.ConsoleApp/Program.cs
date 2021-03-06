﻿using System;
using System.Linq;
using System.Threading.Tasks;
using MovieManager.Core;
using MovieManager.Core.Contracts;
using MovieManager.Core.DataTransferObjects;
using MovieManager.Core.Entities;
using MovieManager.Persistence;

namespace MovieManager.ConsoleApp
{
    class Program
    {
        static async Task Main()
        {
            await InitDataAsync();
            await AnalyzeDataAsync();

            Console.WriteLine();
            Console.Write("Beenden mit Eingabetaste ...");
            Console.ReadLine();
        }

        private static async Task InitDataAsync()
        {
            Console.WriteLine("***************************");
            Console.WriteLine("          Import");
            Console.WriteLine("***************************");

            Console.WriteLine("Import der Movies und Categories in die Datenbank");
            await using IUnitOfWork unitOfWork = new UnitOfWork();
            Console.WriteLine("Datenbank löschen");
            await unitOfWork.DeleteDatabaseAsync();
            Console.WriteLine("Datenbank migrieren");
            await unitOfWork.MigrateDatabaseAsync();
            Console.WriteLine("Movies/Categories werden eingelesen");

            var movies = await ImportController.ReadFromCsvAsync();
            if (movies.Length == 0)
            {
                Console.WriteLine("!!! Es wurden keine Movies eingelesen");
                return;
            }

            var categories = movies
                .Select(movie => movie.Category)
                .Distinct();

            Console.WriteLine($"  Es wurden {movies.Count()} Movies in {categories.Count()} Kategorien eingelesen!");
            await unitOfWork.Categories.AddRangeAsync(categories);
            var count = await unitOfWork.SaveChangesAsync();
            Console.WriteLine($"{count} Categories wurden in DB gespeichert!");

            await unitOfWork.Movies.AddRangeAsync(movies);
            count = await unitOfWork.SaveChangesAsync();
            Console.WriteLine($"{count} Movies wurden in DB gespeichert!");

            Console.WriteLine();
        }

        private static async Task AnalyzeDataAsync()
        {
            Console.WriteLine("***************************");
            Console.WriteLine("        Statistik");
            Console.WriteLine("***************************");


            await using IUnitOfWork unitOfWork = new UnitOfWork();
            // Längster Film: Bei mehreren gleichlangen Filmen, soll jener angezeigt werden, dessen Titel im Alphabet am weitesten vorne steht.
            // Die Dauer des längsten Films soll in Stunden und Minuten angezeigt werden!
            Movie longestMovie = await unitOfWork.Movies.GetLongestMovieAsync();
            Console.WriteLine($"Längster Film: {longestMovie.Title}; Länge: {GetDurationAsString(longestMovie.Duration, false)}");
            Console.WriteLine();


            // Top Kategorie:
            //   - Jene Kategorie mit den meisten Filmen.
            CategoryStatisticEntry topCategory = await unitOfWork.Categories.GetCategoryWithMostMoviesAsync();
            Console.WriteLine($"Kategorie mit den meisten Filmen: '{topCategory.CategoryName}'; Filme: {topCategory.NumberOfMovies}");
            Console.WriteLine();

            // Jahr der Kategorie "Action":
            //  - In welchem Jahr wurden die meisten Action-Filme veröffentlicht?
            int yearOfAction = await unitOfWork.Categories.GetYearWithMostPublicationsForCategoryAsync("Action");
            Console.WriteLine($"Jahr der meisten Action-Filme: {yearOfAction}");
            Console.WriteLine();

            // Kategorie Auswertung (Teil 1):
            //   - Eine Liste in der je Kategorie die Anzahl der Filme und deren Gesamtdauer dargestellt wird.
            //   - Sortiert nach dem Namen der Kategorie (aufsteigend).
            //   - Die Gesamtdauer soll in Stunden und Minuten angezeigt werden!
            Console.WriteLine("Kategorie Auswertung:");
            Console.WriteLine();
            Console.WriteLine("Kategorie   Anzahl   Gesamtdauer");
            Console.WriteLine("================================");
            foreach (var entry in await unitOfWork.Categories.GetCategoryStatisticsAsync())
            {
                Console.WriteLine($"{entry.CategoryName,-12} {entry.NumberOfMovies,-7} {GetDurationAsString(entry.TotalDuration, false),11}");
            }
            Console.WriteLine();

            // Kategorie Auswertung (Teil 2):
            //   - Alle Kategorien und die durchschnittliche Dauer der Filme der Kategorie
            //   - Absteigend sortiert nach der durchschnittlichen Dauer der Filme.
            //     Bei gleicher Dauer dann nach dem Namen der Kategorie aufsteigend sortieren.
            //   - Die Gesamtdauer soll in Stunden, Minuten und Sekunden angezeigt werden!
            Console.WriteLine("Kategorien sortiert nach durchschn. Dauer:");
            Console.WriteLine();
            Console.WriteLine("Kategorie    durchschn. Gesamtdauer");
            Console.WriteLine("===================================");
            foreach (var entry in await unitOfWork.Categories.GetCategoriesWithAverageLengthOfMoviesAsync())
            {
                Console.WriteLine($"{entry.CategoryName,-12} {GetDurationAsString(entry.AverageLength),-15}");
            }
            Console.WriteLine();
        }

        private static string GetDurationAsString(double minutes, bool withSeconds = true)
        {
            int hoursPart = (int)minutes / 60;
            int minutesPart = (int)minutes % 60;
            int secondsPart = (int)(((decimal)minutes % 1) * 60m);

            string withoutSecondsResult = $"{hoursPart:D2} h {minutesPart:D2} min";

            if (withSeconds)
            {
                return $"{withoutSecondsResult} {secondsPart:D2} sec";
            }

            return withoutSecondsResult;
        }
    }
}
