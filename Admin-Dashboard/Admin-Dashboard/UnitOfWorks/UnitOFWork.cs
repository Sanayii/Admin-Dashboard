using Admin_Dashboard.Models;
using Admin_Dashboard.Repository;
using Microsoft.AspNetCore.Identity;

namespace Admin_Dashboard.UnitOfWorks
{
    public class UnitOFWork
    {
        SanayiiContext db;

        GenericRepository<IdentityRole> RoleRepo;
        GenericRepository<UserPhone> UserPhoneRepo;

        GenericRepository<Category> CategoryRepo;
        GenericRepository<Service> ServiceRepo;

        GenericRepository<Payment> PaymentMethodsRepo;
        GenericRepository<Payment> PaymentRepo;

        AdminRepository AdminRepo;
        ServiceRequestPaymentRepository serviceRequestPaymentRepo;
        CustomerRepository CustomerRepo;
        ArtisanRepository ArtisanRepo;

        GenericRepository<Contract> ContractRepo;
        GenericRepository<Violation> ViolationRepo;


        GenericRepository<Discount> DiscountRepo;
        CustomerDiscountRepository CustomerDiscountRepo;

        ReviewRepository reviewRepo;
        GenericRepository<Notification> notificationRepo;


        public UnitOFWork(SanayiiContext db)
        {
            this.db = db;
        }

        public GenericRepository<IdentityRole> _RoleRepo
        {
            get
            {
                if (RoleRepo == null)
                {
                    RoleRepo = new GenericRepository<IdentityRole>(db);
                }
                return RoleRepo;
            }
        }


        public GenericRepository<Category> _CategoryRepo
        {
            get
            {
                if (CategoryRepo == null)
                {
                    CategoryRepo = new GenericRepository<Category>(db);
                }
                return CategoryRepo;
            }
        }
        public GenericRepository<UserPhone> _UserPhoneRepo
        {
            get
            {
                if (UserPhoneRepo == null)
                {
                    UserPhoneRepo = new GenericRepository<UserPhone>(db);
                }
                return UserPhoneRepo;
            }
        }


        public GenericRepository<Service> _ServiceRepo
        {
            get
            {
                if (ServiceRepo == null)
                {
                    ServiceRepo = new GenericRepository<Service>(db);
                }
                return ServiceRepo;
            }
        }

        public GenericRepository<Payment> _paymentMethodsRepo
        {
            get
            {
                if (PaymentMethodsRepo == null)
                {
                    PaymentMethodsRepo = new GenericRepository<Payment>(db);
                }
                return PaymentMethodsRepo;
            }
        }

        public GenericRepository<Payment> _PaymentRepo
        {
            get
            {
                if (PaymentRepo == null)
                {
                    PaymentRepo = new GenericRepository<Payment>(db);
                }
                return PaymentRepo;
            }
        }

        public AdminRepository _AdminRepo
        {
            get
            {
                if (AdminRepo == null)
                {
                    AdminRepo = new AdminRepository(db);
                }
                return AdminRepo;
            }
        }
        public ServiceRequestPaymentRepository _ServiceRequestPaymentRepo
        {
            get
            {
                if (serviceRequestPaymentRepo == null)
                {
                    serviceRequestPaymentRepo = new ServiceRequestPaymentRepository(db);
                }
                return serviceRequestPaymentRepo;
            }
        }

        public CustomerRepository _CustomerRepo
        {
            get
            {
                if (CustomerRepo == null)
                {
                    CustomerRepo = new CustomerRepository(db);
                }
                return CustomerRepo;
            }
        }


        public ArtisanRepository _ArtisanRepo
        {
            get
            {
                if (ArtisanRepo == null)
                {
                    ArtisanRepo = new ArtisanRepository(db);
                }
                return ArtisanRepo;
            }
        }


        public GenericRepository<Contract> _ContractRepo
        {
            get
            {
                if (ContractRepo == null)
                {
                    ContractRepo = new GenericRepository<Contract>(db);
                }
                return ContractRepo;
            }
        }

        public GenericRepository<Violation> _ViolationRepo
        {
            get
            {
                if (ViolationRepo == null)
                {
                    ViolationRepo = new GenericRepository<Violation>(db);
                }
                return ViolationRepo;
            }
        }

        public GenericRepository<Discount> _DiscountRepo
        {
            get
            {
                if (DiscountRepo == null)
                {
                    DiscountRepo = new GenericRepository<Discount>(db);
                }
                return DiscountRepo;
            }
        }

        public CustomerDiscountRepository _CustomerDiscountRepo
        {
            get
            {
                if (CustomerDiscountRepo == null)
                {
                    CustomerDiscountRepo = new CustomerDiscountRepository(db);
                }
                return CustomerDiscountRepo;
            }
        }


        public ReviewRepository _ReviewRepo
        {
            get
            {
                if (reviewRepo == null)
                {
                    reviewRepo = new ReviewRepository(db);
                }
                return reviewRepo;
            }
        }
        public GenericRepository<Notification> _NotificationRepo
        {
            get
            {
                if (notificationRepo == null)
                {
                    notificationRepo = new GenericRepository<Notification>(db);
                }
                return notificationRepo;
            }
        }
        public void save()
        {
            db.SaveChanges();
        }
    }
}
