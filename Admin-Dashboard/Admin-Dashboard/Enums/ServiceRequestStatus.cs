namespace Admin_Dashboard.Enums
{
    using System.ComponentModel.DataAnnotations;

    public enum ServiceRequestStatus
    {
        [Display(Name = "Service Requested")]
        ServiceRequested = 1,

        [Display(Name = "In Progress")]
        InProgress,

        [Display(Name = "Artisan on the Way")]
        ArtisanOnTheWay,

        [Display(Name = "Artisan Nearing Location")]
        ArtisanNearingLocation,

        [Display(Name = "Artisan Arrived")]
        ArtisanArrived,

        [Display(Name = "Service Undergoing")]
        ServiceUndergoing,

        [Display(Name = "Service Completed")]
        ServiceCompleted,

        [Display(Name = "Service Cancelled")]
        ServiceCancelled,

        [Display(Name = "Awaiting Approval")]
        AwaitingApproval,

        [Display(Name = "Artisan Busy")]
        ArtisanBusy,

        [Display(Name = "Service done Successfully, you Should complete payment method!")]
        ServiceDonePendingPayment,

        [Display(Name = "Service done Successfully, and payment done Successfully")]
        ServiceDoneAndPaid
    }

}
