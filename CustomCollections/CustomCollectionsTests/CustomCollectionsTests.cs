using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using CustomCollections;

namespace CustomCollectionsTests
{
    public class CustomHashTableTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Clear_HashTableIsEmpty_ReturnTrue()
        {
            CustomHashTable<int, string> hashTable = new CustomHashTable<int, string>
            {
                { 1, "1"},
                { 2, "2"},
                { 3, "3"}
            };

            hashTable.Clear();

            Assert.IsEmpty(hashTable);
        }

        [Test]
        public void Contains_SomeElement_ReturnTrue()
        {
            CustomHashTable<string, int> hashTable = new CustomHashTable<string, int>
            {
                { "1", 1 },
                { "2", 2 },
                { "13", 35 }
            };

            var containsItem = hashTable.Contains(new KeyValuePair<string, int>("13", 35));

            Assert.IsTrue(containsItem);
        }

        [Test]
        public void Contains_SomeElement_ReturnFalse()
        {
            CustomHashTable<string, int> hashTable = new CustomHashTable<string, int>
            {
                { "1", 1 },
                { "2", 2 },
                { "13", 35 }
            };

            var containsItem = hashTable.Contains(new KeyValuePair<string, int>("13", 31));

            Assert.IsFalse(containsItem);
        }

        [Test]
        public void TryGetValue_SomeElement_ReturnTrue()
        {
            CustomHashTable<string, string> hashTable = new CustomHashTable<string, string>
            {
                { "1", "One" },
                { "2", "Two" },
                { "4", "Four" },
                { "5", "Five" },
                { "6", "Six" },
                { "13", "Three, No" }
            };

            var containsItem = hashTable.TryGetValue("13", out var value);

            Assert.IsTrue(containsItem);
            Assert.AreEqual(value, "Three, No");
        }

        [Test]
        public void TryGetValue_SomeElement_ReturnFalse()
        {
            CustomHashTable<string, int> hashTable = new CustomHashTable<string, int>
            {
                { "1", 1 },
                { "2", 2 },
                { "4", 3 },
                { "5", 4 },
                { "6", 5 },
                { "13", 6 }
            };

            var containsItem = hashTable.TryGetValue("3", out var value);

            Assert.IsFalse(containsItem);
            Assert.AreEqual(value, default(int));

        }

        [Test]
        public void CopyTo_ArrayContainItemsWithHashTable_ReturnTrue()
        {
            CustomHashTable<int, char> hashTable = new CustomHashTable<int, char>
            {
                { 1, 't' },
                { 2, 'y' },
                { 4, 'i' },
                { 5, 'c' },
                { 6, 'r' },
                { 13, 'o' }
            };
            KeyValuePair<int, char>[] array = new KeyValuePair<int, char>[7];
            array[0] = new KeyValuePair<int, char>(0, 'v');
            
            hashTable.CopyTo(array, 1);

            for (int i = 1; i < 7; i++)
            {
                Assert.AreEqual(array[i].Value, hashTable[array[i].Key]);
            }
        }

        [Test]
        public void CopyTo_ArrayContainItemsWithHashTable_ReturnFalse()
        {
            CustomHashTable<int, char> hashTable = new CustomHashTable<int, char>
            {
                { 1, 't' },
                { 2, 'y' },
                { 4, 'i' },
                { 5, 'c' },
                { 6, 'r' },
                { 13, 'o' }
            };
            KeyValuePair<int, char>[] array = new KeyValuePair<int, char>[5];
            array[0] = new KeyValuePair<int, char>(0, 'v');

            try
            {
                hashTable.CopyTo(array, 1);
                Assert.IsTrue(true);
            }
            catch
            {
                Assert.IsFalse(false);
            }
        }

        [Test]
        public void Add_SameElement_ReturnFalse()
        {
            CustomHashTable<int, string> hashTable = new CustomHashTable<int, string>()
            {
                {13, "1"}
            };

            try
            {
                hashTable.Add(new KeyValuePair<int, string>(13, "2"));
                Assert.IsTrue(true);
            }
            catch
            {
                Assert.IsFalse(false);
            }
        }

        [Test]
        public void Remote_Element_ReturnTrue()
        {
            CustomHashTable<string, int> hashTable = new CustomHashTable<string, int>
            {
                { "1", 1 },
                { "2", 2 },
                { "13", 35 }
            };

            var isRemote = hashTable.Remove(new KeyValuePair<string, int>("13", 35));

            Assert.IsTrue(isRemote);
        }

        [Test]
        public void Remote_Element_ReturnFalse()
        {
            CustomHashTable<string, int> hashTable = new CustomHashTable<string, int>
            {
                { "1", 1 },
                { "2", 2 },
                { "13", 35 }
            };

            var isRemote = hashTable.Remove("3");

            Assert.IsFalse(isRemote);
        }

        [Test]
        public void GetEnumerator_GetAllElements_GetTrue()
        {
            CustomHashTable<int, string> hashTable = new CustomHashTable<int, string>
            {
                { 1, "1"},
                { 2, "2"},
                { 3, "3"},
                { 4, "4"},
                { 5, "5"},
                { 6, "6"}
            };
            KeyValuePair<int, string>[] sameElements = new KeyValuePair<int, string>[]{
                new KeyValuePair<int, string>( 1, "1"),
                new KeyValuePair<int, string>( 2, "2"),
                new KeyValuePair<int, string>( 3, "3"),
                new KeyValuePair<int, string>( 4, "4"),
                new KeyValuePair<int, string>( 5, "5"),
                new KeyValuePair<int, string>( 6, "6")
            };

            foreach (var elemen in hashTable)
            {
                Assert.IsTrue(sameElements.Contains(elemen));
            }
        }
    }
}