using ApplicationInsight.Core.Entities;
using System;
using System.Collections.Generic;
using Xunit;

namespace ApplicationInsight.Core.Test
{
    public class ExpenseReportItemTest
    {
        #region [ Equals ]
        public static IEnumerable<object[]> SetOfObjectsForEqualsData =>
           new List<object[]>
           {
                new object[] {
                    new ExpenseReportItem() {Id=Guid.Parse("C6BD8A7D-09A8-4EEB-8649-F581881632BF") },
                    new ExpenseReportItem() {Id=Guid.Parse("C6BD8A7D-09A8-4EEB-8649-F581881632BF") },
                    true},
                new object[] {
                    new ExpenseReportItem() {Id=Guid.Parse("C6BD8A7D-09A8-4EEB-8649-F581881632BF") },
                    new ExpenseReportItem() ,
                    false},
                new object[] {
                    new ExpenseReportItem() {Id=Guid.Parse("C6BD8A7D-09A8-4EEB-8649-F581881632BF") },
                    "pippo",
                    false},
                new object[] {
                    new ExpenseReportItem() {Id=Guid.Parse("C6BD8A7D-09A8-4EEB-8649-F581881632BF") },
                    1,
                    false},
                new object[] {
                    new ExpenseReportItem() {Id=Guid.Parse("C6BD8A7D-09A8-4EEB-8649-F581881632BF") },
                    null,
                    false}
           };

        [Theory]
        [MemberData(nameof(SetOfObjectsForEqualsData))]
        public void Equals_TwoObjects_Result(ExpenseReportItem item1, object obj, bool expectedResult)
        {
            var actual = item1.Equals(obj);

            Assert.Equal(actual, expectedResult);
        }
        #endregion [ Equals ]

        #region [ == ]
        public static IEnumerable<object[]> SetOfObjectsForEqualOperatorData =>
           new List<object[]>
           {
                new object[] {
                    new ExpenseReportItem() {Id=Guid.Parse("C6BD8A7D-09A8-4EEB-8649-F581881632BF") },
                    new ExpenseReportItem() {Id=Guid.Parse("C6BD8A7D-09A8-4EEB-8649-F581881632BF") },
                    true},
                new object[] {
                    new ExpenseReportItem() {Id=Guid.Parse("C6BD8A7D-09A8-4EEB-8649-F581881632BF") },
                    new ExpenseReportItem() ,
                    false},
                new object[] {
                    new ExpenseReportItem(),
                    new ExpenseReportItem() {Id=Guid.Parse("C6BD8A7D-09A8-4EEB-8649-F581881632BF") },
                    false},
                new object[] {
                    new ExpenseReportItem() {Id=Guid.Parse("C6BD8A7D-09A8-4EEB-8649-F581881632BF") },
                    null,
                    false},
                new object[] {
                    null,
                    new ExpenseReportItem() {Id=Guid.Parse("C6BD8A7D-09A8-4EEB-8649-F581881632BF") },
                    false},
                new object[] {
                    null,
                    null,
                    true}
           };

        [Theory]
        [MemberData(nameof(SetOfObjectsForEqualOperatorData))]
        public void EqualOperator_TwoObjects_Result(ExpenseReportItem item1, ExpenseReportItem item2, bool expectedResult)
        {
            var actual = item1 == item2;

            Assert.Equal(actual, expectedResult);
        }
        #endregion [ == ]

        #region [ != ]
        public static IEnumerable<object[]> SetOfObjectsForNotEqualOperatorData =>
         new List<object[]>
         {
                new object[] {
                    new ExpenseReportItem() {Id=Guid.Parse("C6BD8A7D-09A8-4EEB-8649-F581881632BF") },
                    new ExpenseReportItem() {Id=Guid.Parse("C6BD8A7D-09A8-4EEB-8649-F581881632BF") },
                    false},
                new object[] {
                    new ExpenseReportItem() {Id=Guid.Parse("C6BD8A7D-09A8-4EEB-8649-F581881632BF") },
                    new ExpenseReportItem() ,
                    true},
                new object[] {
                    new ExpenseReportItem(),
                    new ExpenseReportItem() {Id=Guid.Parse("C6BD8A7D-09A8-4EEB-8649-F581881632BF") },
                    true},
                new object[] {
                    new ExpenseReportItem() {Id=Guid.Parse("C6BD8A7D-09A8-4EEB-8649-F581881632BF") },
                    null,
                    true},
                new object[] {
                    null,
                    new ExpenseReportItem() {Id=Guid.Parse("C6BD8A7D-09A8-4EEB-8649-F581881632BF") },
                    true},
                new object[] {
                    null,
                    null,
                    false}
         };
        [Theory]
        [MemberData(nameof(SetOfObjectsForNotEqualOperatorData))]
        public void NotEqualOperator_TwoObjects_Result(ExpenseReportItem item1, ExpenseReportItem item2, bool expectedResult)
        {
            var actual = item1 != item2;

            Assert.Equal(actual, expectedResult);
        }
        #endregion [ != ]
    }
}
