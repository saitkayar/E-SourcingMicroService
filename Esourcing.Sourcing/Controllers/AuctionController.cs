﻿using Esourcing.Sourcing.Entities;
using Esourcing.Sourcing.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Esourcing.Sourcing.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuctionController : ControllerBase
    {
        private readonly IAuctionRepository _auctionRepository;
        private readonly IBidRepository _bidRepository;
        private readonly ILogger<AuctionController> _logger;

        public AuctionController(
            IAuctionRepository auctionRepository,
            IBidRepository bidRepository,
            
            ILogger<AuctionController> logger)
        {
            _auctionRepository = auctionRepository;
            _bidRepository = bidRepository;
           
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Auction>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<Auction>>> GetAuctions()
        {
            var auctions = await _auctionRepository.GetAuctions();

            return Ok(auctions);
        }

        [HttpGet("{id:length(24)}", Name = "GetAuction")]
        [ProducesResponseType(typeof(Auction), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<Auction>> GetAuction(string id)
        {
            var auction = await _auctionRepository.GetAuction(id);

            if (auction == null)
            {
                _logger.LogError($"Auction with id: {id}, hasn't been found in database.");
                return NotFound();
            }

            return Ok(auction);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Auction), (int)HttpStatusCode.Created)]
        public async Task<ActionResult<Auction>> CreateAuction([FromBody] Auction auction)
        {
            await _auctionRepository.Create(auction);

            return CreatedAtRoute("GetAuction", new { id = auction.Id }, auction);
        }

        [HttpPut]
        [ProducesResponseType(typeof(Auction), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<Auction>> UpdateAuction([FromBody] Auction auction)
        {
            return Ok(await _auctionRepository.Update(auction));
        }

        [HttpDelete("{id:length(24)}")]
        [ProducesResponseType(typeof(Auction), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<Auction>> DeleteAuctionById(string id)
        {
            return Ok(await _auctionRepository.Delete(id));
        }

        [HttpPost("CompleteAuction")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Accepted)]
        public async Task<ActionResult> CompleteAuction([FromBody] string id)
        {
            Auction auction = await _auctionRepository.GetAuction(id);
            if (auction == null)
                return NotFound();

            if (auction.Status != (int)Status.Active)
            {
                _logger.LogError("Auction can not be completed");
                return BadRequest();
            }

            Bid bid = await _bidRepository.GetWinnerBid(id);
            if (bid == null)
                return NotFound();

           
          

            auction.Status = (int)Status.Closed;
            bool updateResponse = await _auctionRepository.Update(auction);
            if (!updateResponse)
            {
                _logger.LogError("Auction can not updated");
                return BadRequest();
            }

            try
            {
                
            }
            catch (Exception ex)
            {
               // _logger.LogError(ex, "ERROR Publishing integration event: {EventId} from {AppName}", eventMessage.Id, "Sourcing");
                throw;
            }

            return Accepted();
        }

    }
}
