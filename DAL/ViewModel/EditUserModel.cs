using System.ComponentModel.DataAnnotations;
using DAL.Models;
using Microsoft.AspNetCore.Http;

namespace DAL.ViewModel;

public class EditUserModel
{
    [Required(ErrorMessage = "Firstname is required")]
    public string Firstname { get; set; }

    [Required(ErrorMessage = "Lastname is required")]
    public string Lastname { get; set; }

    [Required(ErrorMessage = "Username is required")]
    public string Username { get; set; }
    
    [Required(ErrorMessage = "Email is required")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Password is required")]
    public string Password { get; set; }

    [Required(ErrorMessage = "Country is required")]
    public int Countryid { get; set; }

    [Required(ErrorMessage = "State is required")]
    public int Stateid { get; set; }

    [Required(ErrorMessage = "City is required")]
    public int Cityid { get; set; }

    [Required(ErrorMessage = "Zipcode is required")]
    [RegularExpression(@"^([0-9]{6})$", ErrorMessage = "Invalid Zipcode.")]
    public int Zipcode { get; set; }

    [Required(ErrorMessage = "Address is required")]
    public string Address1 { get; set; }

    [Required(ErrorMessage = "Phone is required")]
    [RegularExpression(@"^([0-9]{10})$", ErrorMessage = "Invalid Phone Number.")]
    public String Phone { get; set; }
    public bool Status { get; set; }

    public int Roleid { get; set; }
    public List<Country> CountryList { get; set; }
    public List<State> StateList { get; set; }
    public List<City> CityList { get; set; }

    public IFormFile imageFile { get; set; }
}

