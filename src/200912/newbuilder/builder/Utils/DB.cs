using Npgsql;

namespace Builder {

    public static class DB {
        public static NpgsqlDataReader GetRows(string query) {
            NpgsqlCommand command = new NpgsqlCommand(query, Constants.Connection);

            NpgsqlDataReader dr = command.ExecuteReader();
            command.Dispose();
            return dr;
        }


    }
}
