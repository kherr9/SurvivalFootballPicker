using System;
using System.Collections.Generic;
using System.Linq;

namespace FootballPicker.ConsoleApp
{
    /// <summary>
    /// Chooses the team w/ the largest rank difference.
    /// Select max team for min week
    /// </summary>
    public static class SimpleGreedyPicker2
    {
        public static void Choose(Match[] input, Rank[] rankings)
        {
            var rankingLookup = rankings.ToDictionary(r => r.Team, r => r.Ranking);

            var selections = input.SelectMany(match =>
            {
                var week = match.Week;

                var homeTeam = match.HomeTeam;
                var homeTeamRanking = rankingLookup[homeTeam];

                var awayTeam = match.AwayTeam;
                var awayTeamRanking = rankingLookup[awayTeam];

                // FACT 60% of games are won at home
                const int homeFieldAdvantageWeight = 4;

                return new List<Selection>
                {
                    new Selection
                    {
                        Week = week,
                        Team = homeTeam,
                        Ranking = homeTeamRanking,
                        Value = awayTeamRanking - homeTeamRanking + homeFieldAdvantageWeight,
                        Opponent = awayTeam,
                        OpponentRanking = awayTeamRanking,
                        HomeTeam = homeTeam
                    },
                    new Selection
                    {
                        Week = week,
                        Team = awayTeam,
                        Ranking = awayTeamRanking,
                        Value = homeTeamRanking - awayTeamRanking - homeFieldAdvantageWeight,
                        Opponent = homeTeam,
                        OpponentRanking = homeTeamRanking,
                        HomeTeam = homeTeam
                    }
                };
            }).ToList();

            // only worry about the first 10 weeks
            selections = selections.Where(s => s.Week <= 10).ToList();

            var picks = new List<Selection>();

            while (selections.Any())
            {
                var pick = selections
                    .GroupBy(s => s.Week)
                    // get the max match for each week
                    .Select(grp => grp.Max())
                    // select the min match from the max matches from each week
                    .Min();

                picks.Add(pick);

                // remove selected team and week from selections
                selections = selections.Where(s => picks.All(p => p.Team != s.Team)).ToList();
                selections = selections.Where(s => picks.All(p => p.Week != s.Week)).ToList();
            }

            foreach (var pick in picks.OrderBy(x => x.Week))
            {
                Console.WriteLine(pick.Print());
            }
        }

        private struct Selection : IComparable<Selection>
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
                return $"Week:{Week,-2} Team:{TeamFormatted(),-4} TeamRank:{Ranking,-2} Value:{Value,-2} Opponent:{OpponentFormatted(),-4} OpponentRank:{OpponentRanking,-2}";
            }

            private bool HomeFieldAdvantage() => Team == HomeTeam;

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

            public int CompareTo(Selection other)
            {
                var valueComparison = Value.CompareTo(other.Value);
                if (valueComparison != 0) return valueComparison;

                var homeFieldAdvantage = HomeFieldAdvantage().CompareTo(other.HomeFieldAdvantage());
                if (homeFieldAdvantage != 0) return homeFieldAdvantage;

                return Ranking.CompareTo(other.Ranking);
            }
        }
    }
}