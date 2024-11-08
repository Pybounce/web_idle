namespace unit_tests;

using Moq;



[TestFixture]
public class EventHubTests {


    public class SomeType {
        public int SomeInt { get; set; }
    }
    public class SomeOtherType {
        public int SomeOtherInt { get; set; }
    }

    private EventHub _eventHub;
    [SetUp]
    public void SetUp() {
        _eventHub = new EventHub();
    }

    [TearDown]
    public void TearDown() {
        _eventHub.Dispose();
    } 
    
    [Test]
    public void Publishing_To_One_Subscriber___One_Subscriber_Invoked() {
        var mockHandler = new Mock<Action<SomeType>>();
        var someInt = 1;
        _eventHub.Subscribe<SomeType>(mockHandler.Object);

        _eventHub.Publish<SomeType>(new SomeType { SomeInt = someInt });

        mockHandler.Verify(h => h(It.Is<SomeType>(st => st.SomeInt == someInt)), Times.Once);
    }

    [Test]
    public void Publishing_To_Many_Subscribers_Same_Type___Many_Subscribers_Invoked() {
        var mockHandler = new Mock<Action<SomeType>>();
        var someInt = 1;
        _eventHub.Subscribe<SomeType>(mockHandler.Object);
        _eventHub.Subscribe<SomeType>(mockHandler.Object);

        _eventHub.Publish<SomeType>(new SomeType { SomeInt = someInt });

        mockHandler.Verify(h => h(It.Is<SomeType>(st => st.SomeInt == someInt)), Times.Exactly(2));
    }

    [Test]
    public void Publishing_To_Many_Subscribers_Different_Type___Only_Same_Type_Subscribers_Invoked() {
        var someMockHandler = new Mock<Action<SomeType>>();
        var someOtherMockHandler = new Mock<Action<SomeOtherType>>();
        var someInt = 1;
        _eventHub.Subscribe<SomeType>(someMockHandler.Object);
        _eventHub.Subscribe<SomeOtherType>(someOtherMockHandler.Object);

        _eventHub.Publish<SomeType>(new SomeType { SomeInt = someInt });

        someMockHandler.Verify(h => h(It.Is<SomeType>(st => st.SomeInt == someInt)), Times.Once);
        someOtherMockHandler.Verify(h => h(It.Is<SomeOtherType>(st => st.SomeOtherInt == someInt)), Times.Never);
    }

    [Test]
    public void Publishing_To_Zero_Subscribers___No_Exceptions_Thrown() {
        var someInt = 1;

        _eventHub.Publish<SomeType>(new SomeType { SomeInt = someInt });

        Assert.Pass("No exceptions were thrown");
    }

    [Test]
    public void Subscribing_On_Multiple_Threads___All_Subscribers_Successfully_Added() {
        int someInt = 3;
        int threadCount = 1000;
        var someMockHandler = new Mock<Action<SomeType>>();
        Parallel.For(0, threadCount, (i) => {
            _eventHub.Subscribe<SomeType>(someMockHandler.Object);
        });

        _eventHub.Publish<SomeType>(new SomeType { SomeInt = someInt });

        someMockHandler.Verify(h => h(It.Is<SomeType>(st => st.SomeInt == someInt)), Times.Exactly(threadCount));
    }

}