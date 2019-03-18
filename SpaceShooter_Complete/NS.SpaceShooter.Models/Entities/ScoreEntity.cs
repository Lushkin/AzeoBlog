namespace NS.SpaceShooter.Models.Entities
{
    using Microsoft.WindowsAzure.Storage.Table;
    using System;

    public class ScoreEntity : TableEntity
    {
        public ScoreEntity()
        {

        }

        public ScoreEntity(Guid id, string name, int score, string date)
        {
            PartitionKey = id.ToString();
            RowKey = date;
            Name = name;
            Score = score;
        }

        public int Score { get; set; }
        public string Name { get; set; }
    }
}
