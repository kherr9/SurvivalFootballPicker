using System;
using System.IO;
using System.Linq;

namespace FootballPicker.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            foreach (var rank in ReadRankings().OrderBy(x => x.Ranking))
            {
                Console.WriteLine(rank.ToString());
            }
        }

        private static Rank[] ReadRankings()
        {
            return File.ReadAllLines("ranking.csv")
                .Skip(1)
                .Where(line => !string.IsNullOrWhiteSpace(line))
                .Select(line => line.Split(','))
                .Select(row => new Rank
                {
                    Team = row[0],
                    Ranking = int.Parse(row[1])
                })
                .ToArray();
        }
    }
}
