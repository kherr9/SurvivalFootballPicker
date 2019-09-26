using System;

namespace FootballPicker.ConsoleApp
{
    class Program
    {
        static void Main()
        {
            var ranks = Data.LoadRankings();
            var matches = Data.LoadMatches();

            var picks = new[]
            {
                "BAL",
                "CAR",
                "DAL"
            };


            Console.WriteLine("\n*** simple greedy ***");
            SimpleGreedyPicker.Choose(matches, ranks);

            Console.WriteLine("\n*** simple greedy 2 ***");
            SimpleGreedyPicker2.Choose(matches, ranks, picks);
        }
    }
}
