using MusicStore.Models.Admin.Customer;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MusicStore.Models.Admin.Customer
{
    public class CustomerListViewModel
    {
        [Display(Name = "First Name")]
        public string FirstNameFilter { get; set; }

        [Display(Name = "Last Name")]
        public string LastNameFilter { get; set; }

        public List<CustomerViewModel> Customers { get; set; } = new List<CustomerViewModel>();

        public int TotalCustomers => Customers.Count;

        public bool HasFilters => !string.IsNullOrEmpty(FirstNameFilter) || !string.IsNullOrEmpty(LastNameFilter);
    }
}