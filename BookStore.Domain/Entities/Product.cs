﻿using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace BookStore.Domain.Entities {
    public class Product {
        [HiddenInput(DisplayValue = false)]
        public int ProductID { get; set; }

        [Required(ErrorMessage = "Proszę podać nazwę produktu.")]
        [Display(Name = "Nazwa")]
        public string Name { get; set; }

        [DataType(DataType.MultilineText), Display(Name = "Opis")]
        [Required(ErrorMessage = "Proszę podać opis.")]
        public string Description { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Proszę podać doatnią cenę.")]
        [Display(Name = "Cena")]
        public decimal Price { get; set; }

        [Display(Name = "Kategoria")]
        [Required(ErrorMessage = "Proszę określić kategorię.")]
        public string Category { get; set; }
    }
}
