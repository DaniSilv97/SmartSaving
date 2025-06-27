namespace SmartSaving.Models {
    public class Transaction {
        public enum TransactionTypes : byte {
            Income = 1,
            Expense = 2
        }

        private Guid _guidTransaction;
        private decimal _value;
        private TransactionTypes _transactionType;
        private Guid _guidTracker;
        private DateTime _date;
        private string _description;

        public string GuidTransaction {
            get { return _guidTransaction.ToString(); }
        }

        public Guid GuidTransactionAsGuid {
            get { return _guidTransaction; }
        }

        public decimal Value {
            get { return _value; }
            set {
                if (value < 0) {
                    throw new ArgumentException("Value cannot be negative");
                }
                _value = Math.Round(value, 3);
            }
        }

        public TransactionTypes TransactionType {
            get { return _transactionType; }
            set {
                if (!Enum.IsDefined(typeof(TransactionTypes), value)) {
                    throw new ArgumentException("Invalid transaction type specified");
                }
                _transactionType = value;
            }
        }

        public string GuidTracker {
            get { return _guidTracker.ToString(); }
            set {
                if (!Guid.TryParse(value, out _guidTracker)) {
                    throw new ArgumentException("Invalid tracker GUID format");
                }
            }
        }

        public DateTime Date {
            get { return _date; }
            set { _date = value; }
        }

        public string Description {
            get { return _description; }
            set {
                if (string.IsNullOrWhiteSpace(value)) {
                    _description = string.Empty;
                }
                else {
                    if (value.Length > 255) {
                        throw new ArgumentException("Description cannot exceed 255 characters");
                    }
                    _description = value.Trim();
                }
            }
        }

        public Transaction() {
            _guidTransaction = Guid.Empty;
            _value = 0.0M;
            _transactionType = TransactionTypes.Income;
            _guidTracker = Guid.Empty;
            _date = DateTime.Now;
            _description = string.Empty;
        }

        public Transaction(string guidTransaction) {
            Guid.TryParse(guidTransaction, out _guidTransaction);
            _value = 0.0M;
            _transactionType = TransactionTypes.Income;
            _guidTracker = Guid.Empty;
            _date = DateTime.Now;
            _description = string.Empty;
        }

        public Transaction(Guid guidTransaction) {
            _guidTransaction = guidTransaction;
            _value = 0.0M;
            _transactionType = TransactionTypes.Income;
            _guidTracker = Guid.Empty;
            _date = DateTime.Now;
            _description = string.Empty;
        }
    }
}