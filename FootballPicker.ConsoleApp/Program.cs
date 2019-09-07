using System;
using System.Linq;

namespace FootballPicker.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            foreach (var rank in Data.LoadRankings().OrderBy(x => x.Ranking))
            {
                Console.WriteLine(rank.ToString());
            }

            Console.WriteLine();

            var matches = Data.LoadMatches();

            var arizonaSchedule = matches.Where(m => m.IsPlaying("ARI")).OrderBy(x => x.Week);

            foreach (var match in arizonaSchedule)
            {
                Console.WriteLine($"Week:{match.Week,-2}, Home:{match.HomeTeam,-3}, Away:{match.AwayTeam,-3}");
            }

            var week1 = matches.Where(m => m.Week == 1).OrderBy(x => x.HomeTeam);

            foreach (var match in week1)
            {
                Console.WriteLine($"Week:{match.Week,-2}, Home:{match.HomeTeam,-3}, Away:{match.AwayTeam,-3}");
            }
        }
    }
}
