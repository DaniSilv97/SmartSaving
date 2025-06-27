using Microsoft.AspNetCore.Mvc;
using SmartSaving.Models;

namespace SmartSaving.Controllers {
    public class HomeController : GenericBaseController {
        public IActionResult Index() {
            // Only load data for authenticated users (not guests)
            if (IsAuthenticated()) {
                TrackerHelper trackerHelper = new TrackerHelper();
                TransactionHelper transactionHelper = new TransactionHelper();
                string userId = _auth.GuidUser;

                // Use efficient stored procedure for tracker count
                ViewBag.TrackerCount = trackerHelper.getTrackerCountByUser(userId);

                // Get lightweight tracker list (without transactions loaded)
                List<Tracker> userTrackers = GetUserTrackersLightweight(userId);

                if (userTrackers.Any()) {
                    // Find current month's tracker (if any)
                    DateTime currentMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                    Tracker? currentMonthTracker = userTrackers
                        .FirstOrDefault(t => t.Date.Year == currentMonth.Year && t.Date.Month == currentMonth.Month);

                    // Current month stats
                    if (currentMonthTracker != null) {
                        decimal currentMonthBalance = transactionHelper.getBalanceByTracker(currentMonthTracker.GuidTracker);
                        decimal currentMonthIncome = transactionHelper.getTotalIncomeByTracker(currentMonthTracker.GuidTracker);

                        ViewBag.CurrentMonthBalance = currentMonthBalance;

                        // Calculate savings rate (balance / income * 100)
                        if (currentMonthIncome > 0) {
                            ViewBag.SavingsRate = Math.Round((currentMonthBalance / currentMonthIncome) * 100, 1);
                        }
                        else {
                            ViewBag.SavingsRate = 0;
                        }
                    }
                    else {
                        ViewBag.CurrentMonthBalance = 0;
                        ViewBag.SavingsRate = 0;
                    }

                    // Calculate totals across all trackers using efficient methods
                    decimal totalBalance = 0;
                    decimal totalIncome = 0;
                    decimal totalExpenses = 0;

                    foreach (var tracker in userTrackers) {
                        totalBalance += transactionHelper.getBalanceByTracker(tracker.GuidTracker);
                        totalIncome += transactionHelper.getTotalIncomeByTracker(tracker.GuidTracker);
                        totalExpenses += transactionHelper.getTotalExpensesByTracker(tracker.GuidTracker);
                    }

                    ViewBag.TotalBalance = totalBalance;
                    ViewBag.AverageMonthlyIncome = Math.Round(totalIncome / userTrackers.Count, 2);
                    ViewBag.AverageMonthlyExpenses = Math.Round(totalExpenses / userTrackers.Count, 2);

                    // Get most recent tracker for additional stats
                    Tracker mostRecentTracker = userTrackers.OrderByDescending(t => t.Date).First();

                    // Calculate recent tracker specific stats
                    decimal recentIncome = transactionHelper.getTotalIncomeByTracker(mostRecentTracker.GuidTracker);
                    decimal recentExpenses = transactionHelper.getTotalExpensesByTracker(mostRecentTracker.GuidTracker);
                    decimal recentBalance = transactionHelper.getBalanceByTracker(mostRecentTracker.GuidTracker);

                    ViewBag.MostRecentTracker = mostRecentTracker;
                    ViewBag.RecentTrackerIncome = recentIncome;
                    ViewBag.RecentTrackerExpenses = recentExpenses;
                    ViewBag.RecentTrackerBalance = recentBalance;
                    ViewBag.LastActivityDate = mostRecentTracker.Date;
                }
                else {
                    // No trackers found - set default values
                    ViewBag.CurrentMonthBalance = 0;
                    ViewBag.SavingsRate = 0;
                    ViewBag.TotalBalance = 0;
                    ViewBag.AverageMonthlyIncome = 0;
                    ViewBag.AverageMonthlyExpenses = 0;
                    ViewBag.MostRecentTracker = null;
                    ViewBag.RecentTrackerIncome = 0;
                    ViewBag.RecentTrackerExpenses = 0;
                    ViewBag.RecentTrackerBalance = 0;
                    ViewBag.LastActivityDate = null;
                }
            }
            return View();
        }

        private List<Tracker> GetUserTrackersLightweight(string guidUser) {
            TrackerHelper trackerHelper = new TrackerHelper();

            List<Tracker> userTrackers = trackerHelper.listByUser(guidUser);

            foreach (var tracker in userTrackers) {
                tracker.Transactions = null; // Free up memory
            }

            return userTrackers;
        }
    }
}