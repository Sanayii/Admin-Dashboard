using Admin_Dashboard.Models;
using AspNetCoreGeneratedDocument;

namespace Admin_Dashboard.Services
{
    public interface INotificationService
    {
        Task SendNotification(Notification notification);
    }
}
