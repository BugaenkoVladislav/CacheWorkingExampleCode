using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using SimpleLocalCacher.Entities;

namespace SimpleLocalCacher.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController(IMemoryCache cache, ILogger<TestController> logger) : ControllerBase
    {
        private ILogger<TestController> _logger = logger;
        private Stopwatch sw = new Stopwatch();

        #region TableData

        private List<Template> _dataTemplate = GeneratorNotes(1000);

        private static List<Template> GeneratorNotes(int count)
        {
            var rand = new Random();
            var products = new List<Template>();
            string[] names = {
            "Apple iPhoneSE", "Samsung GalaxyS", "Huawei Mate", "Xiaomi Mi", "OnePlus Nord",
            "Google Pixel", "Apple iPhoneSE Max", "Samsung GalaxyS Plus", "Huawei Mate Pro",
            "Xiaomi Mi Pro", "OnePlus Nord CE", "Google Pixel XL", "Apple iPhoneSE Mini",
            "Samsung GalaxyS Ultra", "Huawei Mate RS", "Xiaomi Mi Max", "OnePlus Nord N10",
            "Google Pixel 4a", "Apple iPhoneSE Mini", "Samsung GalaxyS FE", "Huawei Mate X",
            "OnePlus Nord N200", "Google Pixel 5", "Apple iPhoneSE Pro", "Samsung GalaxyZ Fold",
            "Huawei P50 Pro", "Xiaomi Poco X4", "OnePlus 10 Pro", "Google Pixel 6", "Apple iPhone 14",
            "Samsung GalaxyS 21 FE", "Huawei Mate 40", "Xiaomi Redmi Note 11", "OnePlus 9 RT",
            "Google Pixel 7", "Apple iPhone 13", "Samsung Galaxy Note 20", "Huawei P40 Pro",
            "Xiaomi Mi 11", "OnePlus 8T", "Google Pixel 6 Pro", "Apple iPhone 12", "Samsung Galaxy A52",
            "Huawei Mate X2", "Xiaomi Mi Mix 4", "OnePlus 7 Pro", "Google Pixel 5a", "Apple iPhone SE 2022",
            "Samsung Galaxy Z Flip", "Huawei P30 Pro", "Xiaomi Redmi Note 10", "OnePlus 9",
            "Google Pixel 4", "Apple iPhone 11 Pro", "Samsung Galaxy S21 Ultra", "Huawei Mate 30 Pro",
            "Xiaomi Mi 10", "OnePlus 8 Pro", "Google Pixel 6a", "Apple iPhone 13 Mini", "Samsung Galaxy Z Fold 2",
            "Huawei P40 Pro", "Xiaomi Mi Mix Alpha", "OnePlus 7T", "Google Pixel 4 XL", "Apple iPhone 12 Pro Max",
            "Samsung Galaxy S21", "Huawei Mate X", "Xiaomi Redmi K40", "OnePlus 6T", "Google Pixel 5a 5G",
            "Apple iPhone 13 Pro Max", "Samsung Galaxy Z Flip 3", "Huawei Mate 40 Pro", "Xiaomi Mi 10 Pro",
            "OnePlus 9R", "Google Pixel 6 XL", "Apple iPhone 14 Pro", "Samsung Galaxy S20 FE", "Huawei P40",
            "Xiaomi Poco F3", "OnePlus Nord CE 2", "Google Pixel 5 XL", "Apple iPhone 13 Pro Max", 
            "Samsung Galaxy Z Fold 3", "Huawei P50", "Xiaomi Mi Mix 5", "OnePlus 9T", "Google Pixel 7 Pro",
            "Apple iPhone SE 2024", "Samsung Galaxy Z Flip 4", "Huawei P50 Pro Lite", "Xiaomi Mi 11 Ultra",
            "OnePlus 10R", "Google Pixel 7a", "Apple iPhone 14 Pro+", "Samsung Galaxy S23", "Huawei P60 Pro",
            "Xiaomi Mi 12", "OnePlus 11", "Google Pixel 8", "Apple iPhone 15", "Samsung Galaxy Z Fold 5",
            "Huawei P60", "Xiaomi Mi 12 Pro", "OnePlus 11 Pro", "Google Pixel 8 Pro", "Apple iPhone 16",
            "Samsung Galaxy Z Flip 6", "Huawei P80 Pro", "Xiaomi Redmi Note 12", "OnePlus 12", "Google Pixel 8a",
            "Apple iPhone 16 Pro", "Samsung Galaxy S24", "Huawei P80", "Xiaomi Mi 13", "OnePlus 12 Pro",
            "Google Pixel 9", "Apple iPhone 17", "Samsung Galaxy S24 Ultra", "Huawei P80 Pro", "Xiaomi Mi 13 Pro",
            "OnePlus 12T", "Google Pixel 9 Pro"
            };

            for (int i = 1; i <= count; i++)
            {
                var name = names[rand.Next(names.Length)];
                var price = Math.Round(rand.NextDouble() * 5000, 2); // Price between 0 and 5000

                products.Add(new Template()
                {
                    Id = i,
                    Name = name,
                    Price = price
                });
            }
            return products;
        }
        #endregion
        
        private string cacheKey = "myKey";
        private IMemoryCache _cache = cache;
        
        //at the second start speed will more rapid than first because data existing in cache and data comes from there 
        [HttpGet("GetFullTable")]
        public async Task<List<Template>?> GetFullTable()
        {
            sw.Start();
            //tryin to find by key data from cache, if data does not exist, it sets into 
            var result =  await _cache.GetOrCreateAsync(cacheKey, cacheEntry =>
            {
                cacheEntry.SetAbsoluteExpiration(TimeSpan.FromSeconds(3600)).SetSlidingExpiration(TimeSpan.FromSeconds(45)).SetPriority(CacheItemPriority.Normal);
                return Task.FromResult(_dataTemplate);
            });
            sw.Stop();
            logger.LogInformation("Time execution: " + sw.Elapsed.ToString());
            return result;
        }

        [HttpPost("ClearCache")]
        public IActionResult ClearCache()
        {
            _cache.Remove(cacheKey);
            return Ok();
        }
    }
}
