namespace unit_tests;


[TestFixture]
public class TickTests {

    [TestCase(100, 10, true)]
    [TestCase(-10, -100, true)]
    [TestCase(-10, int.MaxValue, true)]
    [TestCase(10, 100, false)]
    [TestCase(-100, -10, false)]
    [TestCase(int.MaxValue, -10, false)]
    [TestCase(0, -1, true)]
    [TestCase(int.MinValue, int.MaxValue, true)]
    public void TickA_GreaterThan_LessThan_TickB___Returns_Expected(int rawTickA, int rawTickB, bool expectedGreaterThanResult) {
        var tickA = new Tick(rawTickA);
        var tickB = new Tick(rawTickB);

        var greaterThanResult = tickA > tickB;
        var lessThanResult = tickA < tickB;

        Assert.That(greaterThanResult == expectedGreaterThanResult);
        Assert.That(lessThanResult == !greaterThanResult);
    }

    [TestCase(100, 100, true)]
    [TestCase(-100, -100, true)]
    [TestCase(int.MaxValue, int.MaxValue, true)]
    [TestCase(int.MinValue, int.MinValue, true)]
    [TestCase(int.MaxValue, int.MinValue, false)]
    [TestCase(0, 0, true)]
    public void TickA_Equals_NotEquals_TickB___Returns_Expected(int rawTickA, int rawTickB, bool expectedEqualsResult) {
        var tickA = new Tick(rawTickA);
        var tickB = new Tick(rawTickB);

        var equalsResult = tickA == tickB;
        var notEqualsResult = tickA != tickB;

        Assert.That(equalsResult == expectedEqualsResult);
        Assert.That(notEqualsResult == !equalsResult);
    }

    [TestCase(0, 1)]
    [TestCase(-1, 0)]
    [TestCase(int.MaxValue, int.MinValue)]
    public void Tick_Next___Raw_Tick_As_Expected(int startRawTick, int expectedRawTick) {
        var tick = new Tick(startRawTick);

        tick.Next();

        Assert.That(tick.RawTick == expectedRawTick);
    }
}