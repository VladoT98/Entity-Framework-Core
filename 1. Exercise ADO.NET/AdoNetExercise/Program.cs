using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;

namespace AdoNetExercise
{
    class Program
    {
        static void Main(string[] args)
        {
            SqlConnection sqlConnection = new SqlConnection(Configuration.CONNECTION_STRING);

            sqlConnection.Open();

            //4.
            Console.Write("Enter Minion Info: ");
            string[] minionInfo = Console.ReadLine().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            string minionName = minionInfo[1];
            int minionAge = int.Parse(minionInfo[2]);
            string minionTown = minionInfo[3];

            Console.Write("Enter Villain Info: ");
            string villainName = Console.ReadLine().Split(' ', StringSplitOptions.RemoveEmptyEntries)[1];
            //===============================================================

            using (sqlConnection)
            {
                //4.
                AddMinionToVillain(sqlConnection, minionName, minionAge, villainName, minionTown);
                //=====================================================================================================
            }
        }

        //2. Villain Names
        private static void PrintVillainsWithMoreThan3Minions(SqlConnection sqlConnection)
        {
            SqlCommand sqlCommand = new SqlCommand(Queries.VILLAINS_WITH_MORE_THAN_3_MINIONS_QUERY, sqlConnection);

            SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();

            using (sqlDataReader)
            {
                while (sqlDataReader.Read())
                {
                    string villainName = sqlDataReader.GetString(0);
                    int minionsCount = sqlDataReader.GetInt32(1);

                    Console.WriteLine($"{villainName} - {minionsCount}");
                }
            }
        }
        //=====================================================================================================

        //3. Minion Names
        private static void PrintVillainsWithMinions(SqlConnection sqlConnection, int villainId)
        {
            SqlCommand getVillainNameCmd = new SqlCommand(Queries.VILLAIN_NAME_BY_ID_QUERY, sqlConnection);
            getVillainNameCmd.Parameters.AddWithValue("@Id", villainId);

            string villainName = getVillainNameCmd.ExecuteScalar().ToString();

            if (villainName == null)
            {
                Console.WriteLine("Villain not found.");
                return;
            }

            SqlCommand villainMinionsInfoCmd = new SqlCommand(Queries.VILLAIN_MINIONS_INFO_BY_ID_QUERY, sqlConnection);
            villainMinionsInfoCmd.Parameters.AddWithValue("@Id", villainId);

            SqlDataReader sqlDataReader = villainMinionsInfoCmd.ExecuteReader();

            using (sqlDataReader)
            {
                Console.WriteLine($"Villain: {villainName}");

                if (!sqlDataReader.HasRows)
                {
                    Console.WriteLine("No minions");
                }
                else
                {
                    while (sqlDataReader.Read())
                    {
                        long rowNumber = sqlDataReader.GetInt64(0);
                        string minionName = sqlDataReader.GetString(1);
                        int minionAge = sqlDataReader.GetInt32(2);

                        Console.WriteLine($"{rowNumber}. {minionName} {minionAge}");
                    }
                }

            }
        }
        //=====================================================================================================

        //4. Add Minion
        private static void AddMinionToVillain(SqlConnection sqlConnection, string minionName, int minionAge, string villainName, string townName)
        {
            int townId = AddTown(sqlConnection, townName);
            int minionId = AddMinion(sqlConnection, minionName, minionAge, townId);
            int villainId = AddVillain(sqlConnection, villainName);

            SqlCommand addingMinionToVillain = new SqlCommand(Queries.ADDING_MINION_TO_VILLAIN, sqlConnection);
            addingMinionToVillain.Parameters.AddWithValue("@villainId", minionId);
            addingMinionToVillain.Parameters.AddWithValue("@minionId", villainId);
            addingMinionToVillain.ExecuteNonQuery();

            Console.WriteLine($"Successfully added {minionName} to be minion of {villainName}.");
        }
        private static int AddTown(SqlConnection sqlConnection, string townName)
        {
            SqlCommand checkIfTownExistCommand = new SqlCommand(Queries.CHECK_IF_TOWN_EXIST_QUERY, sqlConnection);
            checkIfTownExistCommand.Parameters.AddWithValue("@townName", townName);

            object townIdObj = checkIfTownExistCommand.ExecuteScalar();

            if (townIdObj == null)
            {
                SqlCommand addTownCommand = new SqlCommand(Queries.INSERT_TOWN_QUERY, sqlConnection);
                addTownCommand.Parameters.AddWithValue("@townName", townName);
                addTownCommand.ExecuteNonQuery();
                townIdObj = checkIfTownExistCommand.ExecuteScalar();

                Console.WriteLine($"Town {townName} was added to the database.");
            }

            return (int)townIdObj;
        }
        private static int AddMinion(SqlConnection sqlConnection, string minionName, int minionAge, int townId)
        {
            SqlCommand checkMinionExistCommand = new SqlCommand(Queries.CHECK_IF_MINION_EXIST_QUERY, sqlConnection);
            checkMinionExistCommand.Parameters.AddWithValue("@Name", minionName);

            object minionIdObj = checkMinionExistCommand.ExecuteScalar();

            if (minionIdObj == null)
            {
                SqlCommand addMinionCommand = new SqlCommand(Queries.INSERT_MINION_QUERY, sqlConnection);
                addMinionCommand.Parameters.AddWithValue("@nam", minionName);
                addMinionCommand.Parameters.AddWithValue("@age", minionAge);
                addMinionCommand.Parameters.AddWithValue("@townId", townId);
                addMinionCommand.ExecuteNonQuery();
                minionIdObj = checkMinionExistCommand.ExecuteScalar();

                Console.WriteLine($"Minion {minionName} was added to the database.");
            }

            return (int)minionIdObj;
        }
        private static int AddVillain(SqlConnection sqlConnection, string villainName)
        {
            SqlCommand checkIfVillainExistCommand = new SqlCommand(Queries.CHECK_IF_VILLAIN_EXIST_QUERY, sqlConnection);
            checkIfVillainExistCommand.Parameters.AddWithValue("@Name", villainName);

            object villainIdObj = checkIfVillainExistCommand.ExecuteScalar();

            if (villainIdObj == null)
            {
                SqlCommand addVillainCommand = new SqlCommand(Queries.INSERT_VILLAIN_QUERY, sqlConnection);
                addVillainCommand.Parameters.AddWithValue("@villainName", villainName);
                addVillainCommand.ExecuteNonQuery();
                villainIdObj = checkIfVillainExistCommand.ExecuteScalar();

                Console.WriteLine($"Villain {villainName} was added to the database.");
            }

            return (int)villainIdObj;
        }
        //=====================================================================================================
    }
}