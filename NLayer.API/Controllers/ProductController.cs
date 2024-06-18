using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NLayer.API.Filters;
using NLayer.Core.DTOs;
using NLayer.Core.Models;
using NLayer.Core.Services;

namespace NLayer.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : CustomBaseController
    {
        private readonly IMapper _mapper;
        private readonly IProductService _service;

        public ProductController(IMapper mapper, IProductService productService)
        {
            _mapper = mapper;
            _service = productService;
        }

        // api/products/GetProductsWithCategory => ENDPOINT
        [HttpGet("GetProductsWithCategory")] // Bunun yerine HttpGet("[action]") direkt olarak actionun ismini alır
        public async Task<IActionResult> GetProductsWithCategory() 
        { 
            return CreateActionResult(await _service.GetProductsWithCategory());
        }

        [HttpGet]
        public async Task<IActionResult> All()
        {
            var datas = await _service.GetAllAsync();

            var datasDto = _mapper.Map<List<ProductDto>>(datas.ToList());
            return CreateActionResult(CustomResponseDto<List<ProductDto>>.Success(200, datasDto));
        }

        [ServiceFilter(typeof(NotFoundFilter<Product>))]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var data = await _service.GetByIdAsync(id);

            var dataDto = _mapper.Map<ProductDto>(data);
            return CreateActionResult(CustomResponseDto<ProductDto>.Success(200, dataDto));
        }

        [HttpPost()]
        public async Task<IActionResult> Save(ProductDto product)
        {
            var data = await _service.AddAsync(_mapper.Map<Product>(product));

            var dataDto = _mapper.Map<ProductDto>(data);
            return CreateActionResult(CustomResponseDto<ProductDto>.Success(201, dataDto));
        }

        [HttpPut()]
        public async Task<IActionResult> Update(ProductUpdateDto product)
        {
            await _service.Update(_mapper.Map<Product>(product));

            return CreateActionResult(CustomResponseDto<NoContentDto>.Success(204));
        }

        [ServiceFilter(typeof(NotFoundFilter<Product>))]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Remove(int id)
        {
            var product = await _service.GetByIdAsync(id);
            await _service.Remove(product);
            return CreateActionResult(CustomResponseDto<NoContentDto>.Success(204));
        }
    }
}
