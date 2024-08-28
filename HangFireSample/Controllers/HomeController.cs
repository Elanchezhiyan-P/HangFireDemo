using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using HangFireSample.Models;
using HangFireSample.Service;
using Hangfire;

namespace HangFireSample.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly EmailService _emailService;

    public HomeController(ILogger<HomeController> logger, EmailService emailService)
    {
        _logger = logger;
        _emailService = emailService;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    [HttpGet]
    public IActionResult ScheduleEmail()
    {
        return View();
    }

    [HttpPost]
    public IActionResult ScheduleEmail(EmailModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        if (model.SendDateTime < DateTime.Now)
        {
            ModelState.AddModelError("SendDateTime", "Scheduled time must be in the future.");
            return View(model);
        }

        BackgroundJob.Schedule(() => _emailService.SendEmail(model.RecipientEmail, model.Subject, model.Body), model.SendDateTime);

        ViewBag.Message = "Email has been scheduled successfully!";
        return View(); 
    }
}
