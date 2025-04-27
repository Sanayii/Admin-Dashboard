using Admin_Dashboard.Models;
using Admin_Dashboard.UnitOfWorks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Admin_Dashboard.Services
{
    public class NotificationService : INotificationService
    {
        private readonly ILogger<NotificationService> _logger;
        private readonly UnitOFWork _unitOfWork;
        private readonly IHttpClientFactory _httpClientFactory;

        public NotificationService(UnitOFWork unitOfWork, ILogger<NotificationService> logger, IHttpClientFactory httpClientFactory)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }

        public async Task SendNotification(Notification notification)
        {
            // Save the notification to the database
            _unitOfWork._NotificationRepo.Add(notification);
             _unitOfWork.save();

            _logger.LogInformation($"Notification sent to CustomerId={notification.UserId}: Your service request status has been updated to: {notification.Content}");

            // Send Notification via HTTP to Web API (SignalR broadcast)
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri("https://localhost:7234/");  

            var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(notification), System.Text.Encoding.UTF8, "application/json");
            var response = await client.PostAsync("api/notification/send", content);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Notification successfully sent to Web API.");
            }
            else
            {
                _logger.LogError($"Failed to send notification. Status code: {response.StatusCode}");
            }
        }
    }
}
