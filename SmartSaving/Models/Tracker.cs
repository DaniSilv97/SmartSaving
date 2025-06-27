namespace SmartSaving.Models {
    public class Tracker {
        private Guid _guidTracker;
        private DateTime _date;
        private User _user;
        private List<Transaction> _transactions;

        public string GuidTracker {
            get { return _guidTracker.ToString(); }
        }

        public DateTime Date {
            get { return _date; }
            set { _date = value; }
        }

        public User User {
            get { return _user; }
            set {
                if (value == null) {
                    throw new ArgumentNullException(nameof(User), "User cannot be null");
                }
                _user = value;
            }
        }

        public List<Transaction> Transactions {
            get { return _transactions; }
            set { _transactions = value ?? new List<Transaction>(); }
        }

        public List<Transaction> Incomes {
            get { return _transactions.Where(t => t.TransactionType == Transaction.TransactionTypes.Income).ToList(); }
        }

        public List<Transaction> Expenses {
            get { return _transactions.Where(t => t.TransactionType == Transaction.TransactionTypes.Expense).ToList(); }
        }

        public decimal TotalIncome {
            get { return Incomes.Sum(t => t.Value); }
        }

        public decimal TotalExpenses {
            get { return Expenses.Sum(t => t.Value); }
        }

        public decimal Balance {
            get { return TotalIncome - TotalExpenses; }
        }

        public Tracker() {
            _guidTracker = Guid.Empty;
            _date = DateTime.Today;
            _transactions = new List<Transaction>();
            _user = new User();
        }

        public Tracker(string guidTracker) {
            Guid.TryParse(guidTracker, out _guidTracker);
            _date = DateTime.Today;
            _transactions = new List<Transaction>();
            _user = new User();
        }

        public Tracker(Guid guidTracker) {
            _guidTracker = guidTracker;
            _date = DateTime.Today;
            _transactions = new List<Transaction>();
            _user = new User();
        }
    }
}