using Xunit;
using Moq;
using FluentAssertions;
using CardCatalogService.Application.Interfaces;
using CardCatalogService.Application.DTOs;
using CardCatalogService.Domain.Entities;
using CardCatalogService.Application.Services;
using AutoMapper;

namespace CardCatalogService.Tests.Services
{
    public class CardServiceTest
    {
        private readonly Mock<ICardRepository> _cardRepositoryMock;
        private readonly Mock<IReservationRepository> _reservationRepositoryMock;
        private readonly Mock<ICardCacheService> _cardCacheServiceMock;
        private readonly Mock<ICacheService> _cacheServiceMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly ICardService _cardService;

        public CardServiceTest()
        {
            _cardRepositoryMock = new Mock<ICardRepository>();
            _reservationRepositoryMock = new Mock<IReservationRepository>();
            _cacheServiceMock = new Mock<ICacheService>();
            _cardCacheServiceMock = new Mock<ICardCacheService>();
            _mapperMock = new Mock<IMapper>();

            _cardService = new CardService(
                _cardRepositoryMock.Object,
                _mapperMock.Object,
                _cacheServiceMock.Object,
                _cardCacheServiceMock.Object,
                _reservationRepositoryMock.Object
            );
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllCardsWithCalculatedAvailableStock()
        {
            // Arrange
            var cards = new List<Card>
            {
                new Card { Id = 1, Name = "Blue Eyes White Dragon", Stock = 10 },
                new Card { Id = 2, Name = "Dark Magician", Stock = 5 }
            };

            _cardRepositoryMock.Setup(r => r.GetAllAsync())
                .ReturnsAsync(cards);

            _reservationRepositoryMock.Setup(r => r.GetActiveReservationsByCardIdAsync(It.IsAny<int>()))
                .ReturnsAsync(new List<CardReservation>());

            _cardCacheServiceMock.Setup(c => c.GetAsync<List<CardDto>>(It.IsAny<string>()))
                .ReturnsAsync((List<CardDto>)null);

            // Act
            var result = await _cardService.GetAllAsync();

            // Assert
            result.Should().HaveCount(2);
            result.First().CalculatedAvailableStock.Should().Be(10);
        }

        [Fact]
        public async Task SearchPagedAsync_ShouldReturnPagedCardsWithCalculatedAvailableStock_AndCacheIt()
        {
            // Arrange
            var parameters = new CardSearchParameters
            {
                Page = 1,
                PageSize = 10
            };

            var dummyCards = new List<Card>
    {
        new Card
        {
            Id = 1,
            Name = "Red Eyes Black Dragon",
            Stock = 8
        }
    };

            var dummyCardDtos = new List<CardDto>
    {
        new CardDto
        {
            Id = 1,
            Name = "Red Eyes Black Dragon",
            Stock = 8,
            CalculatedAvailableStock = 8
        }
    };

            // Cache'de olmadığını simüle ediyoruz (yani DB'ye gidecek)
            _cacheServiceMock.Setup(c => c.GetAsync<PagedList<CardDto>>(It.IsAny<string>()))
                .ReturnsAsync((PagedList<CardDto>)null);

            // Repository kartları dönecek
            _cardRepositoryMock.Setup(r => r.SearchPagedAsync(parameters))
                .ReturnsAsync((dummyCards, 1));

            // Reservationlardan boş dönecek (aktif rezervasyon yok)
            _reservationRepositoryMock.Setup(r => r.GetActiveReservationsByCardIdAsync(It.IsAny<int>()))
                .ReturnsAsync(new List<CardReservation>());

            // Mapper mock'u
            _mapperMock.Setup(m => m.Map<List<CardDto>>(It.IsAny<object>()))
                .Returns(dummyCardDtos);

            // Act
            var result = await _cardService.SearchPagedAsync(parameters);

            // Assert
            result.Should().NotBeNull();
            result.TotalCount.Should().Be(1);
            result.Data.First().CalculatedAvailableStock.Should().Be(8);

            // ✅ Asıl kritik: Cache'e yazılmış mı kontrol ediyoruz
            _cacheServiceMock.Verify(c => c.SetAsync(
                It.IsAny<string>(),
                It.IsAny<PagedList<CardDto>>(),
                It.IsAny<TimeSpan>()
            ), Times.Once);
        }


        [Fact]
        public async Task SearchPagedAsync_ShouldReturnFromCache_WhenExists()
        {
            // Arrange
            var parameters = new CardSearchParameters
            {
                Page = 1,
                PageSize = 10
            };

            var dummyCards = new List<CardDto>
    {
        new CardDto
        {
            Id = 1,
            Name = "Cached Dragon",
            Stock = 10,
            CalculatedAvailableStock = 9
        }
    };

            var cachedPagedList = new PagedList<CardDto>(
                dummyCards,
                page: 1,
                pageSize: 10,
                totalCount: 1
            );

            _cardCacheServiceMock.Setup(c => c.GetAsync<PagedList<CardDto>>(It.IsAny<string>()))
                .ReturnsAsync(cachedPagedList);

            _cacheServiceMock.Setup(c => c.GetAsync<PagedList<CardDto>>(It.IsAny<string>()))
                .ReturnsAsync(cachedPagedList);

            _mapperMock.Setup(m => m.Map<List<CardDto>>(It.IsAny<object>()))
                .Returns(dummyCards);

            // Act
            var result = await _cardService.SearchPagedAsync(parameters);

            // Assert
            result.Should().NotBeNull();
            result.TotalCount.Should().Be(1);
            result.Data.First().Name.Should().Be("Cached Dragon");

            _cardRepositoryMock.Verify(r => r.SearchPagedAsync(It.IsAny<CardSearchParameters>()), Times.Never);
        }

    }
}
