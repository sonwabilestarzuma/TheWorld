﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheWorld.Models;
using TheWorld.Services;
using TheWorld.ViewModel;

namespace TheWorld.Controllers.Web
{
    public class AppController : Controller
    {
        private IMailServices _mailService;
        private IConfigurationRoot _config;
        private IWorldRepository _repository;
        private ILogger<AppController> _logger;

        public AppController(IMailServices mailService, 
            IConfigurationRoot config, 
            IWorldRepository repository,
            ILogger<AppController> logger)
        {
            _mailService = mailService;
            _config = config;
            _repository = repository;
            _logger = logger;
           
        }
        public IActionResult Index()
        {

            return View();
        }
       [Authorize]
        public IActionResult Trips()
        {
            try
            {
                var trips = _repository.GetAllTrips();
                return View(trips);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to get trips in Index page:{ex.Message}");
                return Redirect("/error");
            }
        }
        public IActionResult Contact()   
        {
           
            return View();
        }
        [HttpPost]
        public IActionResult Contact(ContactViewModel model)
        {
            // hard coded 
            if (model.Email.Contains("aol.com"))
            {
                ModelState.AddModelError("Email", "We don't support AOL addresses");
            }

            if (ModelState.IsValid)
            {
                _mailService.SendMail(_config["MailSettings:ToAddress"], model.Email, "From TheWorld", model.Message);
                ModelState.Clear();
                ViewBag.UserMessage = "Message sent "
;            }
           
            return View();
        }
        public IActionResult About()
        {
            return View();
        }
    }
}
