using Admin_Dashboard.Models;
using System.Threading.Tasks;

namespace Admin_Dashboard.Services
{
    public interface INotificationService
    {
        Task SendNotification(Notification notification);
    }
}