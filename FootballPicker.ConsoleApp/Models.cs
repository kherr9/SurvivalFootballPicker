namespace FootballPicker.ConsoleApp
{
    public struct Rank
    {
        public string Team;
        public int Ranking;

        public override string ToString()
        {
            return $"{Team,-3}:{Ranking}";
        }
    }

    public struct Schedule
    {
        public int Week;
        public string HomeTeam;
        public string AwayTeam;
    }
}
