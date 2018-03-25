using shoppingCart.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace shoppingCart.Models.ViewModel.Shop
{
    public class CategoriesVM
    {
        public CategoriesVM()
        {

        }
        public CategoriesVM(CategoriesDTO row)
        {
            Id = row.Id;
            Name = row.Name;
            Slug = row.Slug;
            Description = row.Description;
            Sorting = row.Sorting;
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public string Description { get; set; }
        public int Sorting { get; set; }
    }
}