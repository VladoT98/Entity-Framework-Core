﻿using FastFood.Models;

namespace FastFood.Core.Controllers
{
    using System;
    using AutoMapper;
    using Data;
    using Microsoft.AspNetCore.Mvc;
    using ViewModels.Employees;

    public class EmployeesController : Controller
    {
        private readonly FastFoodContext context;
        private readonly IMapper mapper;

        public EmployeesController(FastFoodContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public IActionResult Register()
        {
            return this.View();
        }

        [HttpPost]
        public IActionResult Register(RegisterEmployeeInputModel model)
        {
            throw new NotImplementedException();
        }

        public IActionResult All()
        {
            throw new NotImplementedException();
        }
    }
}
