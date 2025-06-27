using Microsoft.Data.SqlClient;
using SmartSaving.Models;
using System.Data;

namespace SmartSaving.Models {
    public class TrackerHelper : BaseHelper {

        public List<Tracker> list() {
            DataTable dt = new DataTable();
            List<Tracker> exitList = new List<Tracker>();

            SqlDataAdapter dataAdapter = new SqlDataAdapter();
            SqlCommand command = new SqlCommand();
            SqlConnection connection = new SqlConnection(Conector);

            command.CommandType = CommandType.StoredProcedure;
            command.Connection = connection;
            command.CommandText = "QTracker_List";

            dataAdapter.SelectCommand = command;
            dataAdapter.Fill(dt);

            foreach (DataRow row in dt.Rows) {
                Tracker tracker = new Tracker(row["guidTracker"].ToString());
                tracker.Date = Convert.ToDateTime(row["date"]);

                // Load user information
                UserHelper userHelper = new UserHelper();
                tracker.User = userHelper.get(row["guidUser"].ToString()) ?? new User();

                exitList.Add(tracker);
            }

            return exitList;
        }

        public List<Tracker> listByUser(string guidUser) {
            DataTable dt = new DataTable();
            List<Tracker> exitList = new List<Tracker>();

            SqlDataAdapter dataAdapter = new SqlDataAdapter();
            SqlCommand command = new SqlCommand();
            SqlConnection connection = new SqlConnection(Conector);

            command.CommandType = CommandType.StoredProcedure;
            command.Connection = connection;
            command.CommandText = "QTracker_ListByUser";
            command.Parameters.AddWithValue("@GuidUser", guidUser);

            dataAdapter.SelectCommand = command;
            dataAdapter.Fill(dt);

            foreach (DataRow row in dt.Rows) {
                Tracker tracker = new Tracker(row["guidTracker"].ToString());
                tracker.Date = Convert.ToDateTime(row["date"]);

                // Load user information
                UserHelper userHelper = new UserHelper();
                tracker.User = userHelper.get(row["guidUser"].ToString()) ?? new User();

                // Load transactions for this tracker
                TransactionHelper transactionHelper = new TransactionHelper();
                tracker.Transactions = transactionHelper.listByTracker(tracker.GuidTracker);

                exitList.Add(tracker);
            }

            return exitList;
        }

        public void delete(string guidTrackerToDel) {
            SqlCommand command = new SqlCommand();
            SqlConnection connection = new SqlConnection(Conector);

            command.CommandType = CommandType.StoredProcedure;
            command.Connection = connection;
            command.CommandText = "QTracker_Delete";
            command.Parameters.AddWithValue("@Guid", guidTrackerToDel);

            connection.Open();

            try {
                command.ExecuteNonQuery();
            }
            catch (Exception ex) {
                throw new Exception("Error deleting tracker: " + ex.Message);
            }
            finally {
                connection.Close();
                connection.Dispose();
            }
        }

        public Tracker? get(string guidTrackerToFind) {
            DataTable dt = new DataTable();

            SqlDataAdapter dataAdapter = new SqlDataAdapter();
            SqlCommand command = new SqlCommand();
            SqlConnection connection = new SqlConnection(Conector);

            command.CommandType = CommandType.StoredProcedure;
            command.Connection = connection;
            command.CommandText = "QTracker_Get";
            command.Parameters.AddWithValue("@Guid", guidTrackerToFind);

            dataAdapter.SelectCommand = command;
            dataAdapter.Fill(dt);

            if (dt.Rows.Count == 1) {
                DataRow row = dt.Rows[0];
                Tracker tracker = new Tracker(row["guidTracker"].ToString());
                tracker.Date = Convert.ToDateTime(row["date"]);

                // Load user information
                UserHelper userHelper = new UserHelper();
                tracker.User = userHelper.get(row["guidUser"].ToString()) ?? new User();

                // Load transactions for this tracker
                TransactionHelper transactionHelper = new TransactionHelper();
                tracker.Transactions = transactionHelper.listByTracker(tracker.GuidTracker);

                return tracker;
            }

            return null;
        }

        public Boolean create(Tracker trackerToStore) {
            Boolean result = false;

            SqlCommand command = new SqlCommand();
            SqlConnection connection = new SqlConnection(Conector);

            command.CommandType = CommandType.StoredProcedure;
            command.Connection = connection;
            command.CommandText = "QTracker_Create";

            command.Parameters.AddWithValue("@Date", trackerToStore.Date);
            command.Parameters.AddWithValue("@GuidUser", new Guid(trackerToStore.User.GuidUser));

            try {
                connection.Open();
                command.ExecuteNonQuery();
                result = true;
            }
            catch (Exception ex) {
                throw new Exception("Error creating tracker: " + ex.Message);
            }
            finally {
                connection.Close();
                connection.Dispose();
            }

            return result;
        }

        public Boolean update(Tracker trackerToUpdate, string trackerToUpdateGuid) {
            Boolean result = false;
            Tracker? trackerToSave = get(trackerToUpdateGuid);
            if (trackerToSave == null) {
                return result;
            }

            trackerToSave.Date = trackerToUpdate.Date;
            trackerToSave.User = trackerToUpdate.User;

            SqlCommand command = new SqlCommand();
            SqlConnection connection = new SqlConnection(Conector);

            command.CommandType = CommandType.StoredProcedure;
            command.Connection = connection;
            command.CommandText = "QTracker_Update";

            command.Parameters.AddWithValue("@Guid", trackerToSave.GuidTracker);
            command.Parameters.AddWithValue("@Date", trackerToSave.Date);
            command.Parameters.AddWithValue("@GuidUser", trackerToSave.User.GuidUser);

            try {
                connection.Open();
                command.ExecuteNonQuery();
                result = true;
            }
            catch (Exception ex) {
                throw new Exception("Error updating tracker: " + ex.Message);
            }
            finally {
                connection.Close();
                connection.Dispose();
            }

            return result;
        }

        public Boolean store(Tracker trackerToStore, string trackerToUpdateGuid = "") {
            Boolean result = false;

            if (string.IsNullOrEmpty(trackerToUpdateGuid) || trackerToUpdateGuid == Guid.Empty.ToString()) {
                result = create(trackerToStore);
            }
            else {
                result = update(trackerToStore, trackerToUpdateGuid);
            }

            return result;
        }

        public int getTrackerCount() {
            int count = 0;
            SqlCommand command = new SqlCommand();
            SqlConnection connection = new SqlConnection(Conector);
            command.CommandType = CommandType.StoredProcedure;
            command.Connection = connection;
            command.CommandText = "QTracker_GetCount";

            connection.Open();
            count = (int)command.ExecuteScalar();
            connection.Close();
            connection.Dispose();

            return count;
        }

        public int getTrackerCountByUser(string guidUser) {
            int count = 0;
            SqlCommand command = new SqlCommand();
            SqlConnection connection = new SqlConnection(Conector);
            command.CommandType = CommandType.StoredProcedure;
            command.Connection = connection;
            command.CommandText = "QTracker_GetCountByUser";
            command.Parameters.AddWithValue("@GuidUser", guidUser);

            connection.Open();
            count = (int)command.ExecuteScalar();
            connection.Close();
            connection.Dispose();

            return count;
        }
    }
}