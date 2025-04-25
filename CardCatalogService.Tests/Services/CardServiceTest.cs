using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CardCatalogService.Application.DTOs;
using CardCatalogService.Application.Interfaces;
using CardCatalogService.Application.Services;
using Xunit;
using AutoMapper;
using CardCatalogService.Domain.Entities;

namespace CardCatalogService.Tests
{
    public class CardServiceTest
    {
        private readonly Mock<ICardRepository> _cardRepositoryMock;
        private readonly Mock<ICardCacheService> _cardCacheServiceMock;
        private readonly Mock<ICacheService> _cacheServiceMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly CardService _cardService;

        public CardServiceTest()
        {
            _cardRepositoryMock = new Mock<ICardRepository>();
            _cardCacheServiceMock = new Mock<ICardCacheService>();
            _cacheServiceMock = new Mock<ICacheService>();
            _mapperMock = new Mock<IMapper>();
            _cardService = new CardService(_cardRepositoryMock.Object, _mapperMock.Object, _cacheServiceMock.Object, _cardCacheServiceMock.Object);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsCardsFromCache_WhenCacheIsAvailable()
        {
            // Arrange
            var cachedCards = new List<CardDto> { new CardDto { Id = 1, Name = "Card 1" } };
            _cacheServiceMock.Setup(cs => cs.GetAsync<IEnumerable<CardDto>>("all-cards"))
                             .ReturnsAsync(cachedCards);

            // Act
            var result = await _cardService.GetAllAsync();

            // Assert
            Assert.Equal(cachedCards, result);
            _cacheServiceMock.Verify(cs => cs.GetAsync<IEnumerable<CardDto>>("all-cards"), Times.Once);
            _cardRepositoryMock.Verify(cr => cr.GetAllAsync(), Times.Never);  // DB access should not happen
        }

        [Fact]
        public async Task GetAllAsync_ReturnsCardsFromDb_WhenCacheIsNotAvailable()
        {
            // Arrange
            var dbCards = new List<Card> { new Card { Id = 1, Name = "Card 1" } };
            _cacheServiceMock.Setup(cs => cs.GetAsync<IEnumerable<CardDto>>("all-cards")).ReturnsAsync((IEnumerable<CardDto>)null);
            _cardRepositoryMock.Setup(cr => cr.GetAllAsync()).ReturnsAsync(dbCards);
            _mapperMock.Setup(m => m.Map<IEnumerable<CardDto>>(dbCards)).Returns(new List<CardDto> { new CardDto { Id = 1, Name = "Card 1" } });

            // Act
            var result = await _cardService.GetAllAsync();

            // Assert
            Assert.Single(result);
            _cacheServiceMock.Verify(cs => cs.GetAsync<IEnumerable<CardDto>>("all-cards"), Times.Once);
            _cardRepositoryMock.Verify(cr => cr.GetAllAsync(), Times.Once);  // DB should be accessed once
            _cacheServiceMock.Verify(cs => cs.SetAsync("all-cards", It.IsAny<IEnumerable<CardDto>>(), TimeSpan.FromMinutes(10)), Times.Once);  // Cache should be updated
        }

        [Fact]
        public async Task ReserveStockAsync_ThrowsException_WhenInsufficientStock()
        {
            // Arrange
            var card = new Card { Id = 1, Stock = 10, ReservedStock = 0 };
            _cardRepositoryMock.Setup(cr => cr.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(card);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _cardService.ReserveStockAsync(1, 20));
            Assert.Equal("Insufficient stock", exception.Message);
            _cardRepositoryMock.Verify(cr => cr.SaveChangesAsync(), Times.Never);  // No save should happen
        }

        [Fact]
        public async Task CommitStockAsync_UpdatesStock_WhenSufficientReservedStock()
        {
            // Arrange
            var card = new Card { Id = 1, Stock = 10, ReservedStock = 5 };
            _cardRepositoryMock.Setup(cr => cr.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(card);

            // Act
            await _cardService.CommitStockAsync(1, 5);

            // Assert
            Assert.Equal(0, card.ReservedStock);
            Assert.Equal(5, card.Stock);
            _cardRepositoryMock.Verify(cr => cr.SaveChangesAsync(), Times.Once);
            _cardCacheServiceMock.Verify(ccs => ccs.RemoveCardFromCache(1), Times.Once);  // Cache invalidation
        }

        [Fact]
        public async Task ReleaseStockAsync_UpdatesReservedStock_WhenCalled()
        {
            // Arrange
            var card = new Card { Id = 1, Stock = 10, ReservedStock = 5 };
            _cardRepositoryMock.Setup(cr => cr.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(card);

            // Act
            await _cardService.ReleaseStockAsync(1, 3);

            // Assert
            Assert.Equal(2, card.ReservedStock);
            _cardRepositoryMock.Verify(cr => cr.SaveChangesAsync(), Times.Once);
            _cardCacheServiceMock.Verify(ccs => ccs.RemoveCardFromCache(1), Times.Once);  // Cache invalidation
        }

        [Fact]
        public async Task CommitStockAsync_ThrowsException_WhenNotEnoughReservedStock()
        {
            // Arrange
            var card = new Card { Id = 1, Stock = 10, ReservedStock = 2 };
            _cardRepositoryMock.Setup(cr => cr.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(card);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _cardService.CommitStockAsync(1, 5));
            Assert.Equal("Not enough reserved stock", exception.Message);
            _cardRepositoryMock.Verify(cr => cr.SaveChangesAsync(), Times.Never);  // Save should not happen
            _cardCacheServiceMock.Verify(ccs => ccs.RemoveCardFromCache(1), Times.Never);  // Cache should not be invalidated
        }
    }
}
