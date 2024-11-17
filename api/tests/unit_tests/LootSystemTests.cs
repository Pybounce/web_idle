namespace unit_tests;

using Moq;
using Bogus;
using Bogus.Extensions.UnitedKingdom;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;

[TestFixture]
public class LootSystemTests {

    
    private Mock<IEventHub> _eventHub;
    private Mock<IRandomNumberGenerator> _rng;
    private Mock<ILootDataService> _lootData;
    private LootSystem _lootSystem;
    private Faker _faker;
    
    [SetUp]
    public void SetUp() {
        _faker = new Faker();
        _eventHub = new Mock<IEventHub>();
        _rng = new Mock<IRandomNumberGenerator>();
        _lootData = new Mock<ILootDataService>();
        _lootSystem = new LootSystem(_eventHub.Object, _lootData.Object, _rng.Object);
    }

    [TearDown]
    public void TearDown() {
        _lootSystem.Dispose();
    } 
    
    [Test]
    public void OnResourceHarvestComplete___NoLootTable___NoEventsRaised() {

        var resourceId = _faker.Random.Int();
        LootTable table = null;
        _lootData.Setup(m => m.TryGetTable(It.IsAny<int>(), out table)).Returns(false);
        var resourceHarvestCompleteEvent = new ResourceHarvestComplete() {
            ResourceId = resourceId
        };

        _lootSystem.OnResourceHarvestComplete(resourceHarvestCompleteEvent);

        _eventHub.Verify(x => x.Publish(It.IsAny<ItemGained>()), Times.Never);
    }
    [Test]
    public void OnResourceHarvestComplete___LootTableContainsNoItemChances___NoEventsRaised() {

        var resourceId = _faker.Random.Int(0);
        LootTable table = new LootTable() {
            id = "",
            ResourceId = resourceId,
            ItemChances = []
        };
        _lootData.Setup(m => m.TryGetTable(It.IsAny<int>(), out table)).Returns(true);
        var resourceHarvestCompleteEvent = new ResourceHarvestComplete() {
            ResourceId = resourceId
        };

        _lootSystem.OnResourceHarvestComplete(resourceHarvestCompleteEvent);

        _eventHub.Verify(x => x.Publish(It.IsAny<ItemGained>()), Times.Never);
    }

    [TestCase(0)]
    [TestCase(-1)]
    public void OnResourceHarvestComplete___LootTableChanceNegativeOrZero___ChanceIgnored(int chanceDenominatorMul) {
        var resourceId = _faker.Random.Int(0);
        var chanceDenominator = _faker.Random.Int(1) * chanceDenominatorMul;
        var itemChance = new LootTableItem() {
            ItemId = 1,
            ChanceDenominator = chanceDenominator
        };
        LootTable table = new LootTable() {
            id = "",
            ResourceId = resourceId,
            ItemChances = [itemChance]
        };
        _lootData.Setup(m => m.TryGetTable(It.IsAny<int>(), out table)).Returns(true);
        var resourceHarvestCompleteEvent = new ResourceHarvestComplete() {
            ResourceId = resourceId
        };
        _rng.Setup(m => m.Next(It.IsAny<int>())).Returns(chanceDenominator - 1); //.Next() is maxExclusive so can't return chanceDenominator or higher

        _lootSystem.OnResourceHarvestComplete(resourceHarvestCompleteEvent);
        
        _eventHub.Verify(x => x.Publish(It.IsAny<ItemGained>()), Times.Never);
    }

    [Test]
    public void OnResourceHarvestComplete___LootTableChance___StatisticallyCorrectAmountOfEventsRaised() {
        var resourceId = _faker.Random.Int(0);
        var itemId = _faker.Random.Int(0);
        var chanceDenominator = _faker.Random.Int(1);
        var itemChance = new LootTableItem() {
            ItemId = itemId,
            ChanceDenominator = chanceDenominator
        };
        LootTable table = new LootTable() {
            id = "",
            ResourceId = resourceId,
            ItemChances = [itemChance]
        };
        _lootData.Setup(m => m.TryGetTable(It.IsAny<int>(), out table)).Returns(true);
        var resourceHarvestCompleteEvent = new ResourceHarvestComplete() {
            ResourceId = resourceId
        };
        _lootSystem = new LootSystem(_eventHub.Object, _lootData.Object, new RandomNumberGenerator());
    
        double chance = 1.0 / (double)itemChance.ChanceDenominator;
        int testsRun = 5000;
        double expectedItemsGained = chance * testsRun;
        int minExpectedItemsGained = (int)(expectedItemsGained * 0.95);
        int maxExpectedItemsGained = (int)(expectedItemsGained * 1.05);

        while (testsRun > 0) {
            _lootSystem.OnResourceHarvestComplete(resourceHarvestCompleteEvent);
            testsRun -= 1;
        }

        _eventHub.Verify(x => x.Publish(It.IsAny<ItemGained>()), Times.Between(minExpectedItemsGained, maxExpectedItemsGained, Moq.Range.Inclusive));
    }

    //[TestCase(0)]
    public void asd(int rngReturn) {

        _rng.Setup(m => m.Next(It.IsAny<int>())).Returns(rngReturn);

        Assert.That(true);
    }
}