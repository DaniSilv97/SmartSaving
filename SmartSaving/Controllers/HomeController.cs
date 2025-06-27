using Microsoft.AspNetCore.Mvc;
using SmartSaving.Models;

namespace SmartSaving.Controllers {
    public class HomeController : GenericBaseController {
        public IActionResult Index() {
            // Only load data for authenticated users (not guests)
            if (IsAuthenticated()) {
                TrackerHelper trackerHelper = new TrackerHelper();

                // Get user's trackers
                List<Tracker> userTrackers = trackerHelper.listByUser(_auth.GuidUser);

                // Calculate stats
                ViewBag.TrackerCount = userTrackers.Count;

                // Find current month's tracker (if any)
                DateTime currentMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                Tracker? currentMonthTracker = userTrackers
                    .FirstOrDefault(t => t.Date.Year == currentMonth.Year && t.Date.Month == currentMonth.Month);

                if (currentMonthTracker != null) {
                    ViewBag.CurrentMonthBalance = currentMonthTracker.Balance;

                    // Calculate savings rate (balance / income * 100)
                    if (currentMonthTracker.TotalIncome > 0) {
                        ViewBag.SavingsRate = Math.Round((currentMonthTracker.Balance / currentMonthTracker.TotalIncome) * 100, 1);
                    }
                    else {
                        ViewBag.SavingsRate = 0;
                    }
                }
                else {
                    ViewBag.CurrentMonthBalance = 0;
                    ViewBag.SavingsRate = 0;
                }

                // Calculate total balance across all trackers
                ViewBag.TotalBalance = userTrackers.Sum(t => t.Balance);

                // Calculate average monthly income
                if (userTrackers.Any()) {
                    ViewBag.AverageMonthlyIncome = Math.Round(userTrackers.Average(t => t.TotalIncome), 2);
                    ViewBag.AverageMonthlyExpenses = Math.Round(userTrackers.Average(t => t.TotalExpenses), 2);
                }
                else {
                    ViewBag.AverageMonthlyIncome = 0;
                    ViewBag.AverageMonthlyExpenses = 0;
                }

                // Get most recent tracker for additional stats
                if (userTrackers.Any()) {
                    Tracker mostRecentTracker = userTrackers.OrderByDescending(t => t.Date).First();
                    ViewBag.MostRecentTracker = mostRecentTracker;
                    ViewBag.LastActivityDate = mostRecentTracker.Date;
                }
            }

            return View();
        }
    }
}