using ESourcing.Products.Entities;
using ESourcing.Products.Repositories;
using ESourcing.Products.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ESourcing.Products.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ProductController : ControllerBase
	{

		#region Variables
		private readonly IProductRepository productRepository;
		private readonly ILogger<ProductController> _logger;


		#endregion

		#region Constructur
		public ProductController(IProductRepository productRepository, ILogger<ProductController> logger)
		{
			this.productRepository = productRepository;
			_logger = logger;
		}
		#endregion

		#region Methods
		[HttpGet]
		[ProducesResponseType(typeof(Product), (int)HttpStatusCode.OK)]
		public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
		{
			var products = await productRepository.GetProducts();
			return Ok(products);
		}

		[HttpGet("{id:length(24)}", Name = "GetProduct")]
		[ProducesResponseType((int)HttpStatusCode.NotFound)]
		[ProducesResponseType(typeof(Product), (int)HttpStatusCode.OK)]
		public async Task<ActionResult<Product>> GetProduct(string id)
		{
			var product = await productRepository.GetProduct(id);
			if (product == null)
			{
				_logger.LogError($"Product with id : {id},hasn't been found in databasei");
				return NotFound();
			}
			return Ok(product);
		}

		[HttpPost]
		[ProducesResponseType(typeof(Product), (int)HttpStatusCode.Created)]
		public async Task<ActionResult<Product>> CreateProduct([FromBody] Product product)
		{
			await productRepository.Create(product);
			return CreatedAtRoute("GetProduct", new { id = product.Id }, product);
		}

		[HttpPut]
		[ProducesResponseType(typeof(Product), (int)HttpStatusCode.OK)]
		public async Task<IActionResult> UpdateProduct([FromBody] Product product)
		{
			return Ok(await productRepository.Update(product));
		}


		[HttpDelete("{id:length(24)}")]
		[ProducesResponseType(typeof(Product), (int)HttpStatusCode.OK)]
		public async Task<IActionResult> DeleteProductById(string id)
		{
			return Ok(await productRepository.Delete(id));
		}
		#endregion
	}
}
