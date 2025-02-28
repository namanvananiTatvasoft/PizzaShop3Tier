using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
// using Microsoft.AspNetCore.Mvc.Rendering;
using DAL.Models;
using Microsoft.AspNetCore.Http;

namespace DAL.ViewModel;


public class UpdateUserDetails
{
    [Required]
    public string Email { get; set; } = null!;
    [Required]
    public string Username { get; set; } = null!;
    [Required]
    public string? Phone { get; set; } = null!;
    [Required]
    public string Firstname { get; set; } = null!;
    [Required]
    public string? Lastname { get; set; } = null!;
    [Required]
    public string? Address1 { get; set; } = null!;
    [Required]
    public int? Zipcode { get; set; } = null!;

    public IFormFile profileImg { get; set; }

    public string photoUrl { get; set; }

    // These properties should be added to bind country, state, and city IDs
    public int CountryId { get; set; }
    public int StateId { get; set; }
    public int CityId { get; set; }

    // Add these properties to populate the dropdowns
    public List<Country> CountryList { get; set; }
    public List<State> StateList { get; set; }
    public List<City> CityList { get; set; }
}
