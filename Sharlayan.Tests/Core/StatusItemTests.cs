// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StatusItemTests.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2026 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   Pins the StatusCategory → IsBeneficial / IsDetrimental mapping on Core.StatusItem.
//   The 1 = beneficial / 2 = detrimental values come from the game's Status Excel sheet
//   (the same discriminator the in-game UI uses for the green/red icon border); these
//   tests stop a refactor from silently swapping or shifting them.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Tests.Core {
    using Sharlayan.Core;

    using Xunit;

    public class StatusItemTests {
        [Fact]
        public void StatusCategory1_IsBeneficial_NotDetrimental() {
            StatusItem item = new StatusItem { StatusCategory = 1 };
            Assert.True(item.IsBeneficial);
            Assert.False(item.IsDetrimental);
        }

        [Fact]
        public void StatusCategory2_IsDetrimental_NotBeneficial() {
            StatusItem item = new StatusItem { StatusCategory = 2 };
            Assert.True(item.IsDetrimental);
            Assert.False(item.IsBeneficial);
        }

        [Fact]
        public void StatusCategory0_IsNeither() {
            // 0 = system statuses, or a status id that didn't resolve against
            // XIVDatabase (StatusEffectLookup's DefaultStatusInfo leaves it 0).
            StatusItem item = new StatusItem();
            Assert.False(item.IsBeneficial);
            Assert.False(item.IsDetrimental);
        }

        [Theory]
        [InlineData(3)]
        [InlineData(byte.MaxValue)]
        public void StatusCategory_UnknownValues_AreNeither(byte category) {
            // Future-proofing: if the game ever adds category 3+, neither flag
            // should light up until the mapping is deliberately extended.
            StatusItem item = new StatusItem { StatusCategory = category };
            Assert.False(item.IsBeneficial);
            Assert.False(item.IsDetrimental);
        }
    }
}
