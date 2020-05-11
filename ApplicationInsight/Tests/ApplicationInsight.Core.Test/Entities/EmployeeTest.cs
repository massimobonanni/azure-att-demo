using ApplicationInsight.Core.Entities;
using System;
using System.Collections.Generic;
using Xunit;

namespace ApplicationInsight.Core.Test
{
    public class EmployeeTest
    {
        #region [ Equals ]
        public static IEnumerable<object[]> SetOfObjectsForEqualsData =>
           new List<object[]>
           {
                new object[] {
                    new Employee() {Id=Guid.Parse("C6BD8A7D-09A8-4EEB-8649-F581881632BF") },
                    new Employee() {Id=Guid.Parse("C6BD8A7D-09A8-4EEB-8649-F581881632BF") },
                    true},
                new object[] {
                    new Employee() {Id=Guid.Parse("C6BD8A7D-09A8-4EEB-8649-F581881632BF") },
                    new Employee() ,
                    false},
                new object[] {
                    new Employee() {Id=Guid.Parse("C6BD8A7D-09A8-4EEB-8649-F581881632BF") },
                    "pippo",
                    false},
                new object[] {
                    new Employee() {Id=Guid.Parse("C6BD8A7D-09A8-4EEB-8649-F581881632BF") },
                    1,
                    false},
                new object[] {
                    new Employee() {Id=Guid.Parse("C6BD8A7D-09A8-4EEB-8649-F581881632BF") },
                    null,
                    false}
           };

        [Theory]
        [MemberData(nameof(SetOfObjectsForEqualsData))]
        public void Equals_TwoObjects_Result(Employee employee1, object obj, bool expectedResult)
        {
            var actual = employee1.Equals(obj);

            Assert.Equal(actual, expectedResult);
        }
        #endregion [ Equals ]

        #region [ == ]
        public static IEnumerable<object[]> SetOfObjectsForEqualOperatorData =>
           new List<object[]>
           {
                new object[] {
                    new Employee() {Id=Guid.Parse("C6BD8A7D-09A8-4EEB-8649-F581881632BF") },
                    new Employee() {Id=Guid.Parse("C6BD8A7D-09A8-4EEB-8649-F581881632BF") },
                    true},
                new object[] {
                    new Employee() {Id=Guid.Parse("C6BD8A7D-09A8-4EEB-8649-F581881632BF") },
                    new Employee() ,
                    false},
                new object[] {
                    new Employee(),
                    new Employee() {Id=Guid.Parse("C6BD8A7D-09A8-4EEB-8649-F581881632BF") },
                    false},
                new object[] {
                    new Employee() {Id=Guid.Parse("C6BD8A7D-09A8-4EEB-8649-F581881632BF") },
                    null,
                    false},
                new object[] {
                    null,
                    new Employee() {Id=Guid.Parse("C6BD8A7D-09A8-4EEB-8649-F581881632BF") },
                    false},
                new object[] {
                    null,
                    null,
                    true}
           };

        [Theory]
        [MemberData(nameof(SetOfObjectsForEqualOperatorData))]
        public void EqualOperator_TwoObjects_Result(Employee employee1, Employee employee2, bool expectedResult)
        {
            var actual = employee1 == employee2;

            Assert.Equal(actual, expectedResult);
        }
        #endregion [ == ]

        #region [ != ]
        public static IEnumerable<object[]> SetOfObjectsForNotEqualOperatorData =>
         new List<object[]>
         {
                new object[] {
                    new Employee() {Id=Guid.Parse("C6BD8A7D-09A8-4EEB-8649-F581881632BF") },
                    new Employee() {Id=Guid.Parse("C6BD8A7D-09A8-4EEB-8649-F581881632BF") },
                    false},
                new object[] {
                    new Employee() {Id=Guid.Parse("C6BD8A7D-09A8-4EEB-8649-F581881632BF") },
                    new Employee() ,
                    true},
                new object[] {
                    new Employee(),
                    new Employee() {Id=Guid.Parse("C6BD8A7D-09A8-4EEB-8649-F581881632BF") },
                    true},
                new object[] {
                    new Employee() {Id=Guid.Parse("C6BD8A7D-09A8-4EEB-8649-F581881632BF") },
                    null,
                    true},
                new object[] {
                    null,
                    new Employee() {Id=Guid.Parse("C6BD8A7D-09A8-4EEB-8649-F581881632BF") },
                    true},
                new object[] {
                    null,
                    null,
                    false}
         };
        [Theory]
        [MemberData(nameof(SetOfObjectsForNotEqualOperatorData))]
        public void NotEqualOperator_TwoObjects_Result(Employee employee1, Employee employee2, bool expectedResult)
        {
            var actual = employee1 != employee2;

            Assert.Equal(actual, expectedResult);
        }
        #endregion [ != ]
    }
}
