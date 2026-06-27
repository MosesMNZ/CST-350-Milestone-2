namespace CST_350_Milestone.Models
{
    public class UserModel
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Sex { get; set; }

        public int Age { get; set; }

        public string State { get; set; }

        public string Email { get; set; }

        public string Username { get; set; }

        private string _passwordHash;

        public void SetPassword(string password)
        {
            _passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
        }

        public bool VerifyPassword(string password)
        {
            return BCrypt.Net.BCrypt.Verify(password, _passwordHash);
        }
    }
}
