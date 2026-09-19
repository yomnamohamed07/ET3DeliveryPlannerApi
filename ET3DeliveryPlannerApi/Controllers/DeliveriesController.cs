using ET3DeliveryPlanner.Data.Entities;
using ET3DeliveryPlanner.Data.Repositories;
using ET3DeliveryPlanner.Services.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace ET3DeliveryPlannerApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeliveriesController : ControllerBase
    {
        private readonly IDeliveryRepository _deliveryRepository;

        public DeliveriesController(
            IDeliveryRepository deliveryRepository)
        {
            _deliveryRepository = deliveryRepository;
        }

        [HttpGet]
        public async Task<ActionResult<List<Delivery>>> GetAll()
        {
            var deliveries =
                await _deliveryRepository.GetAllAsync();

            return Ok(deliveries);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Delivery>> GetById(int id)
        {
            var delivery =
                await _deliveryRepository.GetByIdAsync(id);

            if (delivery == null)
            {
                throw new NotFoundException(
                    $"Delivery with ID {id} was not found.");
            }

            return Ok(delivery);
        }

        [HttpPost]
        public async Task<ActionResult<Delivery>> Add(
            [FromBody] Delivery delivery)
        {
            if (delivery.PackageWeight <= 0)
            {
                throw new BadRequestException(
                    "Package weight must be greater than 0.");
            }

            if (delivery.PackageWeight > 10)
            {
                throw new BadRequestException(
                    "Package weight cannot exceed 10 kg.");
            }

         

            await _deliveryRepository.AddAsync(delivery);

            return CreatedAtAction(
                nameof(GetById),
                new { id = delivery.Id },
                delivery);
        }
    }
}