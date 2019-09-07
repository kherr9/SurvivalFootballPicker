using System;
using System.Collections.Generic;
using System.Linq;

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

            var week = 0;
            foreach (var weekMatches in matchesByWeek)
            {
                week++;
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
                        Week = week,
                        Team = homeTeam,
                        Ranking = homeTeamRanking,
                        Value = awayTeamRanking - homeTeamRanking + homeFieldAdvantageWeight,
                        Opponent = awayTeam,
                        OpponentRanking = awayTeamRanking,
                        HomeTeam = homeTeam
                    });

                    selections.Add(new Selection
                    {
                        Week = week,
                        Team = awayTeam,
                        Ranking = awayTeamRanking,
                        Value = homeTeamRanking - awayTeamRanking - homeFieldAdvantageWeight,
                        Opponent = homeTeam,
                        OpponentRanking = homeTeamRanking,
                        HomeTeam = homeTeam
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

            foreach (var pick in picks)
            {
                Console.WriteLine(pick.Print());
            }
        }

        private struct Selection
        {
            public int Week;
            public string Team;
            public int Ranking;
            public int Value;
            public string Opponent;
            public int OpponentRanking;
            public string HomeTeam { get; set; }

            public string Print()
            {
                return $"Week:{Week,-2} Team:{TeamFormatted(),-4} TeamRank:{Ranking,-2} Value:{Value,-2} Opponent:{OpponentFormatted(),-4} OpponentRanking:{OpponentRanking,-2}";
            }

            private string TeamFormatted()
            {
                if (Team == HomeTeam)
                    return $"@{Team}";

                return Team;
            }

            private string OpponentFormatted()
            {
                if (Opponent == HomeTeam)
                    return $"@{Opponent}";

                return Opponent;
            }
        }
    }
}
