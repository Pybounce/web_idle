namespace unit_tests;

using Moq;
using Bogus;
using Bogus.Extensions.UnitedKingdom;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using NuGet.Frameworks;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using System.Reflection.Metadata.Ecma335;

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
    public void OnResourceHarvestComplete___LootTableChanceEqualsOne___EventRaisedEveryTime() {
        var resourceId = _faker.Random.Int(0);
        var itemId = _faker.Random.Int(0);
        var itemChance = new LootTableItem() {
            ItemId = itemId,
            ChanceDenominator = 1
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
    
        int testsToRun = 500000;


        for (int i = 0; i < testsToRun; i++) {
            _lootSystem.OnResourceHarvestComplete(resourceHarvestCompleteEvent);
        }


        _eventHub.Verify(x => x.Publish(It.IsAny<ItemGained>()), Times.Exactly(testsToRun));
    }

    private class LootDataStub : ILootDataService
    {
        private LootTable _table;
        public LootDataStub(LootTable table) {
            _table = table;
        }

        public Task InitAsync()
        {
            throw new NotImplementedException();
        }

        public bool TryGetTable(int resourceId, out LootTable table)
        {
            table = _table;
            return true;
        }
    }

    /// <summary>
    /// This might be the worst thing I've written in a while however...
    /// <br>Using Moq added too much overhead to the 100M calls</br>
    /// </summary>
    [Test]
    public void OnResourceHarvestComplete___ManyLootTableChances___StatisticallyCorrectAmountOfEventsRaised() {
        
        var resourceId = _faker.Random.Int(0);
        var tableItemCount = _faker.Random.Int(5, 20);
        var lootTableItems = new LootTableItem[tableItemCount];
        var baseItemChances = new Dictionary<int, double>();

        for (int i = 0; i < tableItemCount; i++) {
            var itemId = _faker.Random.Int(0);
            var chanceDenominator = _faker.Random.Int(1, 100_000);
            var itemChance = new LootTableItem() {
                ItemId = itemId,
                ChanceDenominator = chanceDenominator
            };
            lootTableItems[i] = itemChance;
            double baseChance = 1.0 / (double)chanceDenominator;

            if (baseItemChances.ContainsKey(itemId)) {
                baseItemChances[itemId] += baseChance;
            }
            else {
                baseItemChances.Add(itemId, baseChance);
            }
        }

        LootTable table = new LootTable() {
            id = "",
            ResourceId = resourceId,
            ItemChances = lootTableItems
        };

        var resourceHarvestCompleteEvent = new ResourceHarvestComplete() {
            ResourceId = resourceId
        };

        var actualRaised = new Dictionary<int, int>();
        var eventHub = new EventHub();
        var incrementActual = (ItemGained e) => {
            if (actualRaised.ContainsKey(e.ItemId)) {
                actualRaised[e.ItemId] += 1;
            }
            else {
                actualRaised.Add(e.ItemId, 1);
            }
        };
        eventHub.Subscribe<ItemGained>(incrementActual);

        _lootSystem = new LootSystem(eventHub, new LootDataStub(table), new RandomNumberGenerator());
    
        int testsToRun = 100_000_000;


        for (int i = 0; i < testsToRun; i++) {
            _lootSystem.OnResourceHarvestComplete(resourceHarvestCompleteEvent);
        }
        
        for (int i = 0; i < tableItemCount; i++) {
            double expectedItemsGained = baseItemChances[lootTableItems[i].ItemId] * (double)testsToRun;
            int minExpectedItemsGained = (int)(expectedItemsGained * 0.9);
            int maxExpectedItemsGained = (int)(expectedItemsGained * 1.1);
            var itemsGained = actualRaised[lootTableItems[i].ItemId];
            Assert.That(itemsGained >= minExpectedItemsGained && itemsGained <= maxExpectedItemsGained);
        }
        //Assert.That(true);
    }


    //Chance = 1 -> always happens




}