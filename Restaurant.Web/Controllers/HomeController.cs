﻿using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Restaurant.Web.Models;
using Restaurant.Web.Services.IServices;
using System.Diagnostics;

namespace Restaurant.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductService _productService;
        private readonly ICartService _cartService;
        public HomeController(ILogger<HomeController> logger, IProductService productService, ICartService cartService)
        {
            _logger = logger;
            _productService = productService;
            _cartService = cartService;
        }

        public async Task<IActionResult> Index()
        {


            List<ProductDto> list = new();
            var response = await _productService.GetAllProductsAsyn<ResponseDto>("");
            if (response != null && response.isSuccess)
            {
                list = JsonConvert.DeserializeObject<List<ProductDto>>(Convert.ToString(response.Result));
            }
            return View(list);
        }

        [Authorize]
        public async Task<IActionResult> Details(int productId)
        {
            ProductDto model = new();
            var response = await _productService.GetProductByIdAsync<ResponseDto>(productId, "");
            if (response != null && response.isSuccess)
            {
                model = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(response.Result));
            }
            return View(model);
        }

        [HttpPost]
        [ActionName("Details")]
        [Authorize]
        public async Task<IActionResult> DetailsPost(ProductDto productDto)
        {
            CartDto cartDto = new()
            {
                CartHeader = new CartHeaderDto
                {
                    UserId = User.Claims.Where(u => u.Type == "sub")?.FirstOrDefault()?.Value
                }
            };

            CartDetailsDto cartDetails = new CartDetailsDto()
            {
                Count = productDto.Count,
                ProductId = productDto.ProductId
            };

            var resp = await _productService.GetProductByIdAsync<ResponseDto>(productDto.ProductId, "");
            if (resp != null && resp.isSuccess)
            {
                cartDetails.Product = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(resp.Result));
            }
            List<CartDetailsDto> cartDetailsDtos = new();
            cartDetailsDtos.Add(cartDetails);
            cartDto.CartDetails = cartDetailsDtos;

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var addToCartResp = await _cartService.AddToCartAsync<ResponseDto>(cartDto, accessToken);
            if (addToCartResp != null && addToCartResp.isSuccess)
            {
                return RedirectToAction(nameof(Index));
            }

            return View(productDto);
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
        [Authorize]
        public async Task<IActionResult> Login()
        {

            return RedirectToAction(nameof(Index));
        }
        public IActionResult Logout()
        {
            return SignOut("Cookies", "oidc");
        }
    }
}