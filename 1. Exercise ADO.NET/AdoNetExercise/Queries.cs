using System;
using System.Collections.Generic;
using System.Text;

namespace AdoNetExercise
{
    public static class Queries
    {
        public const string VILLAINS_WITH_MORE_THAN_3_MINIONS_QUERY =
            @"SELECT v.Name, 
	          COUNT(mv.VillainId) AS MinionsCount  
              FROM Villains AS v 
              JOIN MinionsVillains AS mv ON v.Id = mv.VillainId 
              GROUP BY v.Id, v.Name 
              HAVING COUNT(mv.VillainId) > 3 
              ORDER BY COUNT(mv.VillainId)";

        public const string VILLAIN_NAME_BY_ID_QUERY =
            @"SELECT [Name]
              FROM Villains
              WHERE Id = @Id";

        public const string VILLAIN_MINIONS_INFO_BY_ID_QUERY =
            @"SELECT 
              ROW_NUMBER() OVER (ORDER BY m.Name) as RowNum,
              m.Name, 
              m.Age
              FROM MinionsVillains AS mv
              JOIN Minions As m ON mv.MinionId = m.Id
              WHERE mv.VillainId = @Id
              ORDER BY m.Name";

        public const string ADDING_MINION_TO_VILLAIN = @"INSERT INTO MinionsVillains (MinionId, VillainId) VALUES (@villainId, @minionId)";

        public const string CHECK_IF_VILLAIN_EXIST_QUERY = @"SELECT Id FROM Villains WHERE Name = @Name";

        public const string CHECK_IF_TOWN_EXIST_QUERY = @"SELECT Id FROM Towns WHERE Name = @townName";

        public const string CHECK_IF_MINION_EXIST_QUERY = @"SELECT Id FROM Minions WHERE Name = @Name";


        public const string INSERT_VILLAIN_QUERY = @"INSERT INTO Villains (Name, EvilnessFactorId)  VALUES (@villainName, 4)";

        public const string INSERT_TOWN_QUERY = @"INSERT INTO Towns (Name) VALUES (@townName)";

        public const string INSERT_MINION_QUERY = @"INSERT INTO Minions (Name, Age, TownId) VALUES (@nam, @age, @townId)";
    }
}
