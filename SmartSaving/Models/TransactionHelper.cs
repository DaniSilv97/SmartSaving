using Microsoft.Data.SqlClient;
using SmartSaving.Models;
using System.Data;

namespace SmartSaving.Models {
    public class TransactionHelper : BaseHelper {

        public List<Transaction> list() {
            DataTable dt = new DataTable();
            List<Transaction> exitList = new List<Transaction>();

            SqlDataAdapter dataAdapter = new SqlDataAdapter();
            SqlCommand command = new SqlCommand();
            SqlConnection connection = new SqlConnection(Conector);

            command.CommandType = CommandType.StoredProcedure;
            command.Connection = connection;
            command.CommandText = "QTransaction_List";

            dataAdapter.SelectCommand = command;
            dataAdapter.Fill(dt);

            foreach (DataRow row in dt.Rows) {
                Transaction transaction = new Transaction(row["guidTransaction"].ToString());
                transaction.Value = Convert.ToDecimal(row["value"]);
                transaction.TransactionType = (Transaction.TransactionTypes)Convert.ToByte(row["idTransactionType"]);
                transaction.GuidTracker = row["guidTracker"].ToString();
                transaction.Date = Convert.ToDateTime(row["date"]);

                exitList.Add(transaction);
            }

            return exitList;
        }

        public List<Transaction> listByTracker(string guidTracker) {
            DataTable dt = new DataTable();
            List<Transaction> exitList = new List<Transaction>();

            SqlDataAdapter dataAdapter = new SqlDataAdapter();
            SqlCommand command = new SqlCommand();
            SqlConnection connection = new SqlConnection(Conector);

            command.CommandType = CommandType.StoredProcedure;
            command.Connection = connection;
            command.CommandText = "QTransaction_ListByTracker";
            command.Parameters.AddWithValue("@GuidTracker", guidTracker);

            dataAdapter.SelectCommand = command;
            dataAdapter.Fill(dt);

            foreach (DataRow row in dt.Rows) {
                Transaction transaction = new Transaction(row["guidTransaction"].ToString());
                transaction.Value = Convert.ToDecimal(row["value"]);
                transaction.TransactionType = (Transaction.TransactionTypes)Convert.ToByte(row["idTransactionType"]);
                transaction.GuidTracker = row["guidTracker"].ToString();
                transaction.Date = Convert.ToDateTime(row["date"]);

                exitList.Add(transaction);
            }

            return exitList;
        }

        public List<Transaction> listByType(Transaction.TransactionTypes transactionType) {
            DataTable dt = new DataTable();
            List<Transaction> exitList = new List<Transaction>();

            SqlDataAdapter dataAdapter = new SqlDataAdapter();
            SqlCommand command = new SqlCommand();
            SqlConnection connection = new SqlConnection(Conector);

            command.CommandType = CommandType.StoredProcedure;
            command.Connection = connection;
            command.CommandText = "QTransaction_ListByType";
            command.Parameters.AddWithValue("@IdTransactionType", (byte)transactionType);

            dataAdapter.SelectCommand = command;
            dataAdapter.Fill(dt);

            foreach (DataRow row in dt.Rows) {
                Transaction transaction = new Transaction(row["guidTransaction"].ToString());
                transaction.Value = Convert.ToDecimal(row["value"]);
                transaction.TransactionType = (Transaction.TransactionTypes)Convert.ToByte(row["idTransactionType"]);
                transaction.GuidTracker = row["guidTracker"].ToString();
                transaction.Date = Convert.ToDateTime(row["date"]);

                exitList.Add(transaction);
            }

            return exitList;
        }

        public void delete(string guidTransactionToDel) {
            SqlCommand command = new SqlCommand();
            SqlConnection connection = new SqlConnection(Conector);

            command.CommandType = CommandType.StoredProcedure;
            command.Connection = connection;
            command.CommandText = "QTransaction_Delete";
            command.Parameters.AddWithValue("@Guid", guidTransactionToDel);

            connection.Open();

            try {
                command.ExecuteNonQuery();
            }
            catch (Exception ex) {
                throw new Exception("Error deleting transaction: " + ex.Message);
            }
            finally {
                connection.Close();
                connection.Dispose();
            }
        }

        public Transaction? get(string guidTransactionToFind) {
            DataTable dt = new DataTable();

            SqlDataAdapter dataAdapter = new SqlDataAdapter();
            SqlCommand command = new SqlCommand();
            SqlConnection connection = new SqlConnection(Conector);

            command.CommandType = CommandType.StoredProcedure;
            command.Connection = connection;
            command.CommandText = "QTransaction_Get";
            command.Parameters.AddWithValue("@Guid", guidTransactionToFind);

            dataAdapter.SelectCommand = command;
            dataAdapter.Fill(dt);

            if (dt.Rows.Count == 1) {
                DataRow row = dt.Rows[0];
                Transaction transaction = new Transaction(row["guidTransaction"].ToString());
                transaction.Value = Convert.ToDecimal(row["value"]);
                transaction.TransactionType = (Transaction.TransactionTypes)Convert.ToByte(row["idTransactionType"]);
                transaction.GuidTracker = row["guidTracker"].ToString();
                transaction.Date = Convert.ToDateTime(row["date"]);

                return transaction;
            }

            return null;
        }

        public Boolean create(Transaction transactionToStore) {
            Boolean result = false;

            SqlCommand command = new SqlCommand();
            SqlConnection connection = new SqlConnection(Conector);

            command.CommandType = CommandType.StoredProcedure;
            command.Connection = connection;
            command.CommandText = "QTransaction_Create";

            command.Parameters.AddWithValue("@Value", transactionToStore.Value);
            command.Parameters.AddWithValue("@IdTransactionType", (byte)transactionToStore.TransactionType);
            command.Parameters.AddWithValue("@GuidTracker", transactionToStore.GuidTracker);
            command.Parameters.AddWithValue("@Date", transactionToStore.Date);

            try {
                connection.Open();
                command.ExecuteNonQuery();
                result = true;
            }
            catch (Exception ex) {
                throw new Exception("Error creating transaction: " + ex.Message);
            }
            finally {
                connection.Close();
                connection.Dispose();
            }

            return result;
        }

        public Boolean update(Transaction transactionToUpdate, string transactionToUpdateGuid) {
            Boolean result = false;
            Transaction? transactionToSave = get(transactionToUpdateGuid);
            if (transactionToSave == null) {
                return result;
            }

            transactionToSave.Value = transactionToUpdate.Value;
            transactionToSave.TransactionType = transactionToUpdate.TransactionType;
            transactionToSave.GuidTracker = transactionToUpdate.GuidTracker;
            transactionToSave.Date = transactionToUpdate.Date;

            SqlCommand command = new SqlCommand();
            SqlConnection connection = new SqlConnection(Conector);

            command.CommandType = CommandType.StoredProcedure;
            command.Connection = connection;
            command.CommandText = "QTransaction_Update";

            command.Parameters.AddWithValue("@Guid", transactionToSave.GuidTransaction);
            command.Parameters.AddWithValue("@Value", transactionToSave.Value);
            command.Parameters.AddWithValue("@IdTransactionType", (byte)transactionToSave.TransactionType);
            command.Parameters.AddWithValue("@GuidTracker", transactionToSave.GuidTracker);
            command.Parameters.AddWithValue("@Date", transactionToSave.Date);

            try {
                connection.Open();
                command.ExecuteNonQuery();
                result = true;
            }
            catch (Exception ex) {
                throw new Exception("Error updating transaction: " + ex.Message);
            }
            finally {
                connection.Close();
                connection.Dispose();
            }

            return result;
        }

        public Boolean store(Transaction transactionToStore, string transactionToUpdateGuid = "") {
            Boolean result = false;

            if (string.IsNullOrEmpty(transactionToUpdateGuid) || transactionToUpdateGuid == Guid.Empty.ToString()) {
                result = create(transactionToStore);
            }
            else {
                result = update(transactionToStore, transactionToUpdateGuid);
            }

            return result;
        }

        public int getTransactionCount() {
            int count = 0;
            SqlCommand command = new SqlCommand();
            SqlConnection connection = new SqlConnection(Conector);
            command.CommandType = CommandType.StoredProcedure;
            command.Connection = connection;
            command.CommandText = "QTransaction_GetCount";

            connection.Open();
            count = (int)command.ExecuteScalar();
            connection.Close();
            connection.Dispose();

            return count;
        }

        public int getTransactionCountByTracker(string guidTracker) {
            int count = 0;
            SqlCommand command = new SqlCommand();
            SqlConnection connection = new SqlConnection(Conector);
            command.CommandType = CommandType.StoredProcedure;
            command.Connection = connection;
            command.CommandText = "QTransaction_GetCountByTracker";
            command.Parameters.AddWithValue("@GuidTracker", guidTracker);

            connection.Open();
            count = (int)command.ExecuteScalar();
            connection.Close();
            connection.Dispose();

            return count;
        }

        public decimal getTotalValueByTracker(string guidTracker) {
            decimal value = 0;
            SqlCommand command = new SqlCommand();
            SqlConnection connection = new SqlConnection(Conector);
            command.CommandType = CommandType.StoredProcedure;
            command.Connection = connection;
            command.CommandText = "QTransaction_GetTotalValueByTracker";
            command.Parameters.AddWithValue("@GuidTracker", guidTracker);

            connection.Open();
            try {
                value = (decimal)command.ExecuteScalar();
            }
            catch {
                value = 0.0m;
            }
            connection.Close();
            connection.Dispose();

            return value;
        }

        public decimal getTotalValueByType(Transaction.TransactionTypes transactionType) {
            decimal value = 0;
            SqlCommand command = new SqlCommand();
            SqlConnection connection = new SqlConnection(Conector);
            command.CommandType = CommandType.StoredProcedure;
            command.Connection = connection;
            command.CommandText = "QTransaction_GetTotalValueByType";
            command.Parameters.AddWithValue("@IdTransactionType", (byte)transactionType);

            connection.Open();
            try {
                value = (decimal)command.ExecuteScalar();
            }
            catch {
                value = 0.0m;
            }
            connection.Close();
            connection.Dispose();

            return value;
        }

        public decimal getTotalIncomeByTracker(string guidTracker) {
            return getTotalValueByTrackerAndType(guidTracker, Transaction.TransactionTypes.Income);
        }

        public decimal getTotalExpensesByTracker(string guidTracker) {
            return getTotalValueByTrackerAndType(guidTracker, Transaction.TransactionTypes.Expense);
        }

        public decimal getBalanceByTracker(string guidTracker) {
            return getTotalIncomeByTracker(guidTracker) - getTotalExpensesByTracker(guidTracker);
        }

        private decimal getTotalValueByTrackerAndType(string guidTracker, Transaction.TransactionTypes transactionType) {
            decimal value = 0;
            SqlCommand command = new SqlCommand();
            SqlConnection connection = new SqlConnection(Conector);
            command.CommandType = CommandType.StoredProcedure;
            command.Connection = connection;
            command.CommandText = "QTransaction_GetTotalValueByTrackerAndType";
            command.Parameters.AddWithValue("@GuidTracker", guidTracker);
            command.Parameters.AddWithValue("@IdTransactionType", (byte)transactionType);

            connection.Open();
            try {
                value = (decimal)command.ExecuteScalar();
            }
            catch {
                value = 0.0m;
            }
            connection.Close();
            connection.Dispose();

            return value;
        }
    }
}