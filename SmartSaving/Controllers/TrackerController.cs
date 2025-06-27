using Microsoft.AspNetCore.Mvc;
using SmartSaving.Models;

namespace SmartSaving.Controllers {
    public class TrackerController : GenericBaseController {

        // Show list of user's trackers
        public IActionResult Index() {
            if (!IsAuthenticated()) {
                return RedirectToLogin();
            }

            TrackerHelper trackerHelper = new TrackerHelper();
            List<Tracker> trackers = trackerHelper.listByUser(_auth.GuidUser);

            return View(trackers);
        }

        // Create new tracker
        [HttpGet]
        public IActionResult Create() {
            if (!IsAuthenticated()) {
                return RedirectToLogin();
            }

            return View();
        }

        [HttpPost]
        public IActionResult Create(Tracker tracker) {
            if (!IsAuthenticated()) {
                return RedirectToLogin();
            }

            if (ModelState.IsValid) {
                try {
                    // Verify the user exists in the database
                    UserHelper userHelper = new UserHelper();
                    User? dbUser = userHelper.get(_auth.GuidUser);

                    if (dbUser == null) {
                        ViewBag.Error = "User not found in database. Please log out and log back in.";
                        return View(tracker);
                    }

                    tracker.User = dbUser;

                    TrackerHelper helper = new TrackerHelper();
                    if (helper.create(tracker)) {
                        return RedirectToAction("Index");
                    }
                    else {
                        ViewBag.Error = "An error occurred while creating the tracker.";
                    }
                }
                catch (Exception ex) {
                    ViewBag.Error = "Error creating tracker: " + ex.Message;
                }
            }

            return View(tracker);
        }

        // Delete tracker
        public IActionResult Delete(string id) {
            if (!IsAuthenticated()) {
                return RedirectToLogin();
            }

            try {
                TrackerHelper trackerHelper = new TrackerHelper();

                // Verify ownership before deletion
                Tracker? trackerToDelete = trackerHelper.get(id);
                if (trackerToDelete != null && trackerToDelete.User.GuidUser == _auth.GuidUser) {
                    trackerHelper.delete(id);
                }
            }
            catch (Exception ex) {
                ViewBag.Error = "Error deleting tracker: " + ex.Message;
            }

            return RedirectToAction("Index");
        }

        // Inspect tracker - show transactions in table format
        public IActionResult Inspect(string id) {
            if (!IsAuthenticated()) {
                return RedirectToLogin();
            }

            TrackerHelper trackerHelper = new TrackerHelper();
            Tracker? tracker = trackerHelper.get(id);

            if (tracker != null && tracker.User.GuidUser == _auth.GuidUser) {
                // Load transactions for this tracker
                TransactionHelper transactionHelper = new TransactionHelper();
                tracker.Transactions = transactionHelper.listByTracker(id);

                return View(tracker);
            }

            ViewBag.Error = "Tracker not found or access denied.";
            return RedirectToAction("Index");
        }

        // Add transaction to tracker
        [HttpGet]
        public IActionResult AddTransaction(string trackerId) {
            if (!IsAuthenticated()) {
                return RedirectToLogin();
            }

            // Verify tracker ownership
            TrackerHelper trackerHelper = new TrackerHelper();
            Tracker? tracker = trackerHelper.get(trackerId);

            if (tracker == null || tracker.User.GuidUser != _auth.GuidUser) {
                return RedirectToAction("Index");
            }

            Transaction newTransaction = new Transaction();
            newTransaction.GuidTracker = trackerId;
            newTransaction.Date = DateTime.Now;

            ViewBag.TrackerId = trackerId;
            ViewBag.TrackerDate = tracker.Date.ToString("yyyy-MM-dd");

            return View(newTransaction);
        }

        [HttpPost]
        public IActionResult AddTransaction(string trackerId, Transaction transaction) {
            if (!IsAuthenticated()) {
                return RedirectToLogin();
            }

            // Verify tracker ownership
            TrackerHelper trackerHelper = new TrackerHelper();
            Tracker? tracker = trackerHelper.get(trackerId);

            if (tracker == null || tracker.User.GuidUser != _auth.GuidUser) {
                return RedirectToAction("Index");
            }

            if (ModelState.IsValid) {
                try {
                    transaction.GuidTracker = trackerId;

                    TransactionHelper transactionHelper = new TransactionHelper();
                    if (transactionHelper.create(transaction)) {
                        return RedirectToAction("Inspect", new { id = trackerId });
                    }
                    else {
                        ViewBag.Error = "An error occurred while adding the transaction.";
                    }
                }
                catch (Exception ex) {
                    ViewBag.Error = ex.Message;
                }
            }

            ViewBag.TrackerId = trackerId;
            ViewBag.TrackerDate = tracker.Date.ToString("yyyy-MM-dd");
            return View(transaction);
        }

        // Edit transaction
        [HttpGet]
        public IActionResult EditTransaction(string id, string trackerId) {
            if (!IsAuthenticated()) {
                return RedirectToLogin();
            }

            // Verify tracker ownership
            TrackerHelper trackerHelper = new TrackerHelper();
            Tracker? tracker = trackerHelper.get(trackerId);

            if (tracker == null || tracker.User.GuidUser != _auth.GuidUser) {
                return RedirectToAction("Index");
            }

            TransactionHelper transactionHelper = new TransactionHelper();
            Transaction? transaction = transactionHelper.get(id);

            if (transaction != null && transaction.GuidTracker == trackerId) {
                ViewBag.TrackerId = trackerId;
                ViewBag.TrackerDate = tracker.Date.ToString("yyyy-MM-dd");
                return View(transaction);
            }

            return RedirectToAction("Inspect", new { id = trackerId });
        }

        [HttpPost]
        public IActionResult EditTransaction(string id, string trackerId, Transaction postedTransaction) {
            if (!IsAuthenticated()) {
                return RedirectToLogin();
            }

            // Verify tracker ownership
            TrackerHelper trackerHelper = new TrackerHelper();
            Tracker? tracker = trackerHelper.get(trackerId);

            if (tracker == null || tracker.User.GuidUser != _auth.GuidUser) {
                return RedirectToAction("Index");
            }

            if (ModelState.IsValid) {
                try {
                    postedTransaction.GuidTracker = trackerId;

                    TransactionHelper transactionHelper = new TransactionHelper();
                    if (transactionHelper.update(postedTransaction, id)) {
                        return RedirectToAction("Inspect", new { id = trackerId });
                    }
                    else {
                        ViewBag.Error = "An error occurred while updating the transaction.";
                    }
                }
                catch (Exception ex) {
                    ViewBag.Error = ex.Message;
                }
            }

            ViewBag.TrackerId = trackerId;
            ViewBag.TrackerDate = tracker.Date.ToString("yyyy-MM-dd");
            return View(postedTransaction);
        }

        // Delete transaction
        public IActionResult DeleteTransaction(string id, string trackerId) {
            if (!IsAuthenticated()) {
                return RedirectToLogin();
            }

            // Verify tracker ownership
            TrackerHelper trackerHelper = new TrackerHelper();
            Tracker? tracker = trackerHelper.get(trackerId);

            if (tracker == null || tracker.User.GuidUser != _auth.GuidUser) {
                return RedirectToAction("Index");
            }

            try {
                TransactionHelper transactionHelper = new TransactionHelper();
                Transaction? transaction = transactionHelper.get(id);

                if (transaction != null && transaction.GuidTracker == trackerId) {
                    transactionHelper.delete(id);
                }
            }
            catch (Exception ex) {
                ViewBag.Error = "Error deleting transaction: " + ex.Message;
            }

            return RedirectToAction("Inspect", new { id = trackerId });
        }
    }
}