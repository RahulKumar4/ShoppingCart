using shoppingCart.Models.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace shoppingCart.Models.ViewModel.Pages
{
    public class PageVM
    {
        public PageVM()
        {

        }
        public PageVM(PageDTO row)
        {
            Id = row.Id;
            Title = row.Title;
            Slug = row.Slug;
            Body = row.Body;
            shorting = row.shorting;
            hasSideBar = row.hasSideBar;
        }
        public int Id { get; set; }
        [Required]
        [StringLength (50,MinimumLength =3)]
        public string Title { get; set; }
        public string Slug { get; set; }
        [Required]
        [StringLength(int.MaxValue, MinimumLength = 3)]
        public string Body { get; set; }
        public int shorting { get; set; }
        public bool hasSideBar { get; set; }
    }
}