using System;
using System.Collections.Generic;
using System.Linq;

namespace FootballPicker.ConsoleApp
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine("Hello World!");

            var ranks = Data.LoadRankings();
            var matches = Data.LoadMatches();

            SimpleGreedyPicker.Choose(matches, ranks);
        }
    }

    public static class SimpleGreedyPicker
    {
        public static void Choose(Match[] input, Rank[] rankings)
        {
            var rankingLookup = rankings.ToDictionary(r => r.Team, r => r.Ranking);

            var matchesByWeek = input.GroupBy(x => x.Week)
                .OrderBy(grp => grp.Key)
                .Select(grp => grp.ToArray())
                .ToArray();

            var picks = new List<Selection>();

            foreach (var weekMatches in matchesByWeek)
            {
                var selections = new List<Selection>();

                foreach (var match in weekMatches)
                {
                    var homeTeam = match.HomeTeam;
                    var homeTeamRanking = rankingLookup[homeTeam];

                    var awayTeam = match.AwayTeam;
                    var awayTeamRanking = rankingLookup[awayTeam];

                    // FACT 60% of games are won at home
                    const int homeFieldAdvantageWeight = 4;

                    selections.Add(new Selection
                    {
                        Team = homeTeam,
                        Ranking = homeTeamRanking,
                        Value = awayTeamRanking - homeTeamRanking + homeFieldAdvantageWeight,
                        Opponent = awayTeam,
                        OpponentRanking = awayTeamRanking
                    });

                    selections.Add(new Selection
                    {
                        Team = awayTeam,
                        Ranking = awayTeamRanking,
                        Value = homeTeamRanking - awayTeamRanking - homeFieldAdvantageWeight,
                        Opponent = homeTeam,
                        OpponentRanking = homeTeamRanking
                    });
                }

                var pick = selections
                    // remove teams that have already been picked
                    .Where(s => picks.All(p => p.Team != s.Team))
                    // find max selection
                    .OrderByDescending(s => s.Value)
                    .ThenByDescending(s => s.Ranking)
                    .First();

                picks.Add(pick);
            }

            for (var i = 0; i < picks.Count; i++)
            {
                var week = i + 1;
                var pick = picks[i];

                Console.WriteLine($"Week:{week,-2} Team:{pick.Team,-3} TeamRank:{pick.Ranking,-2} Value:{pick.Value,-2} Opponent:{pick.Opponent,-3} OpponentRanking:{pick.OpponentRanking,-2}");
            }
        }

        private struct Selection
        {
            public string Team;
            public int Ranking;
            public int Value;
            public string Opponent;
            public int OpponentRanking;
        }
    }
}
