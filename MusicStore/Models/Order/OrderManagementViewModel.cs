using System;
using System.Collections.Generic;
using System.Linq;
using MusicStore.Model.Entities;
using MusicStore.Models.Order;

namespace MusicStore.Models.Admin.Order
{
    public class OrderManagementViewModel
    {
        // Collection of orders for the admin table
        public List<OrderViewModel> Orders { get; set; } = new List<OrderViewModel>();

        // Pagination properties
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }
        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;

        // Filter properties
        public string SearchString { get; set; }
        public string Status { get; set; }
        public List<string> StatusOptions { get; set; } = new List<string>();

        // Sorting properties
        public string SortOrder { get; set; } = "date_desc";

        // Helper properties for sort links in the view
        public string DateSortParam => SortOrder == "date_asc" ? "date_desc" : "date_asc";
        public string TotalSortParam => SortOrder == "total_asc" ? "total_desc" : "total_asc";
        public string CustomerSortParam => SortOrder == "customer" ? "date_desc" : "customer";
        public string StatusSortParam => SortOrder == "status" ? "date_desc" : "status";

        
        public string GetSortIconClass(string column)
        {
            if (!SortOrder.StartsWith(column))
                return "fa fa-sort";

            return SortOrder.EndsWith("_asc") ? "fa fa-sort-up" : "fa fa-sort-down";
        }

        // Summary of sttatistcs
        public decimal TotalSales => Orders.Sum(o => o.TotalAmount);
        public int TotalOrders => Orders.Count;
    }
}