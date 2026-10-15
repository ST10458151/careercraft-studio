using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CareerCraft.Web.Controllers;
public sealed class RegisterForm
{
    [Required,EmailAddress] public string Email { get; set; } = "";
    [Required,StringLength(100)] public string FullName { get; set; } = "";
    [Required,DataType(DataType.Password)] public string Password { get; set; } = "";
    [Required,Compare(nameof(Password)),DataType(DataType.Password)] public string ConfirmPassword { get; set; } = "";
}
public sealed class LoginForm
{
    [Required,EmailAddress] public string Email { get; set; } = "";
    [Required,DataType(DataType.Password)] public string Password { get; set; } = "";
}
public class AccountController(UserManager<IdentityUser> users,SignInManager<IdentityUser> signIn) : Controller
{
    [HttpGet] public IActionResult Register() => View(new RegisterForm());
    [HttpPost,ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterForm form)
    {
        if(!ModelState.IsValid)return View(form);
        var user=new IdentityUser {UserName=form.Email.Trim(),Email=form.Email.Trim()};
        var outcome=await users.CreateAsync(user,form.Password);
        if(!outcome.Succeeded){foreach(var e in outcome.Errors)ModelState.AddModelError("",e.Description);return View(form);}
        await signIn.SignInAsync(user,isPersistent:false);
        return RedirectToAction("Create","Orders");
    }
    [HttpGet] public IActionResult Login(string? returnUrl=null)
    {
        ViewData["ReturnUrl"]=Url.IsLocalUrl(returnUrl)?returnUrl:null;return View(new LoginForm());
    }
    [HttpPost,ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginForm form,string? returnUrl=null)
    {
        if(!ModelState.IsValid)return View(form);
        var result=await signIn.PasswordSignInAsync(form.Email.Trim(),form.Password,isPersistent:false,lockoutOnFailure:true);
        if(result.Succeeded)
        {
            if (Url.IsLocalUrl(returnUrl)) return LocalRedirect(returnUrl!);
            var user = await users.FindByEmailAsync(form.Email.Trim());
            return user != null && await users.IsInRoleAsync(user,"Admin") ? RedirectToAction("Index","Admin") : RedirectToAction("Index","Orders");
        }
        ModelState.AddModelError("","Invalid credentials or temporarily locked account.");return View(form);
    }
    [Authorize,HttpPost,ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await signIn.SignOutAsync();return RedirectToAction("Index","Home");
    }
    [HttpGet]public IActionResult Denied()=>View();
}
