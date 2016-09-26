using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorPages.Samples.Web.Data;

namespace RazorPages.Samples.Web.Pages
{
    public class EditCustomer : PageModel
    {
        public EditCustomer(AppDbContext db)
        {
            Db = db;
        }

        public AppDbContext Db { get; }

        public Customer Customer { get; set; }
        
        [TempData]
        public string SuccessMessage
        {
            get { return (string)PageContext.TempData[nameof(SuccessMessage)]; }
            set { PageContext.TempData[nameof(SuccessMessage)] = value; }
        }

        public bool ShowSuccessMessage => !string.IsNullOrEmpty(SuccessMessage);

        [TempData]
        public string ErrorMessage
        {
            get { return (string)PageContext.TempData[nameof(ErrorMessage)]; }
            set { PageContext.TempData[nameof(ErrorMessage)] = value; }
        }

        public bool ShowErrorMessage => !string.IsNullOrEmpty(ErrorMessage);

        public IActionResult OnGet(int id)
        {
            Customer = GetCustomer(id);
            return View();
        }

        public IActionResult OnPost(int id, Customer customer)
        {
            if (ModelState.IsValid)
            {
                UpdateCustomer(id, customer);
                SuccessMessage = $"Customer {id} successfully edited!";
                return Redirect("/");
            }

            // Model errors, populate customer list and show errors
            Customer = GetCustomer(customer.Id);
            ErrorMessage = "Please correct the errors and try again";
            return View();
        }

        public Customer GetCustomer(int id)
        {
            var customer = Db.Customers.Where(c => c.Id == id).SingleOrDefault();

            return customer;
        }

        public void UpdateCustomer(int id, Customer customer)
        {
            var currentCustomer = Db.Customers.Single(c => c.Id == id);
            
            currentCustomer.Name = customer.Name;
            Db.SaveChanges();
        }
    }
}
