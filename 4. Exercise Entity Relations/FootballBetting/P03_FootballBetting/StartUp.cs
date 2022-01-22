using System;
using Microsoft.EntityFrameworkCore;
using P03_FootballBetting.Data;

namespace P03_FootballBetting
{
    class StartUp
    {
        static void Main(string[] args)
        {
            FootballBettingContext db = new FootballBettingContext();

            db.Database.Migrate();

            Console.WriteLine("DB was created successfully!");
        }
    }
}
