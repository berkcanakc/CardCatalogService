using Xunit;
using Moq;
using AutoMapper;
using CardCatalogService.Application.DTOs;
using CardCatalogService.Application.Interfaces;
using CardCatalogService.Application.Services;
using CardCatalogService.Domain.Entities;

namespace CardCatalogService.Tests.Services
{
    public class CardServiceTests
    {
        private readonly Mock<ICardRepository> _mockRepo;
        private readonly Mock<ICacheService> _mockCache;
        private readonly Mock<IMapper> _mockMapper;
        private readonly ICardService _cardService;

        public CardServiceTests()
        {
            _mockRepo = new Mock<ICardRepository>();
            _mockCache = new Mock<ICacheService>();
            _mockMapper = new Mock<IMapper>();

            _cardService = new CardService(
                _mockRepo.Object,
                _mockMapper.Object,
                _mockCache.Object
            );
        }

        [Fact]
        public async Task GetAllAsync_WhenCacheIsNotNull_ReturnsFromCache()
        {
            var cached = new List<CardDto> { new() { Id = 1, Name = "Slifer" } };
            _mockCache.Setup(x => x.GetAsync<IEnumerable<CardDto>>("all-cards"))
                      .ReturnsAsync(cached);

            var result = await _cardService.GetAllAsync();

            Assert.Equal(cached, result);
            _mockRepo.Verify(x => x.GetAllAsync(), Times.Never);
        }

        [Fact]
        public async Task GetAllAsync_WhenCacheIsEmpty_FetchesFromRepoAndCaches()
        {
            _mockCache.Setup(x => x.GetAsync<IEnumerable<CardDto>>("all-cards"))
                      .ReturnsAsync((IEnumerable<CardDto>)null);

            var repoData = new List<Card> { new() { Id = 1, Name = "Obelisk" } };
            var mapped = new List<CardDto> { new() { Id = 1, Name = "Obelisk" } };

            _mockRepo.Setup(x => x.GetAllAsync()).ReturnsAsync(repoData);
            _mockMapper.Setup(m => m.Map<IEnumerable<CardDto>>(repoData)).Returns(mapped);

            var result = await _cardService.GetAllAsync();

            Assert.Equal(mapped, result);
            _mockCache.Verify(x => x.SetAsync(
                "all-cards",
                It.IsAny<IEnumerable<CardDto>>(),
                It.IsAny<TimeSpan>()), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_WhenCardExists_ReturnsCardDto()
        {
            var card = new Card { Id = 1, Name = "Ra" };
            var dto = new CardDto { Id = 1, Name = "Ra" };

            _mockRepo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(card);
            _mockMapper.Setup(m => m.Map<CardDto>(card)).Returns(dto);

            var result = await _cardService.GetByIdAsync(1);

            Assert.NotNull(result);
            Assert.Equal(dto.Name, result.Name);
        }

        [Fact]
        public async Task GetByIdAsync_WhenCardNotFound_ReturnsNull()
        {
            _mockRepo.Setup(x => x.GetByIdAsync(99)).ReturnsAsync((Card)null!);

            var result = await _cardService.GetByIdAsync(99);

            Assert.Null(result);
        }

        [Fact]
        public async Task SearchPagedAsync_WithMatchingCriteria_ReturnsFilteredCards()
        {
            var parameters = new CardSearchParameters
            {
                Page = 1,
                PageSize = 10,
                Query = "Magician"
            };

            var cards = new List<Card> { new() { Id = 1, Name = "Dark Magician" } };
            var dtos = new List<CardDto> { new() { Id = 1, Name = "Dark Magician" } };

            _mockRepo.Setup(r => r.SearchPagedAsync(parameters)).ReturnsAsync((cards, 1));
            _mockMapper.Setup(m => m.Map<List<CardDto>>(cards)).Returns(dtos);

            var result = await _cardService.SearchPagedAsync(parameters);

            Assert.Single(result.Data);
            Assert.Equal(1, result.TotalCount);
        }

        [Fact]
        public async Task SearchPagedAsync_WithNoMatch_ReturnsEmptyList()
        {
            var parameters = new CardSearchParameters
            {
                Page = 1,
                PageSize = 10,
                Query = "Unicorn"
            };

            var empty = new List<Card>();
            _mockRepo.Setup(r => r.SearchPagedAsync(parameters)).ReturnsAsync((empty, 0));
            _mockMapper.Setup(m => m.Map<List<CardDto>>(empty)).Returns(new List<CardDto>());

            var result = await _cardService.SearchPagedAsync(parameters);

            Assert.Empty(result.Data);
            Assert.Equal(0, result.TotalCount);
        }
    }
}
