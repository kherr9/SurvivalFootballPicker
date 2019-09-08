namespace FootballPicker.ConsoleApp
{
    class Program
    {
        static void Main()
        {
            var ranks = Data.LoadRankings();
            var matches = Data.LoadMatches();

            SimpleGreedyPicker.Choose(matches, ranks);
        }
    }
}
