using MySql.Data.MySqlClient;

namespace CST_350_Milestone.Models
{
    public class UserDAO
    {
        private string connectionString;

        public UserDAO(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public void AddUser(UserModel user)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                string query =
                    " INSERT INTO Users (FirstName, LastName, Sex, Age, State, Email, Username, Password) " +
                    " VALUES (@FirstName, @LastName, @Sex, @Age, @State, @Email, @Username, @Password)";

                MySqlCommand cmd = new MySqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@FirstName", user.FirstName);
                cmd.Parameters.AddWithValue("@LastName", user.LastName);
                cmd.Parameters.AddWithValue("@Sex", user.Sex);
                cmd.Parameters.AddWithValue("@Age", user.Age);
                cmd.Parameters.AddWithValue("@State", user.State);
                cmd.Parameters.AddWithValue("@Email", user.Email);
                cmd.Parameters.AddWithValue("@Username", user.Username);
                cmd.Parameters.AddWithValue("@Password", user.GetPasswordHash());

                cmd.ExecuteNonQuery();
            }
        }

        public UserModel GetUserByUsername(string username)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                string query = "SELECT * FROM Users WHERE Username = @Username";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Username", username);

                MySqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    UserModel user = new UserModel
                    {
                        Id       = reader.GetInt32("Id"),
                        FirstName = reader.GetString("FirstName"),
                        LastName  = reader.GetString("LastName"),
                        Sex       = reader.GetString("Sex"),
                        Age       = reader.GetInt32("Age"),
                        State     = reader.GetString("State"),
                        Email     = reader.GetString("Email"),
                        Username  = reader.GetString("Username"),
                    };
                    user.SetPasswordHash(reader.GetString("Password"));
                    return user;
                }
            }
            return null;
        }

        public void UpdateUser(UserModel user)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                string query =
                    "UPDATE Users SET FirstName=@FirstName, LastName=@LastName, Sex=@Sex, " +
                    "Age=@Age, State=@State, Email=@Email, Username=@Username WHERE Id=@Id";

                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@FirstName", user.FirstName);
                cmd.Parameters.AddWithValue("@LastName",  user.LastName);
                cmd.Parameters.AddWithValue("@Sex",       user.Sex);
                cmd.Parameters.AddWithValue("@Age",       user.Age);
                cmd.Parameters.AddWithValue("@State",     user.State);
                cmd.Parameters.AddWithValue("@Email",     user.Email);
                cmd.Parameters.AddWithValue("@Username",  user.Username);
                cmd.Parameters.AddWithValue("@Id",        user.Id);

                cmd.ExecuteNonQuery();
            }
        }

        public UserModel GetUser(string username, string password)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                string query = "SELECT * FROM Users WHERE Username = @Username";

                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Username", username);

                MySqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    UserModel user = new UserModel
                    {
                        Id = reader.GetInt32("Id"),
                        FirstName = reader.GetString("FirstName"),
                        LastName = reader.GetString("LastName"),
                        Sex = reader.GetString("Sex"),
                        Age = reader.GetInt32("Age"),
                        State = reader.GetString("State"),
                        Email = reader.GetString("Email"),
                        Username = reader.GetString("Username"),
                    };
                    user.SetPasswordHash(reader.GetString("Password"));

                    if (user.VerifyPassword(password))
                        return user;
                }
            }

            return null;
        }

    }
}
