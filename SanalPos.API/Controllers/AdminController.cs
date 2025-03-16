using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SanalPos.Application.Common.Interfaces;
using SanalPos.Application.Interfaces;
using SanalPos.Domain.Entities;
using SanalPos.Domain.Enums;

namespace SanalPos.API.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPaymentTransactionRepository _paymentTransactionRepository;

        public AdminController(IUnitOfWork unitOfWork, IPaymentTransactionRepository paymentTransactionRepository)
        {
            _unitOfWork = unitOfWork;
            _paymentTransactionRepository = paymentTransactionRepository;
        }

        [HttpGet("dashboard/summary")]
        public async Task<ActionResult<DashboardSummary>> GetDashboardSummary()
        {
            var userCount = await _unitOfWork.Users.CountAsync();
            var productCount = await _unitOfWork.Products.CountAsync();
            var orderCount = await _unitOfWork.Orders.CountAsync();
            var paymentCount = await _paymentTransactionRepository.CountAsync(DateTime.Now.AddMonths(-1), DateTime.Now);
            
            var totalSales = await _unitOfWork.Orders.CountAsync(o => o.Status == OrderStatus.Delivered);
            var pendingOrders = await _unitOfWork.Orders.CountAsync(o => o.Status == OrderStatus.Pending);
            
            return Ok(new DashboardSummary
            {
                UserCount = userCount,
                ProductCount = productCount,
                OrderCount = orderCount,
                PaymentCount = paymentCount,
                TotalSales = totalSales,
                PendingOrders = pendingOrders
            });
        }
        
        [HttpGet("users")]
        public async Task<ActionResult<IEnumerable<User>>> GetAllUsers()
        {
            var users = await _unitOfWork.Users.GetAllAsync();
            return Ok(users);
        }
        
        [HttpGet("orders")]
        public async Task<ActionResult<IEnumerable<Order>>> GetAllOrders()
        {
            var orders = await _unitOfWork.Orders.GetAllAsync();
            return Ok(orders);
        }
        
        [HttpGet("payments")]
        public async Task<ActionResult<IEnumerable<PaymentTransaction>>> GetAllPayments()
        {
            var startDate = DateTime.Now.AddMonths(-1);
            var endDate = DateTime.Now;
            var payments = await _paymentTransactionRepository.GetSuccessfulTransactionsAsync(startDate, endDate);
            return Ok(payments);
        }
        
        [HttpGet("products")]
        public async Task<ActionResult<IEnumerable<Product>>> GetAllProducts()
        {
            var products = await _unitOfWork.Products.GetAllAsync();
            return Ok(products);
        }
    }
    
    public class DashboardSummary
    {
        public int UserCount { get; set; }
        public int ProductCount { get; set; }
        public int OrderCount { get; set; }
        public int PaymentCount { get; set; }
        public int TotalSales { get; set; }
        public int PendingOrders { get; set; }
    }
} 