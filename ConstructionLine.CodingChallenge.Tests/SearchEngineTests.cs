using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace ConstructionLine.CodingChallenge.Tests
{

    [TestFixture]
    public class SearchEngineTests : SearchEngineTestsBase
    {

        private static List<Shirt> _shirtList = new List<Shirt>
        {
            new Shirt(Guid.NewGuid(), "Black - Small", Size.Small, Color.Black),
            new Shirt(Guid.NewGuid(), "Black - Medium", Size.Medium, Color.Black),
            new Shirt(Guid.NewGuid(), "Black - Large", Size.Large, Color.Black),
            new Shirt(Guid.NewGuid(), "Blue - Small", Size.Small, Color.Blue),
            new Shirt(Guid.NewGuid(), "Blue - Medium", Size.Medium, Color.Blue),
            new Shirt(Guid.NewGuid(), "Red - Medium", Size.Medium, Color.Red),
            new Shirt(Guid.NewGuid(), "Red - Large", Size.Large, Color.Red),
            new Shirt(Guid.NewGuid(), "White - Medium", Size.Medium, Color.White),
        };
    


        [Test]
        public void SearchEngine_WithNullAsInput_ShouldThrowException()
        {
            // Arrange
            Action action = () =>
            {
                var _ = new SearchEngine(null);
            };

            // Act / Assert
            action.Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void Search_WithNoShirts_ShouldNotReturnAnyResult()
        {
            // Arrange
            var shirts = new List<Shirt>();
            var searchEngine = new SearchEngine(shirts);
            var searchOptions = new SearchOptions();

            // Act
            var results = searchEngine.Search(searchOptions);

            // Assert
            var expectedResults = GenerateSearchResults();

            results.Should().BeEquivalentTo(expectedResults);
        }


       

        [Test]
        public void  Search_WithNullAsInput_ShouldThrowException()
        {
            // Arrange
            var shirts = _shirtList;
            var searchEngine = new SearchEngine(shirts);
            Action action = () => searchEngine.Search(null);

            // Act/Assert
            action.Should().Throw<ArgumentNullException>();
        }

        [Test]
        [TestCaseSource(nameof(Search_WithEmptySearchOptions_ShouldReturnAllResults_TestCaseSource))]
        public void Search_WithEmptySearchOptions_ShouldReturnAllResults(SearchOptions searchOptions)
        {
            // Arrange
            var shirts = _shirtList;
            var searchEngine = new SearchEngine(shirts);

            // Act
            var results = searchEngine.Search(searchOptions);

            // Assert
            var expectedShirtResults = shirts;
            var expectedColourCounts = new List<ColorCount>
            {
                new ColorCount { Color = Color.Black, Count = 3 },
                new ColorCount { Color = Color.Blue, Count = 2 },
                new ColorCount { Color = Color.Red, Count = 2 },
                new ColorCount { Color = Color.White, Count = 1 },
            };
            var expectedSizeCounts = new List<SizeCount>
            {
                new SizeCount { Size = Size.Small, Count = 2 },
                new SizeCount { Size = Size.Medium, Count = 4 },
                new SizeCount { Size = Size.Large, Count = 2 }
            };
            var expectedResults = GenerateSearchResults(expectedShirtResults, expectedColourCounts, expectedSizeCounts);

            results.Should().BeEquivalentTo(expectedResults);
        }

       

        [Test]
        [TestCaseSource(nameof(Search_WithCorrectSearchOptions_ShouldReturnResult_TestCaseSource))]
        public void Search_WithCorrectSearchOptions_ShouldReturnCorrectResult(SearchOptions searchOptions, SearchResults expectedSearchResults)
        {
            // Arrange
            var shirts = _shirtList;
            var searchEngine = new SearchEngine(shirts);

            // Act
            var searchResults = searchEngine.Search(searchOptions);

            // Assert
            expectedSearchResults.Should().BeEquivalentTo(searchResults);
        }

        #region Helpers
        private static IEnumerable Search_WithEmptySearchOptions_ShouldReturnAllResults_TestCaseSource()
        {
            yield return new TestCaseData(new SearchOptions())
                .SetName("{m}_EmptySearchOptions");
            yield return new TestCaseData(new SearchOptions { Colors = new List<Color>(), Sizes = new List<Size>() })
                .SetName("{m}_EmptySizeAndColorTerms");
        }

        private static IEnumerable Search_WithCorrectSearchOptions_ShouldReturnResult_TestCaseSource()
        {
            yield return new TestCaseData(
                    new SearchOptions { Colors = new List<Color> { Color.Black } },
                    GenerateSearchResults(
                        _shirtList.Where(shirt => shirt.Color == Color.Black).ToList(),
                        new List<ColorCount>
                        {
                            new ColorCount { Color = Color.Black, Count = 3 }
                        },
                        new List<SizeCount>
                        {
                            new SizeCount { Size = Size.Small, Count = 1 },
                            new SizeCount { Size = Size.Medium, Count = 1 },
                            new SizeCount { Size = Size.Large, Count = 1 }
                        }))
                .SetName("{m}_Search_Black_Shirts_Expect_3_Black_Shirts_Small_Medium_Large");

            yield return new TestCaseData(
                    new SearchOptions { Colors = new List<Color> { Color.Yellow } },
                    GenerateSearchResults())
                .SetName("{m}_Search_Yellow_Shirts_Expect_No_Results");

            yield return new TestCaseData(
                    new SearchOptions { Sizes = new List<Size> { Size.Small } },
                    GenerateSearchResults(
                        _shirtList.Where(shirt => shirt.Size == Size.Small).ToList(),
                        new List<ColorCount>
                        {
                            new ColorCount { Color = Color.Black, Count = 1 },
                            new ColorCount { Color = Color.Blue, Count = 1 }
                        },
                        new List<SizeCount>
                        {
                            new SizeCount { Size = Size.Small, Count = 2 }
                        }))
                .SetName("{m}_Search_Small_Shirts_Expect_1_Black_1_Blue");

            yield return new TestCaseData(
                    new SearchOptions
                    {
                        Sizes = new List<Size> { Size.Small },
                        Colors = new List<Color> { Color.Black }
                    },
                    GenerateSearchResults(
                        _shirtList.Where(shirt =>
                            shirt.Size == Size.Small && shirt.Color == Color.Black).ToList(),
                        new List<ColorCount>
                        {
                            new ColorCount { Color = Color.Black, Count = 1 }
                        },
                        new List<SizeCount>
                        {
                            new SizeCount { Size = Size.Small, Count = 1 }
                        }))
                .SetName("{m}_Search_Small_Black_Shirts_Expect_1_Small_Black");

            yield return new TestCaseData(
                    new SearchOptions
                    {
                        Sizes = new List<Size> { Size.Small, Size.Medium },
                        Colors = new List<Color> { Color.Black, Color.Red }
                    },
                    GenerateSearchResults(
                        _shirtList.Where(shirt =>
                            (shirt.Size == Size.Small || shirt.Size == Size.Medium)
                            && (shirt.Color == Color.Black || shirt.Color == Color.Red)).ToList(),
                        new List<ColorCount>
                        {
                            new ColorCount { Color = Color.Black, Count = 2 },
                            new ColorCount { Color = Color.Red, Count = 1 }
                        },
                        new List<SizeCount>
                        {
                            new SizeCount { Size = Size.Small, Count = 1 },
                            new SizeCount { Size = Size.Medium, Count = 2 }
                        }))
                .SetName("{m}_Search_Small_Medium_Black_Red_Shirts_Expect_Black_Small_Medium_And_Red_Medium");
        }



        private static SearchResults GenerateSearchResults(
            List<Shirt> shirts = null,
            List<ColorCount> colorCounts = null,
            List<SizeCount> sizeCounts = null)
        {
            return new SearchResults
            {
                Shirts = shirts ?? new List<Shirt>(),
                ColorCounts = Color.All.Select(color =>
                        colorCounts?.SingleOrDefault(colorCount => colorCount.Color == color)
                        ?? new ColorCount { Color = color, Count = 0 })
                    .ToList(),
                SizeCounts = Size.All.Select(size =>
                        sizeCounts?.SingleOrDefault(sizeCount => sizeCount.Size == size)
                        ?? new SizeCount() { Size = size, Count = 0 })
                    .ToList()
            };
        } 
        #endregion
    }
}
