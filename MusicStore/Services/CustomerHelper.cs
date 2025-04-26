using MusicStore.Model.Entities;
using MusicStore.Models.Admin.Customer;


namespace MusicStore.Services
{
    public static class CustomerHelper
    {
        /// <summary>
        /// Converts a Customer entity to a CustomerViewModel.
        /// </summary>
        /// <param name="customer">The Customer entity to convert.</param>
        /// <returns>A CustomerViewModel populated with the properties of the Customer entity.</returns>
        public static CustomerViewModel ConvertFromCustomerToCustomerViewModel(Customer customer)
        {
            return new CustomerViewModel
            {
                CustomerId = customer.Id,
                UserId = customer.User.Id,
                FirstName = customer.firstName,
                LastName = customer.lastName,
                Address = customer.Address,
                Phone = customer.Phone,
                CreatedAt = customer.createdAt

            };
        }
    }
}