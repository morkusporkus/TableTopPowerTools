﻿namespace DMPowerTools.Tests.Extensions
{
    public class TetraCubeCreatureExtensionTests : IntegrationTestBase
    {

        [Fact]
        public async Task GivenATetraCubeCreature_WhenCreatingACreature_ThenCalculateArmorClass()
        {
            var mockTetraCubeCreature = CreateFakeTetraCubeCreature();
            mockTetraCubeCreature.ArmorName = "padded";
            var expectedResult = 11 + mockTetraCubeCreature.DexModifier + mockTetraCubeCreature.ShieldBonus;

            mockTetraCubeCreature.ArmorClass.Should().Be(expectedResult);
        }
    }
}
