using System;
using System.Collections.Generic;
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

            Console.WriteLine();

            var matches = ReadSchedule();

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

        private static Match[] ReadSchedule()
        {
            return File.ReadAllLines("schedule.csv")
                .Skip(1)
                .Where(line => !string.IsNullOrWhiteSpace(line))
                .Select(line => line.Split(','))
                .Select(row =>
                    new
                    {
                        Team = row[0],
                        Weeks = row.Skip(1).ToArray()
                    })
                .SelectMany(row =>
                {
                    var result = new List<Match>();
                    for (var i = 0; i < row.Weeks.Length; i++)
                    {
                        var week = i + 1;
                        var otherTeam = row.Weeks[i];

                        // skip when other team is home
                        // because the matches are duplicated.
                        // this is an easy way to de-dupe
                        // and keep logic simple
                        if (otherTeam[0] == '@')
                            continue;

                        result.Add(new Match
                        {
                            Week = week,
                            HomeTeam = row.Team,
                            AwayTeam = otherTeam
                        });
                    }

                    return result;
                })
                .OrderBy(x => x.Week)
                .ThenBy(x => x.HomeTeam)
                .ToArray();
        }
    }
}
