using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using BEYOND.Core.Models;
using BEYOND.Core.ViewModels;
using BEYOND.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;


namespace BEYOND.one.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BEYONDController : ControllerBase
    {
        protected IUnitOfWork _unitOfWork;
        private HttpClient client;
        public BEYONDController()
        {
            client = new HttpClient();
            this._unitOfWork = new UnitOfWork();
        }

        [HttpGet("ISO8601Format")]
        public string ISO8601Format()
        {
            return DateTime.Now.ToUniversalTime().ToString("s") + "Z";
        }

        [HttpGet("GetPosts")]
        public List<Article> GetPosts()
        {
            string searchValue = "minima";
            string url = "https://jsonplaceholder.typicode.com/posts";
            HttpRequestMessage m = new HttpRequestMessage(HttpMethod.Get, url);
            HttpResponseMessage resp = client.SendAsync(m).Result;
            client.Dispose();
            
            var response = JsonSerializer.Deserialize<List<Article>>(resp.Content.ReadAsStringAsync().Result);

            return response.Where(a => a.body.Contains(searchValue)).ToList();
        }

        [HttpGet("GetProducts")]
        public IActionResult GetProducts()
        {
            var productList = _unitOfWork.ProductRepository.GetProductList();
            if (productList.Count == 0)
            {
                return Ok("There is no product");
            }
            return Ok(productList);
        }

        [Route("AddProduct")]
        [HttpPost("AddProduct")]
        public IActionResult AddProduct(int productID, int stockAvailable)
        {
            try
            {
                Product product = new Product();
                product.ProductID = productID;
                product.StockAvailable = stockAvailable;
                product.CreatedOnDate = DateTime.Now;
                _unitOfWork.ProductRepository.Add(product);
                _unitOfWork.Save();

                return Ok();
            }
            catch (Exception ex)
            {
                return Ok(ex.Message.ToString());
            }
        }
    }
}
