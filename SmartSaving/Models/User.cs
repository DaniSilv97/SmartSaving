namespace SmartSaving.Models {
    public class User {
        public enum Roles {
            User = 1,
            Admin = 2,
            Owner = 3
        }

        private Guid _guidUser;
        private string _name;
        private string _email;
        private string _password;
        private Roles _role;
        private List<Tracker> _trackers;

        public string GuidUser {
            get { return _guidUser.ToString(); }
        }

        public string Name {
            get { return _name; }
            set {
                if (string.IsNullOrWhiteSpace(value)) {
                    throw new ArgumentException("Name cannot be empty");
                }
                if (value.Length < 2 || value.Length > 100) {
                    throw new ArgumentException("Name must be between 2 and 100 characters");
                }
                _name = value.Trim();
            }
        }

        public string Email {
            get { return _email; }
            set {
                if (string.IsNullOrWhiteSpace(value)) {
                    throw new ArgumentException("Email cannot be empty");
                }
                if (!IsValidEmail(value)) {
                    throw new ArgumentException("Invalid email format");
                }
                _email = value.Trim().ToLower();
            }
        }

        public string Password {
            get { return _password; }
            set {
                // Allow empty password when clearing for security (after authentication)
                if (string.IsNullOrEmpty(value)) {
                    _password = string.Empty;
                    return;
                }

                if (value.Length < 6) {
                    throw new ArgumentException("Password must be at least 6 characters");
                }
                _password = value;
            }
        }

        public List<Tracker> Trackers {
            get { return _trackers; }
            set { _trackers = value ?? new List<Tracker>(); }
        }

        public Roles Role {
            get { return _role; }
            set {
                if (!Enum.IsDefined(typeof(Roles), value)) {
                    throw new ArgumentException("Invalid role specified");
                }
                _role = value;
            }
        }

        public User() {
            _guidUser = Guid.Empty;
            _name = string.Empty;
            _email = string.Empty;
            _password = string.Empty;
            _trackers = new List<Tracker>();
            _role = Roles.User;
        }

        public User(string guidUser) {
            if (!Guid.TryParse(guidUser, out _guidUser)) {
                _guidUser = Guid.Empty;
            }
            _name = string.Empty;
            _email = string.Empty;
            _password = string.Empty;
            _trackers = new List<Tracker>();
            _role = Roles.User;
        }

        private bool IsValidEmail(string email) {
            try {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch {
                return false;
            }
        }
    }
}