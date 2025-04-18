using Admin_Dashboard.Models;
using Admin_Dashboard.Repository;
using Microsoft.AspNetCore.Identity;

namespace Admin_Dashboard.UnitOfWorks
{
    public class UnitOFWork
    {
        SanayiiContext db;

        GenericRepository<IdentityRole> roleRepo;
        GenericRepository<UserPhone> userPhoneRepo;

        GenericRepository<Category> categoryRopo;
        GenericRepository<Service> serviceRopo;

        GenericRepository<Payment> paymentMethodsRopo;
        GenericRepository<Payment> paymentRepo;

        AdminRepository adminRepo;
        CustomerRepository customerRepo;
        ArtisanRepository artisanRepo;

        GenericRepository<Contract> contractRepo;
        GenericRepository<Violation> violationRepo;

        GenericRepository<ServiceRequestPayment> serviceRequestPaymentRepo;

        GenericRepository<Discount> discountRepo;
        GenericRepository<CustomerDiscount> customerDiscountRepo;

        GenericRepository<Review> reviewRepo;



        public UnitOFWork(SanayiiContext db)
        {
            this.db = db;
        }

        public GenericRepository<IdentityRole> _roleRepo
        {
            get
            {
                if (roleRepo == null)
                {
                    roleRepo = new GenericRepository<IdentityRole>(db);
                }
                return roleRepo;
            }
        }


        public GenericRepository<Category> _categoryRopo
        {
            get
            {
                if (categoryRopo == null)
                {
                    categoryRopo = new GenericRepository<Category>(db);
                }
                return categoryRopo;
            }
        }
        public GenericRepository<UserPhone> _userPhoneRepo
        {
            get
            {
                if (userPhoneRepo == null)
                {
                    userPhoneRepo = new GenericRepository<UserPhone>(db);
                }
                return userPhoneRepo;
            }
        }


        public GenericRepository<Service> _serviceRopo
        {
            get
            {
                if (serviceRopo == null)
                {
                    serviceRopo = new GenericRepository<Service>(db);
                }
                return serviceRopo;
            }
        }

        public GenericRepository<Payment> _paymentMethodsRopo
        {
            get
            {
                if (paymentMethodsRopo == null)
                {
                    paymentMethodsRopo = new GenericRepository<Payment>(db);
                }
                return paymentMethodsRopo;
            }
        }

        public GenericRepository<Payment> _paymentRopo
        {
            get
            {
                if (paymentRepo == null)
                {
                    paymentRepo = new GenericRepository<Payment>(db);
                }
                return paymentRepo;
            }
        }

        public AdminRepository _adminRopo
        {
            get
            {
                if (adminRepo == null)
                {
                    adminRepo = new AdminRepository(db);
                }
                return adminRepo;
            }
        }


        public CustomerRepository _customerRepo
        {
            get
            {
                if (customerRepo == null)
                {
                    customerRepo = new CustomerRepository(db);
                }
                return customerRepo;
            }
        }


        public ArtisanRepository _artisanRepo
        {
            get
            {
                if (artisanRepo == null)
                {
                    artisanRepo = new ArtisanRepository(db);
                }
                return artisanRepo;
            }
        }


        public GenericRepository<Contract> _contractRepo
        {
            get
            {
                if (contractRepo == null)
                {
                    contractRepo = new GenericRepository<Contract>(db);
                }
                return contractRepo;
            }
        }

        public GenericRepository<Violation> _violationRepo
        {
            get
            {
                if (violationRepo == null)
                {
                    violationRepo = new GenericRepository<Violation>(db);
                }
                return violationRepo;
            }
        }

        public GenericRepository<ServiceRequestPayment> _serviceRequestPaymentRepo
        {
            get
            {
                if (serviceRequestPaymentRepo == null)
                {
                    serviceRequestPaymentRepo = new GenericRepository<ServiceRequestPayment>(db);
                }
                return serviceRequestPaymentRepo;
            }
        }

        public GenericRepository<Discount> _discountRepo
        {
            get
            {
                if (discountRepo == null)
                {
                    discountRepo = new GenericRepository<Discount>(db);
                }
                return discountRepo;
            }
        }

        public GenericRepository<CustomerDiscount> _customerDiscountRepo
        {
            get
            {
                if (customerDiscountRepo == null)
                {
                    customerDiscountRepo = new GenericRepository<CustomerDiscount>(db);
                }
                return customerDiscountRepo;
            }
        }


        public GenericRepository<Review> _reviewRepo
        {
            get
            {
                if (reviewRepo == null)
                {
                    reviewRepo = new GenericRepository<Review>(db);
                }
                return reviewRepo;
            }
        }

        public void save()
        {
            db.SaveChanges();
        }
    }
}
