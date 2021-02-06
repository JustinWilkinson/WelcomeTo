using System.Collections.Generic;
using System.Linq;
using WelcomeTo.Shared.Abstractions;
using WelcomeTo.Shared.Enumerations;
using Xunit;

namespace WelcomeTo.Test.Shared
{
    public class StreetTests
    {
        [Fact]
        public void GetEstates_ForGivenStreet_ReturnsCorrectValues()
        {
            // Arrange 
            var street = new Street
            {
                Houses = new List<House>()
                {
                    new House { Index = 0, Number = 1 },
                    new House { Index = 1, Number = 2 },
                    new House { Index = 2, Number = 3, FenceBuilt = true },
                    new House { Index = 3, Number = null },
                    new House { Index = 4, Number = 5 },
                    new House { Index = 5, Number = 6 },
                    new House { Index = 6, Number = 9 },
                    new House { Index = 7, Number = null, FenceBuilt = true },
                    new House { Index = 8, Number = 11, FenceBuilt = true, InFinalEstate = true },
                    new House { Index = 9, Number = null, FenceBuilt = true },
                    new House { Index = 10, Number = 13 },
                    new House { Index = 11, Number = 14 },
                }
            };

            // Act
            var estates = street.GetEstates();

            // Assert
            Assert.Equal(3, estates.Count);
            Assert.Equal(3, estates[0].HouseIndices.Count);
            Assert.Equal(0, estates[0].HouseIndices[0]);
            Assert.Equal(1, estates[0].HouseIndices[1]);
            Assert.Equal(2, estates[0].HouseIndices[2]);
            Assert.Single(estates[1].HouseIndices);
            Assert.Equal(8, estates[1].HouseIndices[0]);
            Assert.Equal(2, estates[2].HouseIndices.Count);
            Assert.Equal(10, estates[2].HouseIndices[0]);
            Assert.Equal(11, estates[2].HouseIndices[1]);
        }

        [Fact]
        public void GetPossibileNumbersForUnbuiltHouse_NormalCardSelected_ReturnsCorrectOptions()
        {
            // Arrange
            var street = new Street
            {
                Houses = new List<House>()
                {
                    new House { Index = 0, Number = 1 },
                    new House { Index = 1, Number = 2 },
                    new House { Index = 2, Number = 3 },
                    new House { Index = 3, Number = null },
                    new House { Index = 4, Number = 5 },
                    new House { Index = 5, Number = null },
                    new House { Index = 6, Number = 9 },
                    new House { Index = 7, Number = null },
                    new House { Index = 8, Number = 11 },
                    new House { Index = 9, Number = null },
                    new House { Index = 10, Number = 13 },
                    new House { Index = 11, Number = 14 },
                }
            };
            var numberEffectPair = new NumberEffectPair { Effect = CardType.Park, Number = 8 };

            // Act/Assert
            foreach (var house in street.Houses)
            {
                var possibleHouseNumbers = street.GetPossibileNumbersForUnbuiltHouse(house, numberEffectPair).ToArray();

                if (house.Index == 5)
                {
                    Assert.True(possibleHouseNumbers.Any());
                    Assert.Equal(8, Assert.Single(possibleHouseNumbers));
                }
                else
                {
                    Assert.False(possibleHouseNumbers.Any());
                }
            }


        }
    }
}