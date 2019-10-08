using FluentAssertions;
using Xunit;

namespace Bearded.UI.Tests.Core
{
    public static class AnchorTests
    {
        private const double epsilon = 0.01;

        public sealed class CalculatePointWithin
        {
            [Fact]
            public void AddsFixedOffset()
            {
                var anchor = new Anchor(percentage: 0, offset: 100);
                var interval = Interval.FromStartAndSize(0, 1000);

                var resultingPoint = anchor.CalculatePointWithin(interval);

                resultingPoint.Should().BeApproximately(100, epsilon);
            }

            [Fact]
            public void AddsIntervalStart()
            {
                var anchor = new Anchor(percentage: 0, offset: 200);
                var interval = Interval.FromStartAndSize(200, 800);

                var resultingPoint = anchor.CalculatePointWithin(interval);

                resultingPoint.Should().BeApproximately(400, epsilon);
            }

            [Fact]
            public void UsesPercentage()
            {
                var anchor = new Anchor(percentage: 0.33, offset: 0);
                var interval = Interval.FromStartAndSize(0, 1000);

                var resultingPoint = anchor.CalculatePointWithin(interval);

                resultingPoint.Should().BeApproximately(330, epsilon);
            }

            [Fact]
            public void WorksWithNegativeOffset()
            {
                var anchor = new Anchor(percentage: 0, offset: -200);
                var interval = Interval.FromStartAndSize(0, 1000);

                var resultingPoint = anchor.CalculatePointWithin(interval);

                resultingPoint.Should().BeApproximately(-200, epsilon);
            }

            [Fact]
            public void WorksWithNegativePercentage()
            {
                var anchor = new Anchor(percentage: -0.33, offset: 0);
                var interval = Interval.FromStartAndSize(0, 1000);

                var resultingPoint = anchor.CalculatePointWithin(interval);

                resultingPoint.Should().BeApproximately(-330, epsilon);
            }

            [Fact]
            public void WorksWithOffsetLargerThanInterval()
            {
                var anchor = new Anchor(percentage: 0, offset: 1200);
                var interval = Interval.FromStartAndSize(0, 1000);

                var resultingPoint = anchor.CalculatePointWithin(interval);

                resultingPoint.Should().BeApproximately(1200, epsilon);
            }

            [Fact]
            public void WorksWithPercentageLargerThanOne()
            {
                var anchor = new Anchor(percentage: 2, offset: 0);
                var interval = Interval.FromStartAndSize(0, 1000);

                var resultingPoint = anchor.CalculatePointWithin(interval);

                resultingPoint.Should().BeApproximately(2000, epsilon);
            }

            [Fact]
            public void AddsOffsetsCorrectly()
            {
                var anchor = new Anchor(percentage: 0.2, offset: 200);
                var interval = Interval.FromStartAndSize(200, 1000);

                var resultingPoint = anchor.CalculatePointWithin(interval);

                resultingPoint.Should().BeApproximately(600, epsilon);
            }
        }
    }
}
