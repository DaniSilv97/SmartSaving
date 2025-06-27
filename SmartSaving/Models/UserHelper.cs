using Microsoft.Data.SqlClient;
using System.Data;
using System.Text.Json;
using System.Security.Cryptography;
using System.Text;

namespace SmartSaving.Models {
    public class UserHelper : BaseHelper {

        public User setGuest() {
            User guest = new User();
            guest.Name = "Guest";
            guest.Email = "guest@smart.saving.pt";
            guest.Role = User.Roles.User;
            return guest;
        }

        public User? authUser(string email, string password) {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password)) {
                return setGuest();
            }

            // For backward compatibility with old hardcoded accounts during transition
            if (email == "admin@istec.pt" && password == "1234") {
                email = "admin@smartsaving.pt";
            }
            if (email == "user@istec.pt" && password == "4321") {
                email = "user@smartsaving.pt";
            }

            User? user = getUserByEmail(email);
            if (user != null && verifyPassword(password, user.Password)) {
                // Clear password for security
                user.Password = "";
                return user;
            }

            return setGuest();
        }

        public User? getUserByEmail(string email) {
            if (string.IsNullOrWhiteSpace(email)) {
                return null;
            }

            DataTable dt = new DataTable();
            SqlDataAdapter dataAdapter = new SqlDataAdapter();
            SqlCommand command = new SqlCommand();
            SqlConnection connection = new SqlConnection(Conector);

            command.CommandType = CommandType.StoredProcedure;
            command.Connection = connection;
            command.CommandText = "QUser_GetByEmail";
            command.Parameters.AddWithValue("@Email", email.Trim().ToLower());

            dataAdapter.SelectCommand = command;
            dataAdapter.Fill(dt);

            if (dt.Rows.Count == 1) {
                DataRow row = dt.Rows[0];
                User user = new User(row["guidUser"].ToString());
                user.Name = row["name"].ToString();
                user.Email = row["email"].ToString();
                user.Password = row["password"].ToString();
                user.Role = (User.Roles)Convert.ToByte(row["idRole"]);
                return user;
            }

            return null;
        }

        public User? get(string guidUserToFind) {
            if (string.IsNullOrWhiteSpace(guidUserToFind)) {
                return null;
            }

            DataTable dt = new DataTable();
            SqlDataAdapter dataAdapter = new SqlDataAdapter();
            SqlCommand command = new SqlCommand();
            SqlConnection connection = new SqlConnection(Conector);

            command.CommandType = CommandType.StoredProcedure;
            command.Connection = connection;
            command.CommandText = "QUser_Get";
            command.Parameters.AddWithValue("@Guid", guidUserToFind);

            dataAdapter.SelectCommand = command;
            dataAdapter.Fill(dt);

            if (dt.Rows.Count == 1) {
                DataRow row = dt.Rows[0];
                User user = new User(row["guidUser"].ToString());
                user.Name = row["name"].ToString();
                user.Email = row["email"].ToString();
                user.Password = row["password"].ToString();
                user.Role = (User.Roles)Convert.ToByte(row["idRole"]);
                return user;
            }

            return null;
        }

        public Boolean create(User userToStore) {
            if (userToStore == null) {
                return false;
            }

            Boolean result = false;
            SqlCommand command = new SqlCommand();
            SqlConnection connection = new SqlConnection(Conector);

            command.CommandType = CommandType.StoredProcedure;
            command.Connection = connection;
            command.CommandText = "QUser_Create";

            command.Parameters.AddWithValue("@Name", userToStore.Name);
            command.Parameters.AddWithValue("@Email", userToStore.Email);
            command.Parameters.AddWithValue("@Password", hashPassword(userToStore.Password));
            command.Parameters.AddWithValue("@IdRole", (byte)userToStore.Role);

            try {
                connection.Open();
                command.ExecuteNonQuery();
                result = true;
            }
            catch (Exception ex) {
                throw new Exception("Error creating user: " + ex.Message);
            }
            finally {
                connection.Close();
                connection.Dispose();
            }

            return result;
        }

        public Boolean update(User userToUpdate, string userToUpdateGuid) {
            if (userToUpdate == null || string.IsNullOrWhiteSpace(userToUpdateGuid)) {
                return false;
            }

            Boolean result = false;
            User? userToSave = get(userToUpdateGuid);
            if (userToSave == null) {
                return result;
            }

            userToSave.Name = userToUpdate.Name;
            userToSave.Email = userToUpdate.Email;
            userToSave.Role = userToUpdate.Role;

            // Only update password if a new one is provided
            if (!string.IsNullOrWhiteSpace(userToUpdate.Password)) {
                userToSave.Password = hashPassword(userToUpdate.Password);
            }

            SqlCommand command = new SqlCommand();
            SqlConnection connection = new SqlConnection(Conector);

            command.CommandType = CommandType.StoredProcedure;
            command.Connection = connection;
            command.CommandText = "QUser_Update";

            command.Parameters.AddWithValue("@Guid", userToSave.GuidUser);
            command.Parameters.AddWithValue("@Name", userToSave.Name);
            command.Parameters.AddWithValue("@Email", userToSave.Email);
            command.Parameters.AddWithValue("@Password", userToSave.Password);
            command.Parameters.AddWithValue("@IdRole", (byte)userToSave.Role);

            try {
                connection.Open();
                command.ExecuteNonQuery();
                result = true;
            }
            catch (Exception ex) {
                throw new Exception("Error updating user: " + ex.Message);
            }
            finally {
                connection.Close();
                connection.Dispose();
            }

            return result;
        }

        public void delete(string guidUserToDel) {
            if (string.IsNullOrWhiteSpace(guidUserToDel)) {
                throw new ArgumentException("User GUID cannot be empty");
            }

            SqlCommand command = new SqlCommand();
            SqlConnection connection = new SqlConnection(Conector);

            command.CommandType = CommandType.StoredProcedure;
            command.Connection = connection;
            command.CommandText = "QUser_Delete";
            command.Parameters.AddWithValue("@Guid", guidUserToDel);

            try {
                connection.Open();
                command.ExecuteNonQuery();
            }
            catch (Exception ex) {
                throw new Exception("Error deleting user: " + ex.Message);
            }
            finally {
                connection.Close();
                connection.Dispose();
            }
        }

        public List<User> list() {
            DataTable dt = new DataTable();
            List<User> exitList = new List<User>();

            SqlDataAdapter dataAdapter = new SqlDataAdapter();
            SqlCommand command = new SqlCommand();
            SqlConnection connection = new SqlConnection(Conector);

            command.CommandType = CommandType.StoredProcedure;
            command.Connection = connection;
            command.CommandText = "QUser_List";

            dataAdapter.SelectCommand = command;
            dataAdapter.Fill(dt);

            foreach (DataRow row in dt.Rows) {
                User user = new User(row["guidUser"].ToString());
                user.Name = row["name"].ToString();
                user.Email = row["email"].ToString();
                user.Role = (User.Roles)Convert.ToByte(row["idRole"]);
                // Don't include password in list results
                exitList.Add(user);
            }

            return exitList;
        }

        public string serializeUser(User user) {
            if (user == null) {
                return string.Empty;
            }

            // Create a copy without sensitive data for serialization
            var userForSerialization = new {
                GuidUser = user.GuidUser,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role
            };

            return JsonSerializer.Serialize(userForSerialization);
        }

        public User? deserializeUser(string userJson) {
            if (string.IsNullOrWhiteSpace(userJson)) {
                return null;
            }

            try {
                // Deserialize to anonymous object structure first
                var jsonData = JsonSerializer.Deserialize<JsonElement>(userJson);

                // Create User object and populate from JSON
                User user = new User(jsonData.GetProperty("GuidUser").GetString() ?? "");
                user.Name = jsonData.GetProperty("Name").GetString() ?? "";
                user.Email = jsonData.GetProperty("Email").GetString() ?? "";

                // Handle Role enum
                var roleValue = jsonData.GetProperty("Role").GetInt32();
                user.Role = (User.Roles)roleValue;

                return user;
            }
            catch (Exception ex) {
                // Log the error for debugging
                System.Diagnostics.Debug.WriteLine($"Deserialization error: {ex.Message}");
                return null;
            }
        }

        private string hashPassword(string password) {
            if (string.IsNullOrWhiteSpace(password)) {
                throw new ArgumentException("Password cannot be empty");
            }

            using (SHA256 sha256Hash = SHA256.Create()) {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++) {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        private bool verifyPassword(string password, string hashedPassword) {
            if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(hashedPassword)) {
                return false;
            }

            string hashOfInput = hashPassword(password);
            return hashOfInput.Equals(hashedPassword, StringComparison.OrdinalIgnoreCase);
        }
    }
}