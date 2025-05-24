using Microsoft.AspNetCore.Mvc;
using Admin_Dashboard.Models;
using Admin_Dashboard.Enums;

namespace Admin_Dashboard.Controllers;

public class HomeController : Controller
{
    private readonly SanayiiContext _db;

    public HomeController(SanayiiContext db)
    {
        _db = db;
    }

    private string[] GetLast30DaysLabels()
    {
        return Enumerable.Range(0, 30)
            .Select(i => DateTime.Now.AddDays(-i).ToString("dd MMM"))
            .Reverse()
            .ToArray();
    }

    private int[] GetLast30DaysOrdersCount()
    {
        var counts = Enumerable.Range(0, 30)
            .Select(i => _db.ServiceRequestPayments
                .Count(o => o.CreatedAt.Date == DateTime.Now.AddDays(-i).Date))
            .Reverse()
            .ToArray();

        // Debugging: ????? ???????? ??????
        Console.WriteLine("Orders count for last 30 days:");
        for (int i = 0; i < 30; i++)
        {
            Console.WriteLine($"{DateTime.Now.AddDays(-29 + i).ToString("dd MMM")}: {counts[i]}");
        }

        return counts;
    }

    public IActionResult Index()
    {
        var model = new DashboardViewModel
        {
            // ?????????? ????????
            TotalAdmins = _db.Admins.Count(),
            TotalCustomers = _db.Customers.Count(),
            TotalArtisans = _db.Artisans.Count(),
            TotalServiceCategories = _db.Categories.Count(),
            TotalServicesRequests = _db.ServiceRequestPayments.Count(),

            // ????? ???????
            RecentOrders = _db.ServiceRequestPayments
                .OrderByDescending(o => o.CreatedAt)
                .Take(10)
                .Select(o => new OrderReport
                {
                    Id = o.PaymentId,
                    ServiceName = o.Service.ServiceName,
                    CreatedAt = o.CreatedAt,
                    Status = o.Status.ToString()
                })
                .ToList(),

            // ?????? ????? ??????? ?? 30 ???
            ChartData = new ChartData
            {
                Labels = GetLast30DaysLabels(),
                Values = GetLast30DaysOrdersCount()
            }
        };

        return View(model);
    }
}