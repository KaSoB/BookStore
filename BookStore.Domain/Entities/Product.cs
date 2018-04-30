﻿using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace BookStore.Domain.Entities {
    public class Product {
        [HiddenInput(DisplayValue = false)]
        public int ProductID { get; set; }
        [Display(Name = "Nazwa")]
        public string Name { get; set; }
        [DataType(DataType.MultilineText), Display(Name = "Opis")]
        public string Description { get; set; }
        [Display(Name = "Cena")]
        public decimal Price { get; set; }
        [Display(Name = "Kategoria")]
        public string Category { get; set; }
    }
}
