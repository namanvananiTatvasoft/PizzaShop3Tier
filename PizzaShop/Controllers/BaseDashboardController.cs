using BAL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace PizzaShop.Controllers
{
    [Authorize(Roles = "Super Admin")]// Apply Authorize at the controller level
    public abstract class BaseDashboardController : Controller
    {
        private string _userName;
        private string _role;
        private string _imgurl;

        private readonly IJwtservices _jwtservices;
        private readonly IAuthServices _auth;

        public BaseDashboardController(IJwtservices jwtservices, IAuthServices auth){
            _jwtservices = jwtservices;
            _auth = auth;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            // _userName = Request.Cookies["UserName"] ?? "Username";
            var token = Request.Cookies["AuthToken"].ToString();
            _userName = _jwtservices.getEmailDetailsFromToken(token);
            _role = _jwtservices.getRoleDetailsFromToken(token);
            _imgurl = _auth.getImageUrl(_userName);

        }

        protected string GetUserName()
        {
            return _userName;
        }

        protected string GetRole()
        {
            return _role;
        }

        protected string GetImgUrl()
        {
            return _imgurl;
        }
    }


}
